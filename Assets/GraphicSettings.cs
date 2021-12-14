using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GraphicSettings : MonoBehaviour
{
    [SerializeField] private Slider brightnessSlider;
    [SerializeField] private Text brightnessText;
    [SerializeField] private Toggle fullSreen;
    [SerializeField] private Dropdown quality;
    [SerializeField] private Dropdown resolutionDropdown;
    private Resolution[] _resolutions;
    private float _defaultBrightness = 1;
    private float _brightnessLevel;
    private int _qualityLevel;
    private bool _isFullScreen;
    private int defaultResolution;

    private void Start()
    {
        _resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();
        List<string> options = new List<string>();

         defaultResolution = 0;
        for (int i = 0; i < _resolutions.Length; i++)
        {
            string option = _resolutions[i].width + " x " + _resolutions[i].height;
            options.Add(option);

            if (_resolutions[i].width == Screen.width && _resolutions[i].height == Screen.height)
            {
                defaultResolution = i;
            }
        }
        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = defaultResolution;
        resolutionDropdown.RefreshShownValue();
        GraphicToDefault();
    }

    public void SetResolution()
    {
        Resolution resolution = _resolutions[resolutionDropdown.value];
        Screen.SetResolution(resolution.width,resolution.height,_isFullScreen);
    }
    public void GraphicApply()
    {
        PlayerPrefs.SetFloat("masterBrightness",_brightnessLevel);
        PlayerPrefs.SetInt("masterQuality",_qualityLevel);
        QualitySettings.SetQualityLevel(_qualityLevel);
        PlayerPrefs.SetInt("masterFullScreen",(_isFullScreen?1:0));
        Screen.fullScreen = _isFullScreen;
        SetResolution();
    }
    public void GraphicToDefault()
    {
        brightnessSlider.value = 1;
        _brightnessLevel = brightnessSlider.value;
        brightnessText.text = brightnessSlider.value.ToString("0.0");
        _isFullScreen = false;
        fullSreen.isOn = _isFullScreen;
        Screen.fullScreen = _isFullScreen;
        Resolution resolution = _resolutions[defaultResolution];
        Screen.SetResolution(resolution.width,resolution.height,_isFullScreen);
        resolutionDropdown.value = defaultResolution;
        quality.value = 0;
        _qualityLevel = quality.value;
        QualitySettings.SetQualityLevel(_qualityLevel);
    }
    public void SetQuality()
    {
        _qualityLevel = quality.value;
        QualitySettings.SetQualityLevel(_qualityLevel);
    }
    
    public void SetBrightness()
    {
        _brightnessLevel = brightnessSlider.value;
        brightnessText.text = brightnessSlider.value.ToString("0.0");
        //Screen.brightness = _brightnessLevel;
        // RenderSettings.ambientLight = new Color(_brightnessLevel, _brightnessLevel, _brightnessLevel, 1);
    }

    public void SetFullScreen()
    {
        if (fullSreen.isOn)
        {
            _isFullScreen = true;
            SetResolution();
        }
        else
        {
            _isFullScreen = false;
            SetResolution();
        }
    }
   
}
