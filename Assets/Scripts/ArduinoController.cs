using UnityEngine;
using System.IO.Ports;
using System.Collections;
using System;
using UnityEngine.Networking;

namespace ArduinoUnity
{
    public class ArduinoController : MonoBehaviour
    {
        [SerializeField]
        protected string portName = "COM3"; // changes on MAC check from Arduino Ide

        [SerializeField]
        private bool _listenPrint;

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

            print("Port opened : "+stream.IsOpen);
        }

        void OnDisable()
        {
            if (stream != null && stream.IsOpen)
                stream.Close();
        }

        public void WriteToArduino(string message)
        {
            if (stream == null || !stream.IsOpen ) return;
            stream.WriteLine(message);
            stream.BaseStream.Flush();
            
        }

        private void Update()
        {
            if (stream == null || !stream.IsOpen) return;
            if (_listenPrint && !_isListening)
                StartCoroutine(AsynchronousReadFromArduino(OnStreamReceived, null, 5f));
        }

        private void OnStreamReceived(string obj)
        {
            Debug.Log("Arduino : "+obj);
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
