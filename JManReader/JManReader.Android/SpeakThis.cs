using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Speech.Tts;
using Android.Views;
using Android.Widget;
using Java.Util;
using SpeakIt.Droid;
using Xamarin.Forms;

[assembly: Dependency(typeof(SpeakThis))]
namespace SpeakIt.Droid
{
    public class SpeakThis:Activity, ISpeakThis, TextToSpeech.IOnInitListener, TextToSpeech.IOnUtteranceCompletedListener
    {
        private string tag = "SpeakIt";
        private TextToSpeech tts = null;
        private Dictionary<string, string> param = null;

        public event DgSpeakCompleded SpeakCompleded;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }
        public bool InitSpeech()
        {

            tts = new TextToSpeech(Forms.Context, this);
            if (tts == null)
            {
                return false;
            }
            param = new Dictionary<string, string>();
            param[TextToSpeech.Engine.KeyParamUtteranceId] = "1";
            tts.SetLanguage(Locale.Germany);
            tts.SetOnUtteranceCompletedListener(this);
            return true;
        }

        private List<string> _voiceNames;

        public List<string> VoiceNames => _voiceNames ?? (_voiceNames = GetVoiceNames());

        private List<string> GetVoiceNames()
        {
            var list= new List<string>();
            foreach (var voice in tts.Voices)
            {
                list.Add(voice.Name);
            }
            list.Sort();
            return list;
        }

        public string CurVoice
        {
            get { return tts.Voice.Name; }
            set
            {
                var v = tts.Voices.FirstOrDefault((p) => p.Name == value);
                if (v != null)
                {
                    tts.SetVoice(v);
                }
            }
        }

        public bool Speak(string text)
        {
            OperationResult ret = tts.Speak(text, QueueMode.Add, param);
            return true;
        }


        public void Stop()
        {
            tts.Stop();
        }

        public void OnInit(OperationResult status)
        {
            
        }

        public void OnUtteranceCompleted(string utteranceId)
        {
            SpeakCompleded?.Invoke();
        }


        public void GetBookText(string path, Action<object, string> action)
        {
            string result = string.Empty;
            if (File.Exists(path))
            {
                var encoding = Encoding.GetEncoding("iso-8859-15");
                result = File.ReadAllText(path,encoding);
                if (result.IndexOf('ü') < 0)
                {
                    encoding = Encoding.UTF8;
                    result = File.ReadAllText(path, encoding);

                }
            }
            action(this, result);
        }

        public  Encoding GetFileEncoding(string srcFile)
        {
            // *** Use Default of Encoding.Default (Ansi CodePage)
            Encoding enc = Encoding.Default;

            // *** Detect byte order mark if any - otherwise assume default
            byte[] buffer = new byte[5];
            using (FileStream file = new FileStream(srcFile, FileMode.Open))
            {
                file.Read(buffer, 0, 5);
            }


            if (buffer[0] == 0xef && buffer[1] == 0xbb && buffer[2] == 0xbf)
                enc = Encoding.UTF8;
            else if (buffer[0] == 0xfe && buffer[1] == 0xff)
                enc = Encoding.Unicode;
            else if (buffer[0] == 0 && buffer[1] == 0 && buffer[2] == 0xfe && buffer[3] == 0xff)
                enc = Encoding.UTF32;
            else if (buffer[0] == 0x2b && buffer[1] == 0x2f && buffer[2] == 0x76)
                enc = Encoding.UTF7;
            else if (buffer[0] == 0xFE && buffer[1] == 0xFF)
                // 1201 unicodeFFFE Unicode (Big-Endian)
                enc = Encoding.GetEncoding(1201);
            else if (buffer[0] == 0xFF && buffer[1] == 0xFE)
                // 1200 utf-16 Unicode
                enc = Encoding.GetEncoding(1200);


            return enc;
        }
    }
}