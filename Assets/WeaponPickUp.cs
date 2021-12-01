using Com.Tereshchuk.Shooter.NewWeapon_Inventory_System;
using UnityEngine;

namespace Com.Tereshchuk.Shooter
{
    public class WeaponPickUp : MonoBehaviour
    {
        public Gun gunScriptableObj;
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
            InventoryController inventoryController = other.gameObject.GetComponent<InventoryController>();

            if (inventoryController)
            {
                inventoryController.Equip(gunScriptableObj,inventoryController.photonView.ViewID);
                Destroy(gameObject);
            }
        }

    }
}