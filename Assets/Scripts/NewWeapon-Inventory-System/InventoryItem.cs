using Photon.Pun;
using UnityEngine;

namespace Com.Tereshchuk.Shooter.NewWeapon_Inventory_System
{
    public abstract class InventoryItem:MonoBehaviourPunCallbacks,IInventoryItem
    {
        public bool _isHolstered { protected set; get; }
        public int SlotNumber { protected set; get; }
        public bool IsActivated;
        public abstract void UpdateItem();
        public abstract void UseItem();

        public bool GetActiveState()
        {
            return IsActivated;
        }
        public abstract void Initialize(Transform parent);
        public abstract void SetUI(Transform UI);
        public abstract string GetName();
        

    }
}