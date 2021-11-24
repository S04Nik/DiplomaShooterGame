using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance; // SINGLTONE
    //variable bind to the class . not actual obj 
    [SerializeField] private Menu[] menus;

    private void Awake()
    {
        Instance = this;
    }

    public void OpenMenu(string menuName)
    {
        foreach (var m in menus)
        {
            if (  m.menuName == menuName)
            {
              m.Open();
            }else if (m.open)
            {
                CloseMenu(m);
            }
        }
    }

    public void OpenMenu(Menu menu)
    {
        foreach (var m in menus)
        {
            if (m.open)
            {
                CloseMenu(m);
            }
        }
        menu.Open();
    }
    public void CloseMenu(Menu menu)
    {
        menu.Close();
    }
}
