using System;
using System.Collections.Generic;
using AVFoundation;
using SpeakIt;

namespace JManReader.iOS
{
    public class SpeakThis : ISpeakThis
    {
        public event DgSpeakCompleded SpeakCompleded;

        private const AVAudioSessionCategory Category = AVAudioSessionCategory.PlayAndRecord | AVAudioSessionCategory.Playback ;
        private AVSpeechSynthesizer _speechSynthesizer;
        public bool InitSpeech()
        {
            AVAudioSession.SharedInstance().SetCategory(Category, AVAudioSessionCategoryOptions.DefaultToSpeaker| AVAudioSessionCategoryOptions.MixWithOthers | AVAudioSessionCategoryOptions.AllowBluetooth | AVAudioSessionCategoryOptions.AllowBluetoothA2DP);
            _speechSynthesizer = new AVSpeechSynthesizer();
            _speechSynthesizer.DidFinishSpeechUtterance += SpeechSynthesizerOnDidFinishSpeechUtterance;
            //throw new NotImplementedException();
            return true;
        }

        public List<string> VoiceNames { get; }
        public string CurVoice { get; set; }
        public bool Speak(string text)
        {
           var speechUtterance = new AVSpeechUtterance(text)
            {
                Rate = 0.54f,//AVSpeechUtterance.MaximumSpeechRate / 1.7f,
                Voice = AVSpeechSynthesisVoice.FromLanguage("de-DE"),
                Volume = 1.90f,
                PitchMultiplier = 1.0f
            };
            _speechSynthesizer.SpeakUtterance(speechUtterance);
            
            return true;
        }

        private void SpeechSynthesizerOnDidFinishSpeechUtterance(object sender, AVSpeechSynthesizerUteranceEventArgs avSpeechSynthesizerUteranceEventArgs)
        {
            SpeakCompleded?.Invoke();
        }

        public void Stop()
        {
            _speechSynthesizer.StopSpeaking(AVSpeechBoundary.Immediate);
        }

        public void GetBookText(string path, Action<object, string> action)
        {
            //throw new NotImplementedException();
        }
    }
}