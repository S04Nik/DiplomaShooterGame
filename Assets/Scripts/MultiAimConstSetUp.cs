using Com.Tereshchuk.Shooter;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class MultiAimConstSetUp : MonoBehaviourPunCallbacks,IPunObservable
{
    public RigBuilder rigBuilder;
    public MultiAimConstraint macWeapon;
    public MultiAimConstraint macHead;
    public MultiAimConstraint macSpine1;
    public MultiAimConstraint macSpine2;
    public Animator anim;
    [SerializeField] private Animator rigAnim;
    public Transform head;
    public Transform spine1;
    public Transform spine2;


    private PlayerController pC;
    public CrossTarget SourceObject;


    [PunRPC]
    public void InitializeBonesConstraints()
    {
        pC = GetComponent<PlayerController>();

        SourceObject = pC.mainCamera.GetComponentInChildren<CrossTarget>();
        SourceObject.Initialize(rigAnim);

        macWeapon.data.sourceObjects.Clear();
        macHead.data.sourceObjects.Clear();
        macSpine1.data.sourceObjects.Clear();
        macSpine2.data.sourceObjects.Clear();
        // SourceObject = MainCameraTmp.transform.Find("AimAt").transform;

        WeightedTransformArray wTransformArray = new WeightedTransformArray();
        
        wTransformArray.Add(new WeightedTransform(SourceObject.transform,1f));
        
        macWeapon.data.sourceObjects = wTransformArray;
        macHead.data.sourceObjects = wTransformArray;
        macSpine1.data.sourceObjects = wTransformArray;
        macSpine2.data.sourceObjects = wTransformArray;
        
        rigBuilder.Build();
        anim.enabled = true;
    }
    private void Awake()
    {
        if (!photonView.IsMine)
        {
            
            photonView.RPC("InitializeBonesConstraints",RpcTarget.All);
        }
           
    }
    private void Start()
    {
       
        if (photonView.IsMine)
            photonView.RPC("InitializeBonesConstraints",RpcTarget.All);
    }
    
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            //stream.SendNext((Vector3)headTarget.transform.localPosition);
            stream.SendNext((Quaternion)head.transform.rotation);
            stream.SendNext((Quaternion)spine1.transform.rotation);
            stream.SendNext((Quaternion)spine2.transform.rotation);
            if(SourceObject)
                stream.SendNext((Vector3)SourceObject.transform.position);
            
        }
        else
        {
            //this.headTarget.transform.localPosition = (Vector3)stream.ReceiveNext();
            head.transform.rotation = (Quaternion)stream.ReceiveNext();
            spine1.transform.rotation = (Quaternion)stream.ReceiveNext();
            spine2.transform.rotation = (Quaternion)stream.ReceiveNext();
            SourceObject.transform.position = (Vector3) stream.ReceiveNext();
        }
    }
    
    
}
