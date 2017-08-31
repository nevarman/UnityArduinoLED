using UnityEngine;
using System.IO.Ports;
using System.Collections;
using System;
using UnityEngine.Networking;

namespace ArduinoUnity
{
    public class ArduinoController : NetworkBehaviour
    {
        [SerializeField]
        protected string portName = "COM3"; // changes on MAC check from Arduino Ide

        [SerializeField]
        protected LedAnimationData currentAnimationData;

        [SerializeField]
        private bool listen;

        SerialPort stream;
        bool _isListening;
        string dataString = null;

        void Start()
        {
            stream = new SerialPort(portName, 9600);
            if (!stream.IsOpen)
                stream.Open();
            stream.ReadTimeout = 1;
            stream.WriteTimeout = 50;
            print(stream.IsOpen);
        }

        void OnDisable()
        {
            if (stream.IsOpen)
                stream.Close();
        }

        public void SetLedAnimation(LedAnimationData data)
        {
            currentAnimationData = data;
            if (!currentAnimationData.sendContinuously)
                SendCurrentSerialData();
        }

        void Update()
        {
            if (!stream.IsOpen) return;

            if (listen && !_isListening)
                StartCoroutine(AsynchronousReadFromArduino((string s) => Debug.Log(s),null,5f ));

            if (currentAnimationData == null || !currentAnimationData.sendContinuously) return;
                SendCurrentSerialData();
        }

        private void SendCurrentSerialData()
        {
            WriteToArduino(currentAnimationData.GetArduinoStream());
        }

        public void WriteToArduino(string message)
        {
            if (!stream.IsOpen) return;
            stream.WriteLine(message);
            stream.BaseStream.Flush();
        }
        
        private IEnumerator AsynchronousReadFromArduino(Action<string> callback, Action fail = null, float timeout = float.PositiveInfinity)
        {
            _isListening = true;
            DateTime initialTime = DateTime.Now;
            DateTime nowTime;
            TimeSpan diff = default(TimeSpan);
            do
            {
                try
                {
                    dataString = stream.ReadLine();
                }
                catch (TimeoutException)
                {
                    dataString = null;
                }

                if (!string.IsNullOrEmpty(dataString))
                {
                    callback(dataString);
                    yield return null;
                }
                else
                    yield return new WaitForSeconds(0.05f);

                nowTime = DateTime.Now;
                diff = nowTime - initialTime;

            } while (diff.Milliseconds < timeout);

            if (fail != null)
                fail();
            yield return null;
            _isListening = false;
        }

    } 
}
