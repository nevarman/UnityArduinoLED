using Assets.Scripts.Network;
using UnityEngine;
/* <copyright company="">
   Copyright (c) 2017 All Rights Reserved
   </copyright>
   <author>Nevzat Arman</author>*/
namespace ArduinoUnity.Examples
{
    public class SetLedColor : MonoBehaviour
    {
        [SerializeField]
        ArduinoNetworkPlayer player;

        public bool ColorWipe
        {
            get;set;
        }

        protected virtual void Start()
        {
            if(player == null)
                player = FindObjectOfType<ArduinoNetworkPlayer>();
        }

        public void SetColor(Color color)
        {
            var r = (int)(color.r * 255);
            var g = (int)(color.g * 255);
            var b = (int)(color.b * 255);

            string data = string.Format("{0},{1},{2},{3}", ColorWipe ? "D" : "A", r.ToString(), g.ToString(), b.ToString());
            player.CmdWrite(data);
        }
    }
}
