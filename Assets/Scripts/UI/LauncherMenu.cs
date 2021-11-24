using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;


namespace Com.Tereshchuk.Shooter
{
    public class LauncherMenu : MonoBehaviourPunCallbacks // allow to use callbacks that photon has
    {
        public static LauncherMenu Instance;
        
        [SerializeField] private TMP_InputField roomNameInputField;
        [SerializeField] private TMP_Text errorTxt;
        [SerializeField] private TMP_Text roomNameTxt;
        [SerializeField] private Transform roomListContent;
        [SerializeField] private Transform playerListContent;
        [SerializeField] private GameObject roomListItemPrefab;
        [SerializeField] private GameObject playerListItemPrefab;
        [SerializeField] private GameObject startGameBtn;
        [SerializeField] private PlayerCustomization playerCustomizationTool;
        
        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            Debug.Log("CONNECTING TO MASTER");
            PhotonNetwork.ConnectUsingSettings();
        }

        public override void OnConnectedToMaster()
        {
            Debug.Log("CONNECTED TO MASTER");
            PhotonNetwork.JoinLobby();
            PhotonNetwork.AutomaticallySyncScene = true;
        }

        public override void OnJoinedLobby()
        {
            MenuManager.Instance.OpenMenu("Title");
            Debug.Log("Joined Lobby");
            PhotonNetwork.NickName = "Player" + Random.Range(0, 1000).ToString("0000");
            
            // setting default skin for situation if he wont customize chr by himself
            playerCustomizationTool.SetDefaultSkin();
        }

        public void CreateRoom()
        {
            if (string.IsNullOrEmpty(roomNameInputField.text))
            {
                return;
            }
            PhotonNetwork.CreateRoom(roomNameInputField.text);
            MenuManager.Instance.OpenMenu("Loading");
        }

        public override void OnJoinedRoom()
        {
            MenuManager.Instance.OpenMenu("Room Menu");
            roomNameTxt.text = PhotonNetwork.CurrentRoom.Name;
            
            Player[] players = PhotonNetwork.PlayerList;

            foreach (Transform t in playerListContent)
            {
                Destroy(t.gameObject);
            }

            foreach (var player in players)
            {
                Instantiate(playerListItemPrefab,playerListContent).GetComponent<PlayerListItem>().SetUp(player);
            }
            
            startGameBtn.SetActive(PhotonNetwork.IsMasterClient);
        }

        public override void OnMasterClientSwitched(Player newMasterClient)
        {
            startGameBtn.SetActive(PhotonNetwork.IsMasterClient);
        }

        public override void OnCreateRoomFailed(short returnCode,string message)
        {
            errorTxt.text = "Room Creation Failed : " + message;
            MenuManager.Instance.OpenMenu("Error Menu");
        }

        public void LeaveRoom()
        {
            PhotonNetwork.LeaveRoom();
            MenuManager.Instance.OpenMenu("Loading");
        }

        public void JoinRoom(RoomInfo info)
        {
            PhotonNetwork.JoinRoom(info.Name);
            MenuManager.Instance.OpenMenu("Loading");
        }

        public override void OnLeftRoom()
        {
            MenuManager.Instance.OpenMenu("Title");
        }

        public void StartGame()
        {
            PhotonNetwork.LoadLevel(1);
        }

        public override void OnRoomListUpdate(List<RoomInfo> roomList)
        {
            // photon doesnt remove rooms from the list . just sets bool
            foreach (Transform t in roomListContent)
            {
                Destroy(t.gameObject);
            }
            
            foreach (var room in roomList)
            {
                if(room.RemovedFromList)
                    continue;
                Instantiate(roomListItemPrefab,roomListContent).GetComponent<RoomListItem>().SetUp(room);
            }
        }

        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            Instantiate(playerListItemPrefab,playerListContent).GetComponent<PlayerListItem>().SetUp(newPlayer);
        }
    }
}