using System;
using System.Collections;
using Cysharp.Threading.Tasks;
using Photon.Pun;
using UnityEngine;
using Task = System.Threading.Tasks.Task;
using Vector3 = UnityEngine.Vector3;

namespace Com.Tereshchuk.Shooter.NewWeapon_Inventory_System
{
    public class GrenadeItem:InventoryItem
    {
        [SerializeField] private int duration;
        [SerializeField] protected Throwable loadOut;
        [SerializeField] protected WeaponAnimationController weaponAnimationController;
        [SerializeField] protected GrenadeSoundController soundController;
        public LineRenderer lineRendererPrefab;
        private LineRenderer _line;
        private Transform _raycastDestination;
        private Rigidbody _rb;
        [SerializeField] protected MeshCollider colliderForCollision;
        private InventoryController _inventoryController;
        [SerializeField] protected ParticleSystem explosionParticle;
        private float radius = 20f;
        private float delay = 3f;
        // private float explosionForce = 10f;
        public float _countDown;
        public bool _hasExploded;
        public bool _timeForBoom;
        // private IEnumerator DestroyAfterTime;
        
        private static float _maxDistancePositiv = 20f;
        private static float _maxDistanceNegative = -20f;
        private  float _tmprDistance ;
        private float _flyH = 0.1f;
        private float gravity = -6f;
        private bool isFLying;

        public bool needLine;
        private struct LaunchData
        {
            public readonly Vector3 initialVelocity;
            public readonly float timeToTarget;

            public LaunchData(Vector3 initialVelocity, float timeToTarget)
            {
                this.initialVelocity = initialVelocity;
                this.timeToTarget = timeToTarget;
            }
        }

        private void DrawPath()
        {
            _line.positionCount = 0;
            _line.positionCount++;
            _line.SetPosition(_line.positionCount-1,transform.position);
            LaunchData launchData = CalculateLaunchData();
            Vector3 previousDrawPoint = transform.position;
            int resolution = 10;
            for (int i = 1; i <= resolution; i++)
            {
                float simulationTime = i / (float) resolution * launchData.timeToTarget;
                Vector3 displacement = launchData.initialVelocity * simulationTime + Vector3.up * gravity * simulationTime *
                    simulationTime / 2f;
                Vector3 drawPoint = transform.position + displacement;
                _line.positionCount++;
                _line.SetPosition(_line.positionCount-1,drawPoint);
                previousDrawPoint = drawPoint;
            }
        }
        
        [PunRPC]
        private void Launch()
        {
            weaponAnimationController.PlayFire();
            transform.parent = null;
            colliderForCollision.enabled = true;
            isFLying = true;
            _rb.useGravity = true;
            _rb.velocity = CalculateLaunchData().initialVelocity;
            Physics.gravity = Vector3.up * gravity;
        }
        private LaunchData CalculateLaunchData()
        {
            // if cross target not use raycast it works
            _flyH = 0.1f;
            if (_flyH < _raycastDestination.position.y)
            {
                _flyH = _raycastDestination.position.y;
            }
            float distance;
            distance = _raycastDestination.TransformPoint(Vector3.zero).z - transform.position.z;

            if (distance > 20f || distance < -20f)
            {
                if (_raycastDestination.TransformPoint(Vector3.zero).z < transform.position.z)
                {
                    _tmprDistance = _maxDistancePositiv;
                }
                else
                {
                    _tmprDistance = _maxDistanceNegative;
                }
            }
            else
            {
                _tmprDistance = _raycastDestination.position.z;
            }
            
            _tmprDistance -= transform.position.z;
      
            float displacementY = _raycastDestination.position.y - transform.position.y;
            Vector3 displacementXZ = new Vector3(_raycastDestination.position.x - transform.position.x, 0,
                _tmprDistance);
            
            Vector3 velocityY = Vector3.up * Mathf.Sqrt(-2 * gravity * _flyH);
            float time = (float)Math.Sqrt(-2 * _flyH / gravity) + Mathf.Sqrt(2 * (displacementY - _flyH) / gravity);
            Vector3 velocityXZ = displacementXZ / time ;
            return new LaunchData(velocityXZ + velocityY,time);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (photonView.IsMine)
            {
                colliderForCollision.isTrigger = false;
                if (!_hasExploded && _timeForBoom)
                {
                    explosionParticle.transform.rotation = other.transform.rotation;
                    if (loadOut.damage > 0)
                    {
                        Collider[] hitColliders = Physics.OverlapSphere(transform.position, radius);
                        foreach (var nearbyObj in hitColliders)
                        {
                            PlayerController pc = nearbyObj.GetComponent<PlayerController>();
                            if (pc != null)
                            {
                                pc.photonView.RPC("TakeDamage", RpcTarget.All, loadOut.damage, photonView.ViewID, nearbyObj.transform.position);
                                //rb.AddExplosionForce(explosionForce,transform.position,radius);
                            }
                        }
                    }
                    _hasExploded = true;
                    Task.Run(Pause);
                }
            }
            else
            {
                colliderForCollision.isTrigger = false;
            }
        }

