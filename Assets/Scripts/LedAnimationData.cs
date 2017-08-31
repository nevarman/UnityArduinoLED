using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace ArduinoUnity
{
    [CreateAssetMenu(fileName = "Data", menuName = "Arduino/Serial Data", order = 1)]
    [System.Serializable]
    public class LedAnimationData : ScriptableObject
    {
        public string serialStartData;
        public string serialData;
        public bool sendContinuously;

        public string GetArduinoStream()
        {
            StringBuilder str = new StringBuilder();
            
            str.Append(serialStartData);
            str.Append(",");
            str.Append(serialData);

            return str.ToString();
        }
    }
}
