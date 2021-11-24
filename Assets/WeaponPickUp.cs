using UnityEngine;

namespace Com.Tereshchuk.Shooter
{
    public class WeaponPickUp : MonoBehaviour
    {
        public RaycastWeapon weaponFab;
        // [SerializeField] private Transform InteractIcon;
        // [SerializeField]private Transform iconPosition;
        // [SerializeField] private Camera cameraMain;

        // private void DrawInteractIcon()
        // {
        //     Vector3 newPosition =cameraMain.WorldToScreenPoint(transform.position);
        //     InteractIcon.transform.position = newPosition;
        //     InteractIcon.gameObject.SetActive(true);
        // }
        private void OnTriggerEnter(Collider other)
        {
            Debug.Log("! ONTRIGGERENTER");
            ActiveWeapon activeWeapon = other.gameObject.GetComponent<ActiveWeapon>();

            if (activeWeapon)
            {
                // DrawInteractIcon();
                // if (Input.GetKeyDown(KeyCode.E))
                // {
                Debug.Log(activeWeapon.photonView.Owner);
                activeWeapon.Equip(0, activeWeapon.photonView);
                Destroy(gameObject);
                // }
                // else
                // {
                //     //InteractIcon.gameObject.SetActive(false);
                // }
            }
        }

    }
}