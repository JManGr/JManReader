using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpeakIt
{
    public delegate void DgSpeakCompleded();
    public interface ISpeakThis
    {
        event DgSpeakCompleded  SpeakCompleded;
        bool InitSpeech();
        List<string> VoiceNames { get; }
        string CurVoice { get; set; }
        bool Speak(string text);
        void Stop();
        //string GetBookText(string path);
        void GetBookText(string path, Action<object, string> action);


    }

}
