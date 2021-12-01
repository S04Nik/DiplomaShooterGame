using UnityEngine;

namespace Com.Tereshchuk.Shooter
{
    public class HealthWidget:MonoBehaviour
    {
        [SerializeField] private Transform _uiHealthBar;
        private int maxHealth;

        public void Initialize(int maxH)
        {
        
            maxHealth = maxH;
        }
        public void RefreshHealthBar(int _currentHealth)
        {
            float tmpHealthRatio = (float) _currentHealth / (float) maxHealth;
            _uiHealthBar.localScale = Vector3.Lerp(_uiHealthBar.localScale,new Vector3(tmpHealthRatio,1,1),Time.deltaTime * 4f);
        }
    }
}