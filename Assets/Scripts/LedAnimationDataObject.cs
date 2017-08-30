using System.Collections.Generic;
using UnityEngine;

namespace ArduinoUnity
{
    [CreateAssetMenu(fileName = "Data", menuName = "Arduino/Serial Data", order = 1)]
    public class LedAnimationDataObject : ScriptableObject
    {
        public LedAnimationData[] serialDatas;

        public Dictionary<LedAnimationType, LedAnimationData> GetDataAsDictionary()
        {
            var dict = new Dictionary<LedAnimationType, LedAnimationData>();
            for(int i = 0; i<serialDatas.Length; i++)
            {
                dict.Add(serialDatas[i].animationType, serialDatas[i]);
            }
            return dict;
        }
    }

    [System.Serializable]
    public class LedAnimationData
    {
        public LedAnimationType animationType;
        public string[] serialData;
        public bool sendContinuously;
    }
}
