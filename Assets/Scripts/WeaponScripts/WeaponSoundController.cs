using UnityEngine;

namespace Com.Tereshchuk.Shooter
{
    public class WeaponSoundController : MonoBehaviour
    {
        [SerializeField] private AudioSource _audioSource;
        [SerializeField] private ActiveWeapon _activeWeapon;
        
        public void Fire(AudioClip audioClip)
        {
            _audioSource.PlayOneShot(audioClip);
        }

        public void DetachMagazine(AudioClip audioClip)
        {
            _audioSource.PlayOneShot(audioClip);
        }

        public void AttachMagazine(AudioClip audioClip)
        {
            _audioSource.PlayOneShot(audioClip);
        }
    }
}