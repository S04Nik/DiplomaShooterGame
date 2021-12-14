using Photon.Pun;
using UnityEngine;

public class CrossTarget : MonoBehaviourPunCallbacks
{
    [SerializeField] private Transform mainCameraTransform;
    private Ray _ray;
    private RaycastHit _hitInfo;
    private Animator _animator;
    private static readonly int IsAiming = Animator.StringToHash("isAiming");

    public void Initialize(Animator anim)
    {
        _animator = anim;
    }
    
    void Update()
    {
        if (photonView.IsMine)
        {
            MoveCrossTarget();
        }
    }
    public void MoveCrossTarget()
    {
        if (_animator && _animator.GetBool(IsAiming)&&_animator.GetInteger("WeaponIndex")<2)
        {
            // for shooting weapon
            _ray.origin = mainCameraTransform.position;
            _ray.direction = mainCameraTransform.forward;
            Physics.Raycast(_ray, out _hitInfo);
            if (_hitInfo.point != Vector3.zero)
            {
                transform.position = _hitInfo.point;
            }
        }
        else
        {
            // for throwable weapon
            transform.localPosition = new Vector3( 0,0, 20f);
        }

    }
}
