using Com.Tereshchuk.Shooter.NewWeapon_Inventory_System;
using Photon.Pun;
using UnityEngine;

namespace Com.Tereshchuk.Shooter
{
    public class ReloadWeapon : MonoBehaviourPunCallbacks
    {
        private Animator rigController;
        private WeaponAnimationEvents AnimationEvents;
        private Transform leftHand;
        private bool isReloading;
        [SerializeField] private GameObject originalMagazine;
        private GameObject magazineAtHand;
        private AmmoWidget _ammoWidget;
        private Gun loadOut;
        private FirearmItem weaponController;


        public bool IsReloading()
        {
            return isReloading;
        }

        private void Start()
        {
            AnimationEvents.WeaponAnimationEvent.AddListener(OnAnimationEvent);
            _ammoWidget = GameObject.Find("HUD/Ammo").GetComponent<AmmoWidget>();
            loadOut =GetComponent<FirearmItem>().loadOut;
            weaponController = GetComponent<FirearmItem>();
        }

        public bool GetReloadingState()
        {
            return isReloading;
        }

        void Update()
        {
            if (photonView.IsMine)
            {
                if (loadOut)
                {
                    if (Input.GetKeyDown(KeyCode.R) || loadOut.GetClip() == 0)
                    {
                        if (!weaponController._isHolstered)
                        {
                            isReloading = true;
                            rigController.SetTrigger("Reload_Weapon");
                        }
                    }
                }
            }
        }

        void OnAnimationEvent(string eventName)
        {
            switch (eventName)
            {
                case "detaching_magazine":
                    DetachMagazine();
                    break;
                case "droping_magazine":
                    DropMagazine();
                    break;
                case "refilling_magazine":
                    RefillMagazine();
                    break;
                case "attaching_magazine":
                    AttachMagazine();
                    break;
            }
        }

        private void DetachMagazine()
        {
            //  weapon.PlaySoundDetachingMagazine();
           magazineAtHand = Instantiate(originalMagazine, leftHand, true);
           originalMagazine.SetActive(false);

        }

        private void DropMagazine()
        {
            GameObject droppedMagazine = Instantiate(magazineAtHand, magazineAtHand.transform.position,
                magazineAtHand.transform.rotation);
            droppedMagazine.AddComponent<Rigidbody>();
            droppedMagazine.AddComponent<BoxCollider>();
            magazineAtHand.SetActive(false);
        }

        private void RefillMagazine()
        {
            magazineAtHand.SetActive(true);
        }

        private void AttachMagazine()
        {
            originalMagazine.SetActive(true);
             Destroy(magazineAtHand); 
             loadOut.Reload();
             rigController.ResetTrigger("Reload_Weapon");
                //   weapon.PlaySoundAttachingMagazine();
           _ammoWidget.RefreshAmmo(loadOut.GetClip(), loadOut.GetStash());
           isReloading = false;
        }
    }
}