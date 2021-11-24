using Photon.Pun;
using UnityEngine;

public class CrossTarget : MonoBehaviourPunCallbacks
{
    [SerializeField] private Transform mainCameraTransform;
    private Ray _ray;
    private RaycastHit _hitInfo;
    
    
    void Update()
    {
        if (photonView.IsMine)
        {
            MoveCrossTarget();
        }
    }
    
    public void MoveCrossTarget()
    {
        _ray.origin = mainCameraTransform.position;
        _ray.direction = mainCameraTransform.forward;
        Physics.Raycast(_ray, out _hitInfo);
        if(_hitInfo.point != Vector3.zero)
            transform.position = _hitInfo.point;
    }
}
