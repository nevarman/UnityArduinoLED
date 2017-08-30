using UnityEngine;
using System.IO.Ports;
using System.Text;
using System.Collections.Generic;
using System.Collections;
using System;

namespace ArduinoUnity
{
    public class ArduinoController : MonoBehaviour
    {
        [SerializeField]
        protected string portName = "COM3"; // changes on MAC check from Arduino Ide

        [SerializeField]
        protected LedAnimationDataObject ledAnimationDataObject;

        protected LedAnimationData currentAnimationData;

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

        void OnGUI()
        {
            foreach(var data in ledAnimationDataObject.serialDatas)
            {
                if (GUILayout.Button(data.animationType.ToString()))
                {
                    SetLedAnimation(data.animationType);
                }
            }
        }

        public void SetLedAnimation(LedAnimationType anim)
        {
            currentAnimationData = ledAnimationDataObject.GetDataAsDictionary()[anim];
            if (!currentAnimationData.sendContinuously)
                SendCurrentSerialData();
            
        }

        void Update()
        {
            if (!stream.IsOpen) return;

            if (!_isListening)
                StartCoroutine(AsynchronousReadFromArduino((string s) => Debug.Log(s),null,5f ));

            if (currentAnimationData == null || !currentAnimationData.sendContinuously) return;
                SendCurrentSerialData();
        }

        private void SendCurrentSerialData()
        {
            
            StringBuilder str = new StringBuilder();
            for (int i =0; i<currentAnimationData.serialData.Length; i++)
            {
                str.Append(currentAnimationData.serialData[i]);
                if(i < currentAnimationData.serialData.Length-1)
                str.Append(",");
            }
            WriteToArduino(str.ToString());

        }

        public void WriteToArduino(string message)
        {
            stream.WriteLine(message);
            stream.BaseStream.Flush();
        }

        
        public IEnumerator AsynchronousReadFromArduino(Action<string> callback, Action fail = null, float timeout = float.PositiveInfinity)
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
