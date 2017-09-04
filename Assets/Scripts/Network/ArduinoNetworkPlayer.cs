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

        [Command]
        public void CmdWriteContinuously(string data)
        {
            if (isClient)
                WriteContinuously(data);
            RpcWriteContinuously(data);
        }

        [ClientRpc]
        private void RpcWrite(string data)
        {
            Write(data);
        }

        [ClientRpc]
        private void RpcWriteContinuously(string data)
        {
            WriteContinuously(data);
        }

        private void Write(string data)
        {
            if (!arduinoController) return;
            arduinoController.WriteToArduino(data);
        }

        private void WriteContinuously(string data)
        {
            if (!arduinoController) return;
            arduinoController.WriteToArduinoContinuously(data);
        }
    }
}
