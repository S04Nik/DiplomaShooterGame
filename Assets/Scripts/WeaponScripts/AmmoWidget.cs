using UnityEngine;
using UnityEngine.UI;

public class AmmoWidget : MonoBehaviour
{
    public Text uiAmmo;
    
    public void RefreshAmmo(int clip , int stash)
    {
        uiAmmo.text = clip.ToString("D2") + " / " + stash.ToString("D2");
    }

}
