using UnityEngine;
using System.IO.Ports;
using System.Collections;
using System;
using UnityEngine.UI;
using System.IO;
using UnityEngine.Networking;

namespace ArduinoUnity
{
    public class ArduinoController : NetworkBehaviour
    {
        private const string WRITE_LINE_CALLBACK = "DONE";

        [SerializeField]
        protected string portName = "COM3"; // changes on MAC check from Arduino Ide

        [SerializeField]
        private bool _openPortOnStart;

        [SerializeField]
        private bool _logReadLine;

        [SerializeField]
        private GameObject _canvasPort;
        [SerializeField]
        private InputField _inputPort;

        SerialPort stream;
        bool _canSendData = true;
        bool _sendContinuously;
        string _continuousMessage;
        bool _isListening;
        string dataString = null;

        public string PortName
        {
            get { return portName; }
            set { portName = value; }
        }

        void Start()
        {
            _canvasPort.SetActive(isServer);
            if (!isServer)
                enabled = false;
            _inputPort.text = portName;
            if (_openPortOnStart)
                OpenPort();
        }

        void OnDisable()
        {
            if (stream != null && stream.IsOpen)
                stream.Close();
        }

        public void OpenPort()
        {
            stream = new SerialPort(portName, 9600);
            try
            {
                if (!stream.IsOpen)
                    stream.Open();
                stream.ReadTimeout = 1;
                stream.WriteTimeout = 50;

                print("Port opened : " + stream.IsOpen);
                _canvasPort.SetActive(false);
            }
            catch (IOException ex)
            {
                Debug.LogError("Error occured while opening port: " + ex.Message);
                _inputPort.image.color = Color.red;
                _canvasPort.SetActive(true);
            }
        }

        /// <summary>
        /// Writes line to arduino
        /// </summary>
        /// <param name="message">The message.</param>
        public void WriteToArduino(string message)
        {
            _continuousMessage = null;
            _sendContinuously = false;
            WriteLine(message);
        }

        /// <summary>
        /// Writes to arduino continuously.
        /// Call it one time!
        /// </summary>
        /// <param name="message">The message.</param>
        public void WriteToArduinoContinuously(string message)
        {
            _continuousMessage = message;
            _sendContinuously = true;
        }

        /// <summary>
        /// Sets the continuous message.
        /// </summary>
        /// <param name="value">The value.</param>
        public void SetContinuousMessage(string value)
        {
            _continuousMessage = value;
        }

        /// <summary>
        /// Writes the line to Arduino
        /// </summary>
        /// <param name="message">The message.</param>
        private void WriteLine(string message)
        {
            if (stream == null || !stream.IsOpen) return;
            stream.WriteLine(message);
            stream.BaseStream.Flush();
            _canSendData = false;
        }

        private void Update()
        {
            if (stream == null || !stream.IsOpen) return;
            DispatchContinuous();
            if (!_isListening)
                StartCoroutine(AsynchronousReadFromArduino(OnStreamReceived, null, 5f));
        }

        /// <summary>
        /// Dispatches the continuous message
        /// </summary>
        private void DispatchContinuous()
        {
            if (!_sendContinuously || !_canSendData || string.IsNullOrEmpty(_continuousMessage))
                return;
            WriteLine(_continuousMessage);
        }

        /// <summary>
        /// Called when [stream received].
        /// </summary>
        /// <param name="obj">The object.</param>
        private void OnStreamReceived(string obj)
        {
            if (_logReadLine)
                Debug.Log("Arduino : " + obj);
            if (obj.Equals(WRITE_LINE_CALLBACK))
                _canSendData = true;
        }

        /// <summary>
        /// Asynchronouses the read lines from arduino.
        /// </summary>
        /// <param name="callback">The callback.</param>
        /// <param name="fail">The fail.</param>
        /// <param name="timeout">The timeout.</param>
        /// <returns></returns>
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
