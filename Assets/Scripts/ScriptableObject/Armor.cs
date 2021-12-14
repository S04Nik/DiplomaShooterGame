using UnityEngine;

namespace Com.Tereshchuk.Shooter
{
    [CreateAssetMenu(fileName = "New Armor", menuName = "Armor")]
    public class Armor:ItemInfo
    {
        public int initialDefense;
        private int realDefense;
    }
}