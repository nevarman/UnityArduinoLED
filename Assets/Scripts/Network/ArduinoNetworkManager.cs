using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
/* <copyright company="">
Copyright (c) 2017 All Rights Reserved
</copyright>
<author>Nevzat Arman</author>*/
namespace Assets.Scripts.Network
{
    public class ArduinoNetworkManager : NetworkManager
    {
        [Space]
        [SerializeField]
        private KeyCode _hudKeyCode = KeyCode.Tab;
        [SerializeField]
        private bool _autoHostOnPc;
        [SerializeField]
        private bool _autoClientOnMobile;


        private void Start()
        {
#if UNITY_STANDALONE || UNITY_EDITOR
            if (_autoHostOnPc) StartHost();
#elif UNITY_IOS || UNITY_ANDROID
            if (_autoClientOnMobile) 
                StartClient();
#endif
        }

        private void Update()
        {
            if (Input.GetKeyDown(_hudKeyCode) || Input.touchCount == 4)
            {
                var hud = GetComponent<NetworkManagerHUD>();
                hud.showGUI = !hud.showGUI;
            }
                
        }
    }
}
