using UnityEngine;

namespace Com.Tereshchuk.Shooter
{
    [CreateAssetMenu(fileName = "New Gun", menuName = "Gun")]
    public class Gun : ItemInfo
    {
        public int damage;
        public int fireRate ; //1
        public float bulletSpeed ; // 1000.0f
        public float bulletDrop ; //= 0.0f
        public int burst; // 0 - ordinary / 1 - auto / 2 - burst fire
        public GameObject magazine;
        public TrailRenderer BulletTrailRenderer;

    }
}