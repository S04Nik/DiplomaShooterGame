using Photon.Pun;
using TMPro;
using UnityEngine;

public class KillListLine : MonoBehaviour
{
    [SerializeField] private TMP_Text name1;
    [SerializeField] private TMP_Text name2;

    public void SetLine(int view1,int view2)
    {
        name1.text = PhotonView.Find(view1).Controller.NickName;
        name2.text = PhotonView.Find(view2).Controller.NickName;
    }

    public void CopyLine(KillListLine line)
    {
        name1.text = line.name1.text;
        name2.text = line.name2.text;
    }

    // public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    // {
    //     if (stream.IsWriting)
    //     {
    //         stream.SendNext(name1.text);
    //         stream.SendNext(name2.text);
    //     }
    //     else
    //     {
    //         name1.text =(string) stream.ReceiveNext();
    //         name2.text =(string) stream.ReceiveNext();
    //     }
    // }
}
