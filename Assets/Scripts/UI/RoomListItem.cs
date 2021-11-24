using Com.Tereshchuk.Shooter;
using Photon.Realtime;
using TMPro;
using UnityEngine;

public class RoomListItem : MonoBehaviour
{
    [SerializeField] private TMP_Text text;
    public RoomInfo _info;
    
    public void SetUp(RoomInfo info)
    {
        _info = info;
        text.text = info.Name;
    }

    public void OnClick()
    {
        LauncherMenu.Instance.JoinRoom(_info);
    }
    
}
