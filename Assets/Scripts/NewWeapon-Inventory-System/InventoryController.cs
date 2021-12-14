using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

namespace Com.Tereshchuk.Shooter.NewWeapon_Inventory_System
{ 
    public class InventoryController:MonoBehaviourPunCallbacks,IControllerInventory
    { 
        public InventoryItem[] _items = new InventoryItem[3];
        [SerializeField] private Transform[] armor;
        public Transform[] weaponSlots;
        public ItemWidjet ItemWidjet;
        public Animator rigController;
        [SerializeField] private Transform leftHand;
        public WeaponAnimationEvents AnimationEvents;
        public bool _isChangingWeapon { private set; get; }
        public bool _isHolstered { private set; get; }
        private readonly int _weaponIndxParam = Animator.StringToHash("WeaponIndex");
        private readonly int _holsterWeaponParam = Animator.StringToHash("Holster_Weapon");
        private readonly int _notSprintingParam = Animator.StringToHash("notSprinting");
        private int _activeWeaponIndex;

        private void Start()
        {
            ItemWidjet = GameObject.Find("Canvas_Widgets").GetComponent<ItemWidjet>();
            
        }

        public Transform GetLeftHand()
        {
            return leftHand;
        }
        
        // [PunRPC]
        // private IEnumerator PlayUnarmed(int indx)
        // {
        //     _isChangingWeapon = true;
        //     
        //
        //         rigController.SetBool(_holsterWeaponParam,false);
        //         rigController.Play("equip_"+weapon.GetName());
        //         // do
        //         // {
        //         //     yield return new WaitForEndOfFrame();
        //         // } while (rigController.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f);
        //         yield return new WaitForSeconds(0.5f);
        //         
        //         _isChangingWeapon = false;
        //         _isHolstered = false;
        //         
        //         StopCoroutine(nameof(ActivateWeapon));
        // }
        
        public void RemoveUsedItem(int indx)
        {
            Debug.Log("%%%%   REMOVE ITEM FROM LIST");
            ItemWidjet.DiactivateUI( _items[indx].ItemInfo);
            _items[indx] = null;
            _activeWeaponIndex = -1;
            rigController.SetInteger(_weaponIndxParam,_activeWeaponIndex);
        }
        public bool Check()
        {
            if (GetActiveWeapon())
            {
                if (GetActiveWeapon().Check() && !_isChangingWeapon)
                {
                    return true;
                }
                else
                    return false;
            }
            else if (!_isChangingWeapon)
                return true;

            return false;
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

                if (weapon && !_isChangingWeapon && !_isHolstered && notSprinting )
                {
                    ItemWidjet.SetActiveWeapon(_activeWeaponIndex,weapon.ItemInfo);
                    weapon.UpdateItem();
                }
                if (Input.GetKeyDown(KeyCode.Alpha1)&& GetWeapon(0) != null)
                {
                    photonView.RPC(nameof(SetActiveWeapon),RpcTarget.All,0);
                }
                if (Input.GetKeyDown(KeyCode.Alpha2)&& GetWeapon(1) != null)
                {
                    photonView.RPC(nameof(SetActiveWeapon),RpcTarget.All,1);
                }
                if (Input.GetKeyDown(KeyCode.Alpha3) && GetWeapon(2) != null)
                {
                    photonView.RPC(nameof(SetActiveWeapon),RpcTarget.All,2);
                }
                if (Input.GetKeyDown(KeyCode.Alpha4) && GetWeapon(3) != null)
                {
                    photonView.RPC(nameof(SetActiveWeapon),RpcTarget.All,3);
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
            Debug.Log("SHOW OTHER MY WEAPON %%%");
            InventoryItem newItem = PhotonView.Find(weaponId).GetComponent<InventoryItem>();
            int weaponSlotIndex = newItem.SlotNumber;
            InventoryItem inventoryItem = newItem;
            inventoryItem.transform.SetParent(weaponSlots[weaponSlotIndex],false);
            inventoryItem.Initialize(transform);
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
            if (!rigController.GetBool("isAiming"))
            {
                int holsterIndex = _activeWeaponIndex;
                int activateIndex = weaponSlotIndex;
                InventoryItem holstered = GetWeapon(holsterIndex);
                InventoryItem activated = GetWeapon(activateIndex);
                if(holstered)  holstered.IsActivated = false;
                if(activated)  activated.IsActivated = true;
                
                if (holsterIndex == activateIndex)
                {
                    holsterIndex = -1; // HolsterWeapon = null
                }
                StartCoroutine(SwitchWeapon(holsterIndex,activateIndex));
            }
          
            
        }
        [PunRPC]
        IEnumerator SwitchWeapon(int holsterIndex, int activateIndex)
        {
            rigController.SetInteger(_weaponIndxParam,activateIndex);
            if (holsterIndex >= 2)
            {
                _items[holsterIndex].gameObject.SetActive(false);
            }
            yield return StartCoroutine(HolsterWeapon(holsterIndex));
            //yield return new WaitForSeconds(1f); 
            // не дожидает окончание holster = false
            yield return StartCoroutine(ActivateWeapon(activateIndex));

            _activeWeaponIndex = activateIndex;
        
            //_ammoWidget.RefreshAmmo(_equipedWeapons[_activeWeaponIndex].loadOut.GetClip(),_equipedWeapons[_activeWeaponIndex].loadOut.GetStash());
            StopCoroutine(nameof(SetActiveWeapon));
            StopCoroutine(nameof(SwitchWeapon));
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
                        yield return new WaitForSeconds(1f);
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
                yield return new WaitForSeconds(0.5f);
                
                _isChangingWeapon = false;
                _isHolstered = false;
                
                StopCoroutine(nameof(ActivateWeapon));
            }
            
        }

        public void Equip(ItemInfo itemInfo,int viewId)
        {
            if (photonView.IsMine)
            {
                if (itemInfo.type != "armor")
                {
                    if (!CheckForDublicates(itemInfo))
                    {
                        _items[itemInfo.slot] = PhotonNetwork.Instantiate(itemInfo.prefab.name,Vector3.zero,Quaternion.identity).GetComponent<InventoryItem>();;
                    }
                    // newWeapon.ChangeCanvasAimingDot += StartChangingCanvasAimindDot;
                    if (photonView.ViewID == viewId)
                    {
                        _items[itemInfo.slot].photonView.RequestOwnership();
                    }
                    ItemWidjet.SetUI(itemInfo,itemInfo.slot);
                    photonView.RPC(nameof(ShowOthersMyWeapon),RpcTarget.AllBuffered, _items[itemInfo.slot].photonView.ViewID,viewId);
                }
                else
                {
                    armor[itemInfo.slot].gameObject.SetActive(true);
                    ItemWidjet.SetUI(itemInfo,-1);
                }

            }
        }

        private bool CheckForDublicates(ItemInfo newWeapon)
        {
            var weapon = GetWeapon(newWeapon.slot);
            if (newWeapon.slot == 2 || newWeapon.slot == 3)
            {
                if (weapon)
                {
                    return true;
                }
            }else if (weapon)
            {
                Destroy(weapon.gameObject);
                return false;
            }

            return false;
        }
    }
}