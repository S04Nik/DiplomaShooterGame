using Photon.Pun;
using UnityEngine;

namespace Com.Tereshchuk.Shooter
{
    public class CharacterLocomotion : MonoBehaviourPunCallbacks
    {
        public float stepDown;
        public float airControl;
        public float jumpDamp;
        public float groundSpeed;
        private float jumpHeight = 3;
        private float gravity = 20;
        private CharacterController _characterController;
        private Vector3 _rootMotion;
        private Animator _animator;
        private Vector2 _input;
        private Vector3 _velocity;
        private bool _isJumping;
       // private ActiveWeapon _activeWeapon;
        private ReloadWeapon _reloadWeapon;
        [SerializeField] private Transform cameraLookAt;
        [SerializeField] private Animator rigController;
        private PlayerAudioController _playerAudioController;


        // reduce GC
        private readonly int _isSprintingParam = Animator.StringToHash("isSprinting");
        private readonly int _isJumpingParam = Animator.StringToHash("isJumping");
        private readonly int _inputXParam = Animator.StringToHash("InputX");
        private readonly int _inputYParam = Animator.StringToHash("InputY");
        private readonly int _crouching = Animator.StringToHash("Crouching");


        void Start()
        {

            _animator = GetComponent<Animator>();
            _characterController = GetComponent<CharacterController>();
            //_activeWeapon = GetComponent<ActiveWeapon>();
            _reloadWeapon = GetComponent<ReloadWeapon>();
            _playerAudioController = GetComponent<PlayerAudioController>();
        }


        public void RpcFuncMotions()
        {
            _input.x = Input.GetAxis("Horizontal");
            _input.y = Input.GetAxis("Vertical");

            _animator.SetFloat(_inputXParam, _input.x);
            _animator.SetFloat(_inputYParam, _input.y);

            UpdateIsSprinting();

            if (_animator.GetBool(_crouching))
            {
                if (_input.x != 0 || _input.y != 0)
                    _animator.SetLayerWeight(1, 1);
                else
                {
                    _animator.SetLayerWeight(1, 0);
                }
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                Jump();
            }

            // CROUCHING !!!!
            if (Input.GetKeyDown(KeyCode.LeftControl))
            {
                if (_animator.GetBool(_crouching))
                {
                    AdjustToStandingPose();
                }
                else
                {
                    AdjustToCrouchingPose();
                }
            }
        }

        public void AdjustToCrouchingPose()
        {
            Vector3 oldPosition = cameraLookAt.position;
            cameraLookAt.position = new Vector3(oldPosition.x, oldPosition.y - 0.4f, oldPosition.z);
            groundSpeed /= 2;
            _characterController.center = new Vector3(0, 0.7f, 0);
            _characterController.height = 1.3f;
            _animator.SetBool(_crouching, true);
        }

        public void AdjustToStandingPose()
        {
            Vector3 oldPosition = cameraLookAt.position;
            cameraLookAt.position = new Vector3(oldPosition.x, oldPosition.y + 0.4f, oldPosition.z);
            groundSpeed *= 2;
            _characterController.center = new Vector3(0, 0.915f, 0);
            _characterController.height = 1.6f;
            _animator.SetBool(_crouching, false);
        }

        void Update()
        {
            if (photonView.IsMine)
            {
                RpcFuncMotions();
            }
        }

        private void FixedUpdate()
        {
            if (photonView.IsMine)
            {
                if (_isJumping) // inAir
                {
                    UpdateInAir();
                }
                else // isGrounded
                {
                    UpdateAtGround();
                }
            }

        }

        bool isSprinting()
        {
            bool isSprinting = Input.GetKey(KeyCode.LeftShift) && _input.y > 0;

           // bool isFiring = _activeWeapon.IsFiring();
            bool isReloading = _reloadWeapon.IsReloading();
          //  bool isChangingWeapon = _activeWeapon.IsChangingWeapon();

          //  return isSprinting && !isFiring && !isReloading && !isChangingWeapon;
          return false;
        }

        private void UpdateIsSprinting()
        {
            bool isSprintring = isSprinting();
            if (isSprintring && _animator.GetBool(_crouching))
            {
                AdjustToStandingPose();
            }

            _animator.SetBool(_isSprintingParam, isSprintring);
            rigController.SetBool(_isSprintingParam, isSprintring);

        }

        Vector3 CalculateAirControl()
        {
            return ((transform.forward * _input.y) + (transform.right * _input.x)) * (airControl / 100);
        }

        private void OnAnimatorMove()
        {
            _rootMotion += _animator.deltaPosition;
        }

        void UpdateInAir()
        {
            _velocity.y -= gravity * Time.fixedDeltaTime;
            Vector3 displacement = _velocity * Time.fixedDeltaTime;
            displacement += CalculateAirControl();
            _characterController.Move(displacement);
            _isJumping = !_characterController.isGrounded; // store if character was touching the groung after last call
            if (!_isJumping)
            {
                _playerAudioController.FinishJumping(); // AUDIO JUMP
            }

            _rootMotion = Vector3.zero; // glitching
            _animator.SetBool(_isJumpingParam, _isJumping);

        }

        void UpdateAtGround()
        {
            Vector3 stepForwardAmount = _rootMotion * groundSpeed;
            Vector3 stepDownAmount = Vector3.down * stepDown;
            // Move root Motion portion of animator in Fixed Update
            _characterController.Move(stepForwardAmount + stepDownAmount); // character getting root motion from animator
            _rootMotion = Vector3.zero;

            if (!_characterController.isGrounded)
            {
                SetInAir(0);
            }
        }

        void Jump()
        {
            if (_animator.GetBool(_crouching))
                AdjustToStandingPose();

            _playerAudioController.StartJumping();

            if (!_isJumping)
            {
                float jumpVelocity = Mathf.Sqrt(2 * gravity * jumpHeight);
                SetInAir(jumpVelocity);
            }
        }

        void SetInAir(float jumpVelocity)
        {
            _isJumping = true;
            _velocity = _animator.velocity * jumpDamp * groundSpeed;
            _velocity.y = jumpVelocity;
            _animator.SetBool(_isJumpingParam, true);
        }

    }
}