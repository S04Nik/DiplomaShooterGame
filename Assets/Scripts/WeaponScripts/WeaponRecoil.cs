using Cinemachine;
using Photon.Pun;
using UnityEngine;

namespace Com.Tereshchuk.Shooter
{
    public class WeaponRecoil : MonoBehaviour
    {
        public Vector2[] recoilPattern;
        public float duration;
        private CharacterAiming characterAiming;
        private Animator rigController;
        private float _time;
        private CinemachineImpulseSource _impulseSource;
        private float _verticalRecoil;
        private float _horizontalRecoil;
        private int _recoilPatternIndex;
        private Camera _mainCamera;
        
        public void SetRecoil(CharacterAiming aimingScript , Animator animator)
        {
            characterAiming = aimingScript;
            rigController = animator;
        }
        
        private void Awake()
        {
            _impulseSource = GetComponent<CinemachineImpulseSource>();
        }

        public void SetCamera(Camera cameraMain)
        {
            _mainCamera = cameraMain;
        }

        void Update()
        {
            if (_time > 0)
            {
                characterAiming.yAxis.Value -= ((_verticalRecoil / 10) * Time.deltaTime) / duration;
                characterAiming.xAxis.Value -= ((_horizontalRecoil / 10) * Time.deltaTime) / duration;
                _time -= Time.deltaTime;
            }
        }

        public void Reset()
        {
            _recoilPatternIndex = 0;
        }

        int NextIndex(int indx)
        {
            return (indx + 1) % recoilPattern.Length;
        }

        [PunRPC]
        public void GenerateRecoil(string weaponName)
        {
            _time = duration;

            _impulseSource.GenerateImpulse(_mainCamera.transform.forward);

            _horizontalRecoil = recoilPattern[_recoilPatternIndex].x;
            _verticalRecoil = recoilPattern[_recoilPatternIndex].y;

            _recoilPatternIndex = NextIndex(_recoilPatternIndex);
            rigController.Play("weapon_recoil_" + weaponName, 1, 0.0f);

        }
    }
}