using System.Collections.Generic;
using UnityEngine;

namespace Com.Tereshchuk.Shooter
{
    [CreateAssetMenu(fileName = "PlayerSettings", menuName = "PlayerSettings")]
    public class PlayerSettings:ScriptableObject
    {
        public Dictionary<string,string> Skins = new Dictionary<string, string>();
        public Dictionary<string,int> Genders = new Dictionary<string, int>();
        public Dictionary<string,int> Hats = new Dictionary<string, int>();
        public Dictionary<string,int> HairCuts = new Dictionary<string, int>();
        public Dictionary<string,int> Beards = new Dictionary<string, int>();
       
        
    }
}