using System.IO;
using SpeechLib;
using UnityEngine;

namespace Assets.Scripts
{
    public class Voice
    {
        private SpVoice voice = new SpVoice();

        public void CreatePromt()
        {
            voice.Volume = 100;
            voice.Rate = 0;
            SpFileStream fs = new SpFileStream();
            fs.Open(@"e:\elevator.wav", SpeechStreamFileMode.SSFMCreateForWrite, false);
            voice.AudioOutputStream = fs;
            voice.Speak("You are nearby elevator.", SpeechVoiceSpeakFlags.SVSFDefault);
            fs.Close();
        }
        private string loadXMLStandalone(string fileName)
        {

            string path = System.IO.Path.Combine("Resources", fileName);
            path = System.IO.Path.Combine(Application.dataPath, path);
            Debug.Log("Path:  " + path);
            StreamReader streamReader = new StreamReader(path);
            string streamString = streamReader.ReadToEnd();
            Debug.Log("STREAM XML STRING: " + streamString);
            return streamString;
        }
    }
}




