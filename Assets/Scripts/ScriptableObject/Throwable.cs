using UnityEngine;

namespace Com.Tereshchuk.Shooter
{
    [CreateAssetMenu(fileName = "New Grenade", menuName = "Grenade")]
    public class Throwable:ItemInfo
    {
        public float speed;
        public int damage;
    }
}