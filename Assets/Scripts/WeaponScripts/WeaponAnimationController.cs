using Photon.Pun;
using UnityEngine;

namespace Com.Tereshchuk.Shooter
{
    public class WeaponAnimationController:MonoBehaviourPun
    {
        private Animator rigController;
        public WeaponAnimationEvents AnimationEvents { private set; get; }
        private static readonly int Weapon = Animator.StringToHash("Reload_Weapon");
        private static readonly int Fire = Animator.StringToHash("Fire");
        private static readonly int Unarmed = Animator.StringToHash("Unarmed");
        private static readonly int Aiming = Animator.StringToHash("isAiming");

        public void PlayUnArmed()
        {
            rigController.SetTrigger(Unarmed);
        }
        public void Initialize(WeaponAnimationEvents events)
        {
            AnimationEvents = events;
            rigController = events.riggingAnimator;
        }

        public bool GrenadePrepared()
        {
            if (rigController.GetBool(Aiming) && rigController.GetInteger("WeaponIndex") >= 2)
            {
                return true;
            }
            else
                return false;
        }
        public void PlayFire()
        {
            rigController.SetTrigger(Fire);
        }
        public void PlayRecoil(string animName , int layer,float timeNormal)
        {
            rigController.Play(animName, layer, timeNormal);
        }

        public void PlayReloading()
        {
            rigController.SetTrigger(Weapon);
        }
        public void FinishReloading()
        {
            rigController.ResetTrigger(Weapon);
        }
    }
}