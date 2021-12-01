using UnityEngine;

namespace Com.Tereshchuk.Shooter.NewWeapon_Inventory_System
{
    public interface IInventoryItem
    {
        public void UpdateItem();
        public void UseItem();
        public bool GetActiveState();
        public void Initialize(Transform parent);
        public void SetUI(Transform UI);
        public string GetName();

    }
}