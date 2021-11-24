using Photon.Pun;
using UnityEngine;

namespace Com.Tereshchuk.Shooter
{
    public class ReloadWeapon : MonoBehaviourPunCallbacks
    {
        public Animator rigController;
        public WeaponAnimationEvents AnimationEvents;
        public ActiveWeapon activeWeapon;
        public Transform leftHand;
        
        private bool isReloading;
        private GameObject magazineAtHand;
        private AmmoWidget _ammoWidget;

        public bool IsReloading()
        {
            return isReloading;
        }

        private void Start()
        {
            AnimationEvents.WeaponAnimationEvent.AddListener(OnAnimationEvent);
            _ammoWidget = GameObject.Find("HUD/Ammo").GetComponent<AmmoWidget>();
        }

        public bool GetReloadingState()
        {
            return isReloading;
        }

        void Update()
        {
            if (photonView.IsMine)
            {
                RaycastWeapon weapon = activeWeapon.GetActiveWeapon();
                if (weapon)
                {
                    if (Input.GetKeyDown(KeyCode.R) || weapon.loadOut.GetClip() == 0)
                    {
                        if (!activeWeapon.GetHolsteredState())
                        {
                            isReloading = true;
                            rigController.SetTrigger("Reload_Weapon");
                        }
                    }

                    if (weapon.isFiring)
                    {
                        _ammoWidget.RefreshAmmo(weapon.loadOut.GetClip(), weapon.loadOut.GetStash());
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
            RaycastWeapon weapon = activeWeapon.GetActiveWeapon();
            weapon.PlaySoundDetachingMagazine();
            magazineAtHand = Instantiate(weapon.magazine, leftHand, true);
            weapon.magazine.SetActive(false);

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
            RaycastWeapon weapon = activeWeapon.GetActiveWeapon();
            weapon.magazine.SetActive(true);
            Destroy(magazineAtHand);
            weapon.loadOut.Reload();
            rigController.ResetTrigger("Reload_Weapon");
            weapon.PlaySoundAttachingMagazine();
            _ammoWidget.RefreshAmmo(weapon.loadOut.GetClip(), weapon.loadOut.GetStash());

            // not fully finished anim yet . leave for now
            isReloading = false;
        }
    }
}