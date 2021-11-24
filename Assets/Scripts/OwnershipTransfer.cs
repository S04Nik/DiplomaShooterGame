using Photon.Pun;
using Photon.Realtime;
using UnityEngine;


public class OwnershipTransfer : MonoBehaviourPun,IPunOwnershipCallbacks
{
    private void Awake()
    {
        //  Registrating ownershipCallbacks
        
        PhotonNetwork.AddCallbackTarget(this);
    }

    private void OnDestroy()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    public void OnOwnershipRequest(PhotonView targetView, Player requestingPlayer)
    {
       
        if (targetView != photonView)
            return;
        
        //checks here
        
       // Debug.Log("! OnOwnershipRequest(): Player " + requestingPlayer + " requests ownership of: " + targetView + ".");
        targetView.TransferOwnership(requestingPlayer);
    }

    public void OnOwnershipTransfered(PhotonView targetView, Player previousOwner)
    {
        if (targetView != photonView)
            return;
    }

    public void OnOwnershipTransferFailed(PhotonView targetView, Player senderOfFailedRequest)
    {
        //Debug.Log("Transfer FAILED !!!");
        throw new System.NotImplementedException();
    }
}
