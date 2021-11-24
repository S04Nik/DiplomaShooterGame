using Photon.Pun;
using UnityEditor;
using UnityEngine;
using UnityEngine.Animations.Rigging;

namespace Com.Tereshchuk.Shooter
{
    public class Look : MonoBehaviourPunCallbacks
    {
        #region Variables
        
        public Transform player;
        public Transform camera;
        public Transform weapon;
        public static bool cursorLock = true;
        public float xSensitivity;
        public float ySensitivity;
        public float maxAngle;
        
        private Quaternion camCenter;
        
        #endregion

        #region MonoBehaviour Callbacks

        void Start()
        {
            //camCenter = camera.localRotation;
        }
        void Update()
        {
            
            if (!photonView.IsMine)
            {
                return;
            }
            // SetY();
            // SetX();
            
            UpdateCursorLock();
        }


        #endregion

        #region Private Methods
        
        void SetY()
        {
            float tInput = Input.GetAxis("Mouse Y") * ySensitivity * Time.deltaTime;
            Quaternion tAdjastment = Quaternion.AngleAxis(tInput, - Vector3.right);
            Quaternion tDelta = camera.localRotation * tAdjastment;

            if (Quaternion.Angle(camCenter, tDelta) < maxAngle)
            {
                camera.localRotation = tDelta;
              //Debug.Log(tmp.position);
            }
            //weapon.rotation = camera.rotation; !!!!!!!!
        }
        void SetX()
        {
            float tInput = Input.GetAxis("Mouse X") * xSensitivity * Time.deltaTime;
            Quaternion tAdjastment = Quaternion.AngleAxis(tInput,Vector3.up);
            Quaternion tDelta = player.localRotation * tAdjastment;
            player.localRotation = tDelta;
        }

        void UpdateCursorLock()
        {
            if (cursorLock)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    cursorLock = false;
                }
            }
            else
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    cursorLock = true;
                }
            }
        }
        
        #endregion


    }
}