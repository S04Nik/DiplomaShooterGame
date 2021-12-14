using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

namespace Com.Tereshchuk.Shooter.NewWeapon_Inventory_System
{
    public class FirearmItem:InventoryItem
    {
        public Transform raycastOrigin;
        public ParticleSystem[] muzzleFlash;
        public ParticleSystem[] hitEffects;
        public Action ChangeCanvasAimingDot;
        
        // private AmmoWidjet _ammoWidget;
        private GunSoundController _soundController;
        private bool isReloading;
        private Ray _ray;
        private RaycastHit _hitInfo;
        private float _accumulatedTime;
        private List<Bullet> _bullets = new List<Bullet>();
        private float _maxLifeTime = 1.0f;
        private Transform _raycastDestination;
        private WeaponRecoil recoilManager;
        private ReloadWeapon reloadManager;
        
        [SerializeField]private Gun loadOut;
        [SerializeField] private ParticleSystem bulletShellDrop;
        [SerializeField] WeaponAnimationController weaponAnimationController;
        class Bullet
        {
            public float Time;
            public Vector3 InitialPosition;
            public Vector3 InitialVelocity;
            public TrailRenderer Tracer;
        }

        private void Awake()
        {
            ItemInfo = loadOut;
            loadOut.Initialize();
            reloadManager = GetComponent<ReloadWeapon>();
            recoilManager = GetComponent<WeaponRecoil>();
            // _ammoWidget = GetComponent<AmmoWidjet>();
            _soundController = GetComponent<GunSoundController>();
        }
        [PunRPC]
        public void RPCInitializeAudioGrenade()
        {
            _soundController.Initialize();
        }
        private void Start()
        {
            photonView.RPC(nameof(RPCInitializeAudioGrenade),RpcTarget.All);
        }
        public void SetRaycastDestination(Camera mainCamera)
        {
            _raycastDestination = mainCamera.gameObject.transform.Find("CrossTarget").transform;
            recoilManager.SetCamera(mainCamera);
        }
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
        public override void UpdateItem()
        {
            if (photonView.IsMine)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    _soundController.Fire();
                    UseItem();
                }

                //photonView.RPC(nameof(UpdateBullets),RpcTarget.All,Time.deltaTime);
                UpdateBullets(Time.deltaTime);

                if (Input.GetMouseButtonUp(0))
                {
                    StopFiring();
                }
            }
        }
        public override bool Check()
        {
            if (!reloadManager.isReloading && !IsActivated)
            {
                return true;
            }
            else
                return false;
        }

        public override void UseItem()
        {
            IsActivated = true;
            _accumulatedTime = 0.0f;
            recoilManager.Reset();
            UpdateFiring(Time.deltaTime);
            //FireBullet();
        }
        public override void Initialize(Transform parent)
        {
            PlayerController pc = parent.GetComponent<PlayerController>();
            reloadManager.Initialize(parent.GetComponent<InventoryController>().GetLeftHand(), loadOut);
            SetRaycastDestination(pc.mainCamera);
            
            WeaponAnimationEvents weaponAnim = parent.GetComponentInChildren<WeaponAnimationEvents>();
            weaponAnimationController.Initialize(weaponAnim);
            recoilManager.Initialize(parent.GetComponentInParent<CharacterAiming>(),weaponAnimationController);
        }

        public override void SetUI(Transform ui)
        {
        }

        public override string GetName()
        {
            return loadOut.name;
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
                _bullets.RemoveAll(bullet => bullet.Time >= _maxLifeTime);
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
                Debug.DrawLine(_ray.origin,_hitInfo.point,Color.red,10.0f);

                bullet.Tracer.transform.position = _hitInfo.point;
                bullet.Time = _maxLifeTime;

                if (_hitInfo.transform.CompareTag("Player"))
                {
                    PlayerController tmpr = _hitInfo.transform.GetComponent<PlayerController>();
                    tmpr.photonView.RPC("TakeDamage", RpcTarget.All, loadOut.damage, photonView.ViewID, _hitInfo.point);
                    //ChangeCanvasAimingDot(); !!!!
                }
                else
                {
                    hitEffects[0].transform.position = _hitInfo.point;
                    hitEffects[0].transform.forward = _hitInfo.normal;
                    hitEffects[2].transform.position = _hitInfo.point;
                    hitEffects[2].transform.forward = _hitInfo.normal;
                    // if metal if other
                    hitEffects[0].Emit(1);
                    hitEffects[2].Emit(1);
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
            if (loadOut.Fire())
            {
                photonView.RPC(nameof(EmmitParticles), RpcTarget.All);

                Vector3 velocity = (_raycastDestination.position - raycastOrigin.position).normalized *
                                   loadOut.bulletSpeed;
                var bullet = CreateBullet(raycastOrigin.position, velocity);
                _bullets.Add(bullet);

                recoilManager.GenerateRecoil(loadOut.name);
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

        public void StopFiring()
        {
            IsActivated = false;
        }
    }
}