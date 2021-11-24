using Cinemachine;
using UnityEngine;
using Photon.Pun;

namespace Com.Tereshchuk.Shooter
{
    public class SpawnManager : MonoBehaviourPunCallbacks
    {
        [SerializeField] private string playerPrefab;
        [SerializeField] private string cameraPrefabName;
        [SerializeField] private Transform[] spawnPoints;
        [SerializeField] private KillListContainer killListContainer;
        private int startingLayer = 12;
        private int countPlayers=1;

        private void Start()
        {
            Spawn();
        }

        public void ConfigLayers(Camera main , CinemachineVirtualCamera followCamera)
        {
            main.gameObject.layer = startingLayer;
            main.cullingMask+= LayerMask.GetMask("P"+countPlayers);
            followCamera.gameObject.layer = startingLayer;
            countPlayers++;
            startingLayer++;
        }

        public void UpdateDeathList(int view1,int view2)
        {
            killListContainer.ShowNewDeath(view1,view2);
        }

        public int GetPlayerLayer()
        {
            return startingLayer;
        }
        public void Spawn()
        {
            Transform tempSpawn = spawnPoints[Random.Range(0, spawnPoints.Length)];
            GameObject cameraObj = PhotonNetwork.Instantiate(cameraPrefabName,tempSpawn.position,tempSpawn.rotation);
            cameraObj.SetActive(true);
            cameraObj.tag = "BusyCamera";
            Camera cameraTmp = cameraObj.GetComponent<Camera>();
            
            
            GameObject playerObj = PhotonNetwork.Instantiate(playerPrefab,tempSpawn.position,tempSpawn.rotation);
            
            SkinApply skinTool = playerObj.GetComponentInChildren<SkinApply>();
            skinTool.ApplySettings();

            //playerObj.GetComponent<PlayerController>().SetCamera(cameraTmp);
            playerObj.GetComponent<PlayerController>().SetCamera(cameraObj.GetPhotonView().ViewID);
            Debug.Log("CAMERA SET !");
            playerObj.layer = startingLayer;
            ConfigLayers(cameraTmp, playerObj.GetComponent<PlayerController>().followCamera);
        }
        
    }
}