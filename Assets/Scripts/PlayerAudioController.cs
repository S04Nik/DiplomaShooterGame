using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using Random = UnityEngine.Random;

public class PlayerAudioController : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private AudioMixerGroup mixerOutPut;
    [SerializeField] private AudioClip[] _audioFootClips;
    [SerializeField] private AudioClip _audioClipStartJumping;
    [SerializeField] private AudioClip[] playerPainVoice;
    private int playerVoiceId;
    private  List<AudioClip> randomClipsList;
    private float InvokeSpeed;
    // [SerializeField] private float pitchMin = 0.95f;
    // [SerializeField] private float pitchMax = 1.05f;
    private float _volumeMin = 0.25f;
    private float _volumeMedium = 0.55f;
    private float _volumeMax = 1.00f;
    private bool _playerIsMoving;
    private AudioSource _audioSource;
    private readonly int _crouching = Animator.StringToHash("Crouching");
    private readonly int _sprinting = Animator.StringToHash("isSprinting");

    public void SetVoice(int g)
    {
        playerVoiceId = g;
    }
    public void PlayPainVoice()
    {
        _audioSource.PlayOneShot(playerPainVoice[playerVoiceId]);
    }

    public void MakeFootStepSound()
    {
        PlayRandomSound();
    }

    public void StartJumping()
    {
        _audioSource.volume = _volumeMax;
        _audioSource.PlayOneShot(_audioClipStartJumping);
    }

    public void FinishJumping()
    {
        _audioSource.volume = _volumeMax;
        _audioSource.PlayOneShot(_audioFootClips[1]);
    }

    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        randomClipsList = new List<AudioClip>(new AudioClip[_audioFootClips.Length]);
        for (int i = 0; i < _audioFootClips.Length; i++)
        {
            randomClipsList[i] = _audioFootClips[i];
        }
    }
    
    public void Reset()
    {
        for (int i = 0; i < _audioFootClips.Length; i++)
        {
            randomClipsList.Add(_audioFootClips[i]);
        }
    }
    
    private void PlayRandomSound()
    {
        //int i = Random.Range(0,randomClipsList.Count);
        // _audioSource.pitch = Random.Range(pitchMin, pitchMax);
        if (_animator.GetBool(_crouching))
            _audioSource.volume = _volumeMin;
        else if (_animator.GetBool(_sprinting))
            _audioSource.volume = _volumeMax;
        else 
            _audioSource.volume = _volumeMedium;
            
            //_audioSource.volume = Random.Range(volumeMin, volumeMax);
        
        _audioSource.PlayOneShot(randomClipsList[0]);
        randomClipsList.RemoveAt(0);
        if (randomClipsList.Count == 0)
        {
            Reset();
        }
    }
}
