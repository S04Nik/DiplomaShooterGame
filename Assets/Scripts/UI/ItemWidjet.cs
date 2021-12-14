using System;
using Com.Tereshchuk.Shooter;
using UnityEngine;
using UnityEngine.UI;

public class ItemWidjet : MonoBehaviour
{
    [SerializeField] private Image[] weaponList;
    [SerializeField] private Image[] armorList;
   
    [HideInInspector] public AmmoWidget AmmoWidget { private set; get; }

    private void Start()
    {
        AmmoWidget = GameObject.Find("Canvas_Widgets/HUD/AmmoAndArmorBar/WeaponAmmo").GetComponent<AmmoWidget>();
    }

    public void ClearWidjets()
    {
        foreach (var w in weaponList)
        {
            w.sprite = null;
            w.gameObject.SetActive(false);
        }
        foreach (var armor in armorList)
        {
            armor.sprite = null;
            armor.gameObject.SetActive(false);
        }

        DiactivateAmmoWidjet();
    }
    public void SetActiveWeapon(int activeWeapon,ItemInfo info)
   {
       foreach (var w in weaponList)
       {
           w.color = Color.white;
       }
       weaponList[activeWeapon].color = Color.yellow;
       ActivateAmmoWidjet(info);
   }
    public void ActivateAmmoWidjet(ItemInfo info)
    {
       AmmoWidget.gameObject.SetActive(true);

       AmmoWidget.RefreshAmmo(info.GetClip(),info.GetStash());
    }
    public void DiactivateAmmoWidjet()
    {
        AmmoWidget.gameObject.SetActive(false);
    }
   public void SetUI(ItemInfo loadOUt, int activeItem)
    {
        switch (loadOUt.type)
        {
            case "armor":
                armorList[loadOUt.slot].gameObject.SetActive(true);
                armorList[loadOUt.slot].sprite = loadOUt.icon;
                
            break;
            case "weapon":
                // ammo
                weaponList[loadOUt.slot].gameObject.SetActive(true);
                weaponList[loadOUt.slot].sprite = loadOUt.icon;
                SetActiveWeapon(activeItem,loadOUt);
                break;
        }
        
    }
   public void DiactivateUI(ItemInfo loadOUt)
   {
       switch (loadOUt.type)
       {
           case "armor":
               armorList[loadOUt.slot].sprite = null;
               armorList[loadOUt.slot].gameObject.SetActive(false);
               break;
           case "weapon":
               weaponList[loadOUt.slot].sprite = null;
               weaponList[loadOUt.slot].gameObject.SetActive(false);
               break;
       }
        
   }
}
