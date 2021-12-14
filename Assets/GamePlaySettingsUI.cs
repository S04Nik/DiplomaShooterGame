using UnityEngine;
using UnityEngine.UI;

public class GamePlaySettingsUI : MonoBehaviour
{
    [SerializeField] private Text sensitivityTxt; 
    [SerializeField] private Slider sensitivitySlider;
    private int sensitivityDefault = 1;
    public int sensitivityMain = 1;
    
    public void SetSensitivity()
    {
        sensitivityMain = Mathf.RoundToInt(sensitivitySlider.value);
        sensitivityTxt.text = sensitivitySlider.value.ToString("0");
    }

    public void SensitivityApply()
    {
        PlayerPrefs.SetFloat("masterSensitivity",sensitivityMain);
    }

    public void SetDefaultSensitivity()
    {
        sensitivityMain = sensitivityDefault;
        sensitivitySlider.value = sensitivityMain; 
        sensitivityTxt.text = sensitivityMain.ToString("0.0 ");

    }
}
