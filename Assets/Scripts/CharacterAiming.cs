using Cinemachine;
using Com.Tereshchuk.Shooter;
using Photon.Pun;
using UnityEngine;

public class CharacterAiming : MonoBehaviourPunCallbacks
{
    public float turnSpeed = 10f;
    //public float aimDuration = 0.3f;
    public AxisState xAxis;
    public AxisState yAxis;
    [SerializeField]private Transform cameraLookAt;
    private Camera _cameraMain;
    private PlayerController _playerController;
    [SerializeField] private Animator animator;
    private int _isAimingParam = Animator.StringToHash("isAiming");
    private float _sensitivity;

    public Camera GetCameraMain()
    {
        return _cameraMain;
    }
    // !!!!
    void Start()
    {
        _playerController = GetComponent<PlayerController>();
        _cameraMain = _playerController.mainCamera;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        _sensitivity = PlayerPrefs.GetFloat("masterSensitivity");
    }

    
    private void Update()
    {
        if (photonView.IsMine)
        {
            bool isAiming = Input.GetMouseButton(1);
            animator.SetBool(_isAimingParam, isAiming);
        }
    }

    void FixedUpdate()
    {
        if (photonView.IsMine)
        {
            CameraRot();
        }
    }
    
    [PunRPC]
    void CameraRot()
    {
        xAxis.Update(Time.fixedDeltaTime);
        yAxis.Update(Time.fixedDeltaTime);
        float yValue = yAxis.Value * _sensitivity;
        float xValue = xAxis.Value * _sensitivity;
        if (yValue >= 45f)
        {
            yValue = 45f;
        }
        else if (yValue <= -45f)
        {
            yValue = -45f;
        }
        cameraLookAt.eulerAngles = new Vector3(yValue, xValue, 0);

        float cameraRot = _cameraMain.transform.rotation.eulerAngles.y;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, cameraRot, 0),
            turnSpeed * Time.fixedDeltaTime);
    }
}
