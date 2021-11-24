using Photon.Pun;
using UnityEngine;

namespace Com.Tereshchuk.Shooter
{
    public class WeaponSway : MonoBehaviourPunCallbacks
    {
        #region Variables

        public float intensity;
        public float smooth;
        public bool isMine;
        
        private Quaternion _originRotation;
        
        #endregion


        #region MonoBehaviour CallBacks

        private void Start()
        {
            _originRotation = transform.localRotation;
        }

        private void Update()
        {
            if (!photonView.IsMine)// for testing is i instantiated this ?
            {
                return;
            }
            UpdateSway();
        }

        #endregion


        #region Private Methods

        private void UpdateSway()
        {
            //controls
            float xMouse = Input.GetAxis("Mouse X");
            float yMouse = Input.GetAxis("Mouse Y");

            if (!isMine)
            {
                xMouse = 0;
                yMouse = 0;
            }
            
            // calculate target rotation
            Quaternion xAdjastment = Quaternion.AngleAxis(-intensity * xMouse, Vector3.up);
            Quaternion yAdjastment = Quaternion.AngleAxis(intensity * yMouse, Vector3.right);
            Quaternion targetRotation = _originRotation * xAdjastment * yAdjastment;

            // rotate towards target rotation
            transform.localRotation = Quaternion.Lerp(transform.localRotation, targetRotation, Time.deltaTime * smooth);
        }

        #endregion

    }
}