using Photon.Pun;
using UnityEngine;

namespace Com.Tereshchuk.Shooter.NewWeapon_Inventory_System
{
    public class GunSoundController:MonoBehaviourPun
    {
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip fireClip;
        [SerializeField] private AudioClip attachMagazineClip;
        [SerializeField] private AudioClip detachMagazineClip;
        private AudioSource _audioSourceRPC;
        private float _maxDistanceFireSound = 30f;
        private float _maxDistanceReloadSound = 10f;
        public void Initialize()
        {
            CreateAudioSource();
        }

        private void CreateAudioSource()
        {
            _audioSourceRPC = gameObject.AddComponent<AudioSource> ();
            _audioSourceRPC.maxDistance = 25f;
            _audioSourceRPC.minDistance = 1f;
            _audioSourceRPC.spatialBlend = 1;
            _audioSourceRPC.reverbZoneMix = 1f;
            _audioSourceRPC.rolloffMode = AudioRolloffMode.Linear;
        }

        [PunRPC]
        public void RpcFire()
        {
            _audioSourceRPC.maxDistance = _maxDistanceFireSound;
            _audioSourceRPC.PlayOneShot(fireClip);
        }
        public void Fire()
        {
            if(photonView.IsMine)
                photonView.RPC(nameof(RpcFire),RpcTarget.All);
        }
        [PunRPC]
        public void RpcAttachMagazine()
        {
            _audioSourceRPC.maxDistance = _maxDistanceReloadSound;
            _audioSourceRPC.PlayOneShot(attachMagazineClip);
        }
        public void AttachMagazine()
        {
            if(photonView.IsMine)
                photonView.RPC(nameof(RpcAttachMagazine),RpcTarget.All);
        }
        [PunRPC]
        public void RpcDetachMagazine()
        {
            _audioSourceRPC.maxDistance = _maxDistanceReloadSound;
            _audioSourceRPC.PlayOneShot(detachMagazineClip);
        }
        public void DetachMagazine()
        {
            if(photonView.IsMine)
                photonView.RPC(nameof(RpcDetachMagazine),RpcTarget.All);
        }


    }
}