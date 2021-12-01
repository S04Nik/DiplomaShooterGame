using UnityEngine;

namespace Com.Tereshchuk.Shooter
{
    public class ItemInfo:ScriptableObject
    {
        public string name;
        public GameObject prefab;
        public int slot; // 0 - primary weapon . 1 - secondary
    }
}