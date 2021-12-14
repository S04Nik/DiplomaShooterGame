using Photon.Pun;
using UnityEngine;

namespace Com.Tereshchuk.Shooter.NewWeapon_Inventory_System
{
    public class GrenadeSoundController:MonoBehaviourPun
    {
        [SerializeField] private AudioClip explosionAudio;
        [SerializeField] private AudioClip grenadePin;
        private AudioSource _audioSourceRPC;
        private float _maxDistanceFireSound = 25f;
        private float _maxDistancePrepationSound = 10f;

        public void Initialize()
        {
            CreateAudioSource();
        }

        private void CreateAudioSource()
        {
            _audioSourceRPC = gameObject.AddComponent<AudioSource> ();
            _audioSourceRPC.maxDistance = 15f;
            _audioSourceRPC.minDistance = 1f;
            _audioSourceRPC.spatialBlend = 1;
            _audioSourceRPC.reverbZoneMix = 1f;
            _audioSourceRPC.rolloffMode = AudioRolloffMode.Linear;
        }
        [PunRPC]
        public void UsePinRpc()
        {
            _audioSourceRPC.maxDistance = _maxDistancePrepationSound;
            _audioSourceRPC.PlayOneShot(grenadePin);
        }
        public void UsePin()
        {
            if(photonView.IsMine)
                photonView.RPC(nameof(UsePinRpc),RpcTarget.All);
            /*audioSource.PlayOneShot(grenadePin);*/
        }
        [PunRPC]
        public void RpcFire()
        {
            _audioSourceRPC.maxDistance = _maxDistanceFireSound;
            _audioSourceRPC.PlayOneShot(explosionAudio);
        }

        public void Fire()
        {
            if(photonView.IsMine)
                photonView.RPC(nameof(RpcFire),RpcTarget.All);
        }
        
    }
}