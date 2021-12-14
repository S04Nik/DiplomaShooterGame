using Cinemachine;
using Com.Tereshchuk.Shooter.NewWeapon_Inventory_System;
using UnityEngine;
using Photon.Pun;
using Unity.Mathematics;

namespace Com.Tereshchuk.Shooter
{
    public class PlayerController : MonoBehaviourPunCallbacks
    {
        #region Variables
        
        [SerializeField]
        public CinemachineVirtualCamera followCamera;
        public Camera mainCamera;
        public MultiAimConstSetUp rigging;
        [SerializeField] private InventoryController inventoryController;
        private SpawnManager _spawnManager;
        private Transform _canvasHitSuccess;
        public Light light;
        [SerializeField] private Transform leftHand;

        [SerializeField] private PlayerAudioController playerAudioController;
        [SerializeField] private ParticleSystem hitEffectPrefab;
        [SerializeField] private HealthController healthController;
        private ParticleSystem _hitEffect;
        
        #endregion

        #region MonoBehaviour Callbacks
        
        public void SetGenderVoice(int gender)
        {
            playerAudioController.SetVoice(gender);
        }
        private void Awake()
        {
            // cameras on client not setting properly / null exception
            if (photonView.IsMine) return;
            mainCamera = GameObject.FindWithTag("FreeCamera").GetComponent<Camera>();
            mainCamera.gameObject.tag = "BusyCamera";

        }

        public void Start()
        {
            _spawnManager = GameObject.Find("Manager").GetComponent<SpawnManager>();
            light = GameObject.FindWithTag("Light").GetComponent<Light>();
            light.intensity = PlayerPrefs.GetFloat("masterBrightness");
            _hitEffect = Instantiate(hitEffectPrefab, transform.position, quaternion.identity, transform);

            if (!photonView.IsMine) // LAYERS
            {
                mainCamera.enabled = false;
                mainCamera.GetComponent<CinemachineBrain>().gameObject.SetActive(false);
                mainCamera.GetComponent<AudioListener>().gameObject.SetActive(false);
                followCamera.enabled = false;
                gameObject.layer = _spawnManager.GetPlayerLayer();
                _spawnManager.ConfigLayers(mainCamera,followCamera);
            }

            if (photonView.IsMine)
            {
                _canvasHitSuccess = GameObject.FindWithTag("CanvasAimingSuccessHit").transform;
                _canvasHitSuccess.gameObject.SetActive(false);
                //inventoryController.SetCanvasSuccesHitImg(_canvasHitSuccess);
            }
            rigging.InitializeBonesConstraints();
        }

        void Update()
        {
            if (!photonView.IsMine)
            {
                return;
            }
            healthController.UpdateHealth();
        }


        #endregion
        
        #region Public Methods
        
        [PunRPC]
        public void RPCCameraSet(int cameraViewID)
        {
            mainCamera = PhotonView.Find(cameraViewID).GetComponent<Camera>();
        }
        public void SetCamera(int cameraViewID)
        {
            photonView.RPC(nameof(RPCCameraSet),RpcTarget.AllBuffered,cameraViewID);
        }
        
        [PunRPC]
        public void UpdateKillList(int view1,int view2)
        {
            _spawnManager.UpdateDeathList(view1,view2);
        }
        
        [PunRPC]
        public void TakeDamage(int damage , int enemyId,Vector3 hitInfoPoint)
        {
            Debug.Log("!@# TAKE DAMAGE");
            if (photonView.IsMine)
            {
                if (!healthController.DecreaseHealth(damage))
                {
                    inventoryController.ItemWidjet.ClearWidjets();
                    photonView.RPC(nameof(UpdateKillList),RpcTarget.All,enemyId,photonView.ViewID);
                    // wont find if it will be disactive
                    _canvasHitSuccess.gameObject.SetActive(true);
                    _spawnManager.Spawn();
                    PhotonNetwork.Destroy(gameObject);
                    PhotonNetwork.Destroy(mainCamera.gameObject);
                }
                playerAudioController.PlayPainVoice();
            }
            // BLEEDING
            _hitEffect.transform.position = hitInfoPoint;
             _hitEffect.Play();
        }

        #endregion
    }

}
