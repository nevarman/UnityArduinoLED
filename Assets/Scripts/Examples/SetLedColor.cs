using System.Collections;
using UnityEngine;
/* <copyright company="">
   Copyright (c) 2017 All Rights Reserved
   </copyright>
   <author>Nevzat Arman</author>*/
namespace ArduinoUnity.Examples
{
    public class SetLedColor : BaseLedAnimation
    {
        public void SetColor(Color color)
        {
            var r = (int)(color.r * 255);
            var g = (int)(color.g * 255);
            var b = (int)(color.b * 255);

            animationData.serialData = string.Format("{0},{1},{2}", r.ToString(), g.ToString(), b.ToString());
            if (!animationData.sendContinuously)
                _arduinoController.SetLedAnimation(animationData);
        }
    }
}
