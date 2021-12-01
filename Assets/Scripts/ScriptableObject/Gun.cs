using UnityEngine;

namespace Com.Tereshchuk.Shooter
{
    [CreateAssetMenu(fileName = "New Gun", menuName = "Gun")]
    public class Gun : ItemInfo
    {
        public int damage;
        public int ammo;
        public int clipSize;
        public int fireRate ; //1
        public float bulletSpeed ; // 1000.0f
        public float bulletDrop ; //= 0.0f
        public int burst; // 0 - ordinary / 1 - auto / 2 - burst fire
        public GameObject magazine;
        public TrailRenderer BulletTrailRenderer;
        private int clip; // current clip 
        private int stash; // current ammo

        public void Initialize()
        {
            stash = ammo;
            clip = clipSize;
 
        }

        public bool FireBullet()
        {
            if (clip > 0)
            {
                clip -= 1;
                return true;
            }
            return false;
        }

        public void Reload()
        {
            stash += clip;
            clip = Mathf.Min(clipSize,stash);
            stash -= clip;
        }

        public int GetStash() { return stash; }

        public int GetClip() { return clip; }
    }
}