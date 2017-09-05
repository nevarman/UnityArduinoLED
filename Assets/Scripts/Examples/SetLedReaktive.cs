using Assets.Scripts.Network;
using System.Collections;
using UnityEngine;
/* <copyright company="">
   Copyright (c) 2017 All Rights Reserved
   </copyright>
   <author>Nevzat Arman</author>*/
namespace Assets.Scripts.Examples
{
    public class SetLedReaktive : MonoBehaviour
    {
        [SerializeField]
        private Light _light;

        [SerializeField]
        private ArduinoNetworkPlayer _arduino;

        void OnEnable()
        {
            _arduino.CmdWriteContinuously(GetLightData()); // trigger send conti
        }

        private void OnDisable()
        {
            _arduino.CmdWrite(GetLightData()); // will reset send cont tag
        }

        private void Update()
        {
            _arduino.CmdSetContinuousData(GetLightData());
        }

        private string GetLightData()
        {
            var color = _light.color;
            var r = (int)(color.r * 255);
            var g = (int)(color.g * 255);
            var b = (int)(color.b * 255);

            return string.Format("{0},{1},{2},{3}", "A", r.ToString(), g.ToString(), b.ToString());
        }
    }
}
