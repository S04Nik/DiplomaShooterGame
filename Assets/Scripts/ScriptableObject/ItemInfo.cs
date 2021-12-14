using UnityEngine;

namespace Com.Tereshchuk.Shooter
{
    public class ItemInfo:ScriptableObject
    {
        public string name;
        public string type;
        public int ammo;
        public int clipSize;
        protected int clip; // current clip 
        protected int stash; // current ammo
        public GameObject prefab;
        public Sprite icon;
        public int slot; // 0 - primary weapon . 1 - secondary
        public void Initialize()
        {
            stash = ammo-clipSize;
            clip = clipSize;
        }
        public bool Fire()
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