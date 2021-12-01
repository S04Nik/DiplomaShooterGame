using Photon.Pun;

namespace Com.Tereshchuk.Shooter.NewWeapon_Inventory_System
{
    public interface IControllerInventory
    {
        public void Show();
        public void Hide();
        public void Equip(ItemInfo itemInfo,int viewId);

    }
}