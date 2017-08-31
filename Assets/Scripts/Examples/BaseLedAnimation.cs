using UnityEngine;
/* <copyright company="">
   Copyright (c) 2017 All Rights Reserved
   </copyright>
   <author>Nevzat Arman</author>*/
namespace ArduinoUnity.Examples
{
    public abstract class BaseLedAnimation : MonoBehaviour
    {
        [SerializeField]
        protected LedAnimationData animationData;

        protected ArduinoController _arduinoController;

        protected virtual void OnEnable()
        {
            if (_arduinoController == null) _arduinoController = FindObjectOfType<ArduinoController>();
            _arduinoController.SetLedAnimation(animationData);
        }
    }
}
