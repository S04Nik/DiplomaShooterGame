using Photon.Pun;
using UnityEngine;

namespace Com.Tereshchuk.Shooter
{
    public class HealthController:MonoBehaviourPunCallbacks
    {
        [SerializeField]private int maxHealth;
        private int _currentHealth;
        private HealthWidget _uiHealthWidget;

        private void Awake()
        {
            Debug.Log("@ AWAKE");
            _currentHealth = maxHealth;
            _uiHealthWidget = GameObject.Find("HUD/Health").GetComponent<HealthWidget>();
            _uiHealthWidget.Initialize(maxHealth);
            _uiHealthWidget.RefreshHealthBar(_currentHealth);
            
        }

        public bool DecreaseHealth(int dmg)
        {
            _currentHealth -= dmg;
            if (_currentHealth > 0)
            {
                UpdateHealth();
                return true;
            }
            else
            {
                UpdateHealth();
                return false;
            }
        }
        public void UpdateHealth()
        {
            _uiHealthWidget.RefreshHealthBar(_currentHealth);
        }
    }
}