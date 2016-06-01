using System.IO;
using SpeechLib;
using UnityEngine;

namespace Assets.Scripts
{
    public class Voice
    {
        private SpVoice voice = new SpVoice();

        public void CreatePromt(string Text, string FileName)
        {
            voice.Volume = 100;
            voice.Rate = 0;
            SpFileStream fs = new SpFileStream();
            string path = Application.dataPath + "/Resources/Sounds/" + FileName + ".wav";
            fs.Open(path, SpeechStreamFileMode.SSFMCreateForWrite, false);
            voice.AudioOutputStream = fs;
            voice.Speak(Text, SpeechVoiceSpeakFlags.SVSFDefault);
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




