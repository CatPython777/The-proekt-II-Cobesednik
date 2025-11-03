using System;
using System.Collections.Generic;
using Seagull.Interior_I1.Inspector;
using UnityEngine;

namespace Seagull.Interior_I1.SceneProps {
    [Serializable]
    public class String2Shiftable 
    {
        public string key;
        public Shiftable value;
    }
    
    public class ShiftableObject : MonoBehaviour {
        public List<String2Shiftable> shiftables = new();
        private Dictionary<string, Shiftable> shiftableMap = new();
        
        private void Awake() {
            foreach (var item in shiftables) {
                shiftableMap[item.key] = item.value;
            }
        }

        public void Shift(string id, float rotation01) {
            rotation01 = Mathf.Clamp01(rotation01);
            if (shiftableMap.ContainsKey(id))
                shiftableMap[id].shift = rotation01;
        }
        
        public void Shift(float rotation01) {
            rotation01 = Mathf.Clamp01(rotation01);
            foreach (var rot in shiftableMap.Values) 
                rot.shift = rotation01;
        }
    }
}