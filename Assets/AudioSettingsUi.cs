using UnityEngine;
using UnityEngine.UI;

public class AudioSettingsUi : MonoBehaviour
{
    [SerializeField] private Text volumeTxt; 
    [SerializeField] private Slider volumeSlider;
    private float volumeDefault = 1f;
   // public float volume { private set; get; }

    public void SetVolume()
    {
        AudioListener.volume = volumeSlider.value;
        volumeTxt.text = volumeSlider.value.ToString("0.0 ");
    }

    public void VolumeApply()
    {
        PlayerPrefs.SetFloat("masterVolume",AudioListener.volume);
    }

    public void SetDefaultValues()
    {
        AudioListener.volume = volumeDefault;
        volumeSlider.value = volumeDefault; 
        volumeTxt.text = volumeDefault.ToString("0.0 ");
        VolumeApply();
    }
}