        private async UniTaskVoid Pause()
        {
            await UniTask.Delay(duration*1000);
            if (explosionParticle.particleCount > 500)
            {
                explosionParticle.Stop();
                await UniTask.Delay(10000);
            }
            PhotonNetwork.Destroy(this.gameObject);
        }
        [PunRPC]
        private void Explode()
        {
            _timeForBoom = true;
            colliderForCollision.isTrigger = true;
            explosionParticle.Play();
            soundController.Fire();
        }
        private void Awake()
        {
            SlotNumber = loadOut.slot;
            loadOut.Initialize();
            ItemInfo = loadOut;
            _rb = GetComponent<Rigidbody>();
            _rb.useGravity = false;
            _countDown = delay;
            soundController = GetComponent<GrenadeSoundController>();
        }

        [PunRPC]
        public void RPCInitializeAudioGrenade()
        {
            soundController.Initialize();
        }
        private void Start()
        {
            photonView.RPC(nameof(RPCInitializeAudioGrenade),RpcTarget.All);
        }

        private void Update()
        {
            if (photonView.IsMine)
            {
                if (weaponAnimationController.GrenadePrepared()&&!isFLying)
                {
                    if (needLine)   
                    {
                        if(!_line) _line = Instantiate(lineRendererPrefab);
                        DrawPath();
                    }else
                    {
                        if (_line)
                        {
                            Destroy(_line.gameObject);
                        }
                        needLine = false;
                        // if (_line)
                        // {
                        //     _line.positionCount = 0;
                        // }
                    }
                }
                else
                {
                    if (_line)
                    {
                        Destroy(_line.gameObject);
                    }
                    needLine = false;
                }

                if (isFLying)
                {
                    _countDown -= Time.deltaTime;
                    if (_countDown <= 0f && !_hasExploded)
                    {
                        photonView.RPC(nameof(Explode),RpcTarget.All);
                    }
                }
            }

        }
        [PunRPC]
        private void RemoveUsedGrenade()
        {
            weaponAnimationController.PlayUnArmed();
                    
            _inventoryController.RemoveUsedItem(loadOut.slot);
        }
        public override void UpdateItem()
        {
            if (photonView.IsMine)
            {
                if (loadOut.GetClip()<=0)
                {
                    photonView.RPC(nameof(RemoveUsedGrenade),RpcTarget.All);
                }
                
                if (weaponAnimationController.GrenadePrepared() && !isFLying)
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        needLine = false;
                        if (loadOut.Fire())
                        {
                            UseItem();
                        }
                    }
                    else
                    {
                        needLine = true;
                    }
                }
            }

          
        }
        
        public override void UseItem()
        {
            if (photonView.IsMine)
            {
                Destroy(_line.gameObject);
                photonView.RPC(nameof(Launch),RpcTarget.All);
            }
        }
        public void SetRaycastDestination(Camera mainCamera)
        {
            _raycastDestination = mainCamera.gameObject.transform.Find("CrossTarget").transform;
        }
        public override void Initialize(Transform parent)
        {
            PlayerController pc = parent.GetComponent<PlayerController>();
            WeaponAnimationEvents weaponAnim = parent.GetComponentInChildren<WeaponAnimationEvents>();
            weaponAnimationController.Initialize(weaponAnim);
            weaponAnimationController.AnimationEvents.WeaponAnimationEvent.AddListener(OnAnimationEvent);
            SetRaycastDestination(pc.mainCamera);
            _inventoryController = parent.GetComponent<InventoryController>();
        }

        public override void SetUI(Transform UI)
        {
        
        }

        public override string GetName()
        {
            return loadOut.name;
        }

        public override bool Check()
        {
            return true;
        }
        protected void OnAnimationEvent(string eventName)
        {
            switch (eventName)
            {
                case "show_grenade":
                    ShowGrenade();
                    break;
                case "hide_grenade":
                        HideGrenade();
                    break;
                case "use_pin":
                    soundController.UsePin();
                    break;
                case"destroy_grenade_way":
                    if (_line)
                    {
                        Destroy(_line.gameObject);
                    }
                    break;
            }
        }
        public void ShowGrenade()
        {
            if(IsActivated)
                gameObject.SetActive(true);
        }
        public void HideGrenade()
        {
            if(!IsActivated)
                gameObject.SetActive(false); 
        }
    }
}