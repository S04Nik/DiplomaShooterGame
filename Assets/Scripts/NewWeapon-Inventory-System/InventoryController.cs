using System.Collections;
using Photon.Pun;
using UnityEngine;

namespace Com.Tereshchuk.Shooter.NewWeapon_Inventory_System
{ 
    public class InventoryController:MonoBehaviourPunCallbacks,IControllerInventory
    { 
        private InventoryItem[] _items = new InventoryItem[2];
        public Transform[] weaponSlots;
        
        public Animator rigController;
        public Transform leftHand;
        public WeaponAnimationEvents AnimationEvents;
        
        
        private bool _isHolstered;
        private bool _isChangingWeapon;
        private readonly int _weaponIndxParam = Animator.StringToHash("WeaponIndex");
        private readonly int _holsterWeaponParam = Animator.StringToHash("Holster_Weapon");
        private readonly int _notSprintingParam = Animator.StringToHash("notSprinting");
        private int _activeWeaponIndex;
        
        public bool GetHolsteredState()
        {
            return _isHolstered;
        }
        public void Show()
        {

        }

        public void Hide()
        {
          
        }

        InventoryItem GetWeapon(int indx)
        {
            if (indx < 0 || indx > _items.Length)
            {
                return null;
            }
            return _items[indx];
        }
        void Update()
        {
            if (photonView.IsMine)
            {
                var weapon = GetWeapon(_activeWeaponIndex);
                bool notSprinting = rigController.GetCurrentAnimatorStateInfo(2).shortNameHash == _notSprintingParam;
            
                if (weapon /*&& !_isHolstered && notSprinting && !_reloadWeapon.GetReloadingState()*/)
                {
                    weapon.UpdateItem();
                }
                if (Input.GetKeyDown(KeyCode.Alpha1))
                {
                    photonView.RPC(nameof(SetActiveWeapon),RpcTarget.All,0);
                }
                if (Input.GetKeyDown(KeyCode.Alpha2))
                {
                    photonView.RPC(nameof(SetActiveWeapon),RpcTarget.All,1);
                }
                if (Input.GetKeyDown(KeyCode.X))
                {
                    photonView.RPC(nameof(ToggleActiveWeapon),RpcTarget.All);
                }
            }
           
        }
        
        public InventoryItem GetActiveWeapon()
        {
            return GetWeapon(_activeWeaponIndex);
        }
        [PunRPC]
        public void ShowOthersMyWeapon(int weaponId,int ownerId)
        {
            InventoryItem newItem = PhotonView.Find(weaponId).GetComponent<InventoryItem>();
            int weaponSlotIndex = newItem.SlotNumber;
            InventoryItem inventoryItem = newItem;
            inventoryItem.transform.SetParent(weaponSlots[weaponSlotIndex],false);
            inventoryItem.Initialize(this.transform);
            _items[weaponSlotIndex] = inventoryItem;
            photonView.RPC(nameof(SetActiveWeapon),RpcTarget.All,inventoryItem.SlotNumber);
        }
 
        [PunRPC]
        void ToggleActiveWeapon()
        {
            bool isHolstered = rigController.GetBool(_holsterWeaponParam);
            if (isHolstered)
            {
                 StartCoroutine(ActivateWeapon(_activeWeaponIndex));
            }
            else
            {
                 StartCoroutine(HolsterWeapon(_activeWeaponIndex));
            }
        }

