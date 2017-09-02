using ArduinoUnity;
using UnityEngine;
using UnityEngine.Networking;
/* <copyright company="">
Copyright (c) 2017 All Rights Reserved
</copyright>
<author>Nevzat Arman</author>*/
namespace Assets.Scripts.Network
{
    public class ArduinoNetworkPlayer : NetworkBehaviour
    {
        [SerializeField]
        private GameObject _canvasLed;

        ArduinoController arduinoController;
        void Start()
        {
            arduinoController = FindObjectOfType<ArduinoController>();
            _canvasLed.SetActive(isLocalPlayer);
        }
        [Command]
        public void CmdWrite(string data)
        {
            if (isClient)
                Write(data);
            RpcWrite(data);
        }
        [ClientRpc]
        private void RpcWrite(string data)
        {
            Write(data);
        }

        private void Write(string data)
        {
            if (!arduinoController) return;
            arduinoController.WriteToArduino(data);
        }

        //[SerializeField]
        //protected string writeStream = "A,255,255,0";
        //private void OnGUI()
        //{
        //    if (!isLocalPlayer) return;
        //    writeStream = GUILayout.TextField(writeStream);
        //    if (GUILayout.Button("Send"))
        //    {
        //        CmdWrite(writeStream);
        //    }
        //}
    }
}
