using UnityEngine;
using UnityEngine.UI;

public class AmmoWidget : MonoBehaviour
{
    public Text uiAmmo;
    
    public void RefreshAmmo(int clip , int stash)
    {
        if (clip == 0 && stash == 0)
        {
            Debug.Log("EMPTY AMMO !!!!");
            gameObject.SetActive(false);
        }
        uiAmmo.text = clip.ToString("D1") + " , " + stash.ToString("D1");
    }

}