        [PunRPC]
        void SetActiveWeapon(int weaponSlotIndex)
        {
            int holsterIndex = _activeWeaponIndex;
            int activateIndex = weaponSlotIndex;

            if (holsterIndex == activateIndex)
            {
                holsterIndex = -1; // HolsterWeapon = null
            }
        
            StartCoroutine(SwitchWeapon(holsterIndex,activateIndex));
        }
        [PunRPC]
        IEnumerator SwitchWeapon(int holsterIndex, int activateIndex)
        {
            rigController.SetInteger(_weaponIndxParam,activateIndex);
        
            yield return StartCoroutine(HolsterWeapon(holsterIndex));
            //yield return new WaitForSeconds(1f); 
            // не дожидает окончание holster = false
            yield return StartCoroutine(ActivateWeapon(activateIndex));

            _activeWeaponIndex = activateIndex;
        
            //_ammoWidget.RefreshAmmo(_equipedWeapons[_activeWeaponIndex].loadOut.GetClip(),_equipedWeapons[_activeWeaponIndex].loadOut.GetStash());
            StopCoroutine("SwitchWeapon");
        }
        [PunRPC]
        IEnumerator HolsterWeapon(int indx)
        {
            _isChangingWeapon = true;
            _isHolstered = true;
            var weapon = GetWeapon(indx);
            if (weapon)
            {
                rigController.SetBool(_holsterWeaponParam,true);
                // do
                // {
                // не успевает закончится . срабатывает 1 раз
                        yield return new WaitForSeconds(1);
                // } while (rigController.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f);
                
                _isChangingWeapon = false;
                
                StopCoroutine(nameof(HolsterWeapon));
            }
        }
        [PunRPC]
        private IEnumerator ActivateWeapon(int indx)
    {
        _isChangingWeapon = true;

        var weapon = GetWeapon(indx);
        if (weapon)
        {
            rigController.SetBool(_holsterWeaponParam,false);
            rigController.Play("equip_"+weapon.GetName());
            // do
            // {
            //     yield return new WaitForEndOfFrame();
            // } while (rigController.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f);
            yield return new WaitForSeconds(1);

            _isHolstered = false;
            _isChangingWeapon = false;
            
            StopCoroutine("ActivateWeapon");
        }
        
    }
        [PunRPC]
        // public void ShowOthersMyWeapon(int weaponId,int ownerId)
        // {
        //     //RaycastWeapon newWeapon = Instantiate(loadOut[loadOutIndex].prefab).GetComponent<RaycastWeapon>();
        //     InventoryItem newWeapon = PhotonView.Find(weaponId).GetComponent<InventoryItem>();
        //
        //     //newWeapon.SetCamera(characterAiming._cameraMain);   
        //     int weaponSlotIndex = newWeapon.Initialize();
        //     var weapon = GetWeapon(weaponSlotIndex);
        //     if (weapon)
        //     {
        //         Destroy(weapon.gameObject);//raycastweapon not gameobject
        //     }
        //     weapon = newWeapon;
        //     weapon.recoil.SetRecoil(characterAiming,rigController);
        //
        //     weapon.transform.SetParent(weaponSlots[weaponSlotIndex],false);
        //     _equipedWeapons[weaponSlotIndex] = weapon;
        //
        //     //photonView.RPC("SetActiveWeapon",RpcTarget.All,newWeapon.loadOut.slot);
        //     SetActiveWeapon(newWeapon.loadOut.slot); 
        // }
    public void Equip(ItemInfo itemInfo,int viewId)
    {
        if (photonView.IsMine)
        {
            InventoryItem newWeapon = PhotonNetwork.Instantiate(itemInfo.prefab.name,Vector3.zero,Quaternion.identity).GetComponent<InventoryItem>();
            if (!CheckForExistingWeapon(newWeapon))
            { 
                _items[0] = newWeapon;
            }
            // newWeapon.ChangeCanvasAimingDot += StartChangingCanvasAimindDot;
            if (photonView.ViewID == viewId)
            {
                newWeapon.photonView.RequestOwnership();
            }
            photonView.RPC(nameof(ShowOthersMyWeapon),RpcTarget.AllBuffered,newWeapon.photonView.ViewID,viewId);
        }
    }

        private bool CheckForExistingWeapon(InventoryItem newWeapon)
        {
            for (int i = 0; i < _items.Length; i++)
            {
                if (_items[newWeapon.SlotNumber])
                {
                    Destroy(_items[i].gameObject);
                    _items[i] = newWeapon;
                    return true;
                }
            }
            return false;
        }
    }
}