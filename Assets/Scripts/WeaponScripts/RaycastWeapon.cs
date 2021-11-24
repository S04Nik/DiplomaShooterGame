using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

namespace Com.Tereshchuk.Shooter
{
    public class RaycastWeapon : MonoBehaviourPun
    {
        public bool isFiring;
        public Gun loadOut;
        public GameObject magazine;
        public Transform raycastOrigin;
    //    public TrailRenderer trailRenderer;
        public WeaponRecoil recoil;
        public ParticleSystem[] muzzleFlash;
        public ParticleSystem[] hitEffects;
        public Action ChangeCanvasAimingDot;
        class Bullet
        {
            public float Time;
            public Vector3 InitialPosition;
            public Vector3 InitialVelocity;
            public TrailRenderer Tracer;
        }
        private Transform _raycastDestination;
        private Ray _ray;
        private RaycastHit _hitInfo;
        private float _accumulatedTime;
        private List<Bullet> _bullets = new List<Bullet>();
        private float _maxLifeTime = 1.0f;
        private WeaponSoundController _soundController;
        [SerializeField] private AudioClip fireClip;
        [SerializeField] private AudioClip attachMagazineClip;
        [SerializeField] private AudioClip detachMagazineClip;
        [SerializeField] private ParticleSystem bulletShellDrop;



        Vector3 GetPosition(Bullet bullet)
        {
            Vector3 gravity = Vector3.down * loadOut.bulletDrop;
            return (bullet.InitialPosition) + (bullet.InitialVelocity * bullet.Time) +
                   (0.5f * gravity * bullet.Time * bullet.Time);
        }

        Bullet CreateBullet(Vector3 position, Vector3 velocity)
        {
            Bullet bullet = new Bullet();
            bullet.InitialPosition = position;
            bullet.InitialVelocity = velocity;
            bullet.Time = 0.0f;
            bullet.Tracer = Instantiate(loadOut.BulletTrailRenderer, position, Quaternion.identity);
            bullet.Tracer.AddPosition(position);
            return bullet;
        }

        private void Awake()
        {
            recoil = GetComponent<WeaponRecoil>();
        }

        public void SetCamera(Camera mainCamera)
        {
            _raycastDestination = mainCamera.gameObject.transform.Find("CrossTarget").transform;
            recoil.SetCamera(mainCamera);
        }


        public void SetAudioController(WeaponSoundController audioController)
        {
            _soundController = audioController;

        }

        public void PlaySoundDetachingMagazine()
        {
            _soundController.DetachMagazine(detachMagazineClip);
        }

        public void PlaySoundAttachingMagazine()
        {
            _soundController.AttachMagazine(attachMagazineClip);
        }

        private void OnEnable()
        {
            loadOut.Initialize();
        }

        public void UpdateWeapon()
        {
            if (photonView.IsMine)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    _soundController.Fire(fireClip);
                    StartFiring();
                }

                if (isFiring)
                {
                    UpdateFiring(Time.deltaTime);
                }

                // photonView.RPC("UpdateBullets",RpcTarget.All,Time.deltaTime);
                UpdateBullets(Time.deltaTime);

                if (Input.GetMouseButtonUp(0))
                {
                    StopFiring();
                }
            }
        }

        public void UpdateFiring(float deltaTime)
        {
            _accumulatedTime += deltaTime;
            float fireInterval = 20f / loadOut.fireRate;

            while (_accumulatedTime >= 0.0f)
            {
                FireBullet();
                _accumulatedTime -= fireInterval;
            }
        }

        void SimulateBullets(float deltaTime)
        {
            _bullets.ForEach(bullet =>
            {
                Vector3 p0 = GetPosition(bullet);
                bullet.Time += deltaTime;
                Vector3 p1 = GetPosition(bullet);
                RaycastSegment(p0, p1, bullet);
            });
            
        }

        public void UpdateBullets(float deltaTime)
        {
            SimulateBullets(deltaTime);
            DestroyBullets();
        }

        void DestroyBullets()
        {
            if (_bullets.Count > 0)
            {
                _bullets.RemoveAll(bullet =>
                {
                    // if (bullet.time >= maxLifeTime)
                    // {
                    //     Destroy(bullet.tracer);
                    // }
                    return bullet.Time >= _maxLifeTime;
                });
         
            }

        }

        void RaycastSegment(Vector3 start, Vector3 end, Bullet bullet)
        {
            Vector3 direction = end - start;
            float distance = direction.magnitude;
            _ray.origin = start;
            _ray.direction = direction;

            if (Physics.Raycast(_ray, out _hitInfo, distance))
            {
                Debug.DrawLine(_ray.origin,_hitInfo.point,Color.red,1.0f);

                bullet.Tracer.transform.position = _hitInfo.point;
                bullet.Time = _maxLifeTime;

                if (_hitInfo.transform.CompareTag("Player"))
                {

                    PlayerController tmpr = _hitInfo.transform.GetComponent<PlayerController>();
                    tmpr.photonView.RPC("TakeDamage", RpcTarget.All, loadOut.damage, photonView.ViewID, _hitInfo.point,
                        _hitInfo.normal);
                    ChangeCanvasAimingDot();
                }
                else
                {
                    hitEffects[0].transform.position = _hitInfo.point;
                    hitEffects[0].transform.forward = _hitInfo.normal;
                    // if metal if other
                    hitEffects[0].Emit(1);
                }


                //Collision Impulse
                var rb2d = _hitInfo.collider.GetComponent<Rigidbody>();
                if (rb2d)
                {
                    rb2d.AddForceAtPosition(_ray.direction * 20, _hitInfo.point, ForceMode.Impulse);
                }
            }
            else
            {
                if(bullet.Tracer!=null)
                {
                    bullet.Tracer.transform.position = end;
                }
            }
        }

        public void FireBullet()
        {
            if (loadOut.FireBullet())
            {
                photonView.RPC("EmmitParticles", RpcTarget.All);

                Vector3 velocity = (_raycastDestination.position - raycastOrigin.position).normalized *
                                   loadOut.bulletSpeed;
                var bullet = CreateBullet(raycastOrigin.position, velocity);
                _bullets.Add(bullet);

                recoil.GenerateRecoil(loadOut.name);
            }
        }

        [PunRPC]
        public void EmmitParticles()
        {
            bulletShellDrop.Emit(1); // выпадает гильза 

            foreach (var particle in muzzleFlash)
            {
                particle.Emit(1); // ефект стрельбы
            }
        }

        public void StartFiring()
        {
            isFiring = true;
            _accumulatedTime = 0.0f;
            recoil.Reset();
            //FireBullet();
        }

        public void StopFiring()
        {
            isFiring = false;
        }
    }
}