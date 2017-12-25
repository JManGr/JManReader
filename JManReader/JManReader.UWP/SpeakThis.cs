using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Media.Core;
using Windows.Media;
using Windows.Media.Playback;
using Windows.Media.SpeechSynthesis;
using Windows.Storage;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using SpeakIt.UWP;
using Xamarin.Forms;

[assembly: Dependency(typeof(SpeakThis))]
namespace SpeakIt.UWP
{
    public class SpeakThis : ISpeakThis
    {
        private string tag = "SpeakIt";

        private SpeechSynthesizer tts = null;
        private Dictionary<string, string> param = null;

        public event DgSpeakCompleded SpeakCompleded;

        //public static MediaElement _mediaElement = new MediaElement();
     


        public bool InitSpeech()
        {
            tts = new SpeechSynthesizer();
            if (tts == null)
            {
                return false;
            }
            foreach (var v in SpeechSynthesizer.AllVoices)
            {
                Debug.WriteLine(v.DisplayName);
            }

            tts.Voice = SpeechSynthesizer.AllVoices.FirstOrDefault((p) => p.DisplayName.Contains("Stef")) ?? SpeechSynthesizer.DefaultVoice;
            //_mediaElement.MediaEnded -= _mediaElement_MediaEnded;
            //_mediaElement.MediaEnded += _mediaElement_MediaEnded;
            //_mediaElement.DefaultPlaybackRate *= 1.4;
           
           
            return true;
        }

        private MediaPlayer _currentPlayer;
        private MediaPlayer CurrentPlayer
        {
            get
            {

                if (_currentPlayer != null) return _currentPlayer;
                _currentPlayer = new MediaPlayer();// { AutoPlay = false, AudioCategory = MediaPlayerAudioCategory.Media };
                
                //_currentPlayer.MediaEnded += CurrentPlayerOnMediaEnded;
                return _currentPlayer;
            }
        }

        private void CurrentPlayerOnMediaEnded(MediaPlayer sender, object args)
        {
            CurrentPlayer.MediaEnded -= CurrentPlayerOnMediaEnded;
          
            _currentPlayer.Dispose();
            _currentPlayer = null;
            SpeakCompleded?.Invoke();
        }



        private List<string> _voiceNames;

        public List<string> VoiceNames => _voiceNames ?? (_voiceNames = GetVoiceNames());

        private List<string> GetVoiceNames()
        {
            var list = new List<string>();
            foreach (var voice in SpeechSynthesizer.AllVoices)
            {
                list.Add(voice.DisplayName);
            }
            list.Sort();
            return list;
        }


        public string CurVoice
        {
            get { return tts.Voice.DisplayName; }
            set
            {
                var v = SpeechSynthesizer.AllVoices.FirstOrDefault((p) => p.DisplayName == value);
                if (v != null)
                {
                    tts.Voice = v;
                }
            }
        }

        //public void _mediaElement_MediaEnded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        //{
        //    SpeakCompleded?.Invoke();

        //}


        //zusätzliche Methode, kann manchmal nützlich sein
        public bool Speak(string text)
        {
            doSpeak(text);
            return true;
        }


        //public async void doSpeak(string text)
        //{
        //    // Make sure to be on the UI Thread.

        //    await _mediaElement.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, new DispatchedHandler(
        //        async () =>
        //        {



        //            var results =  await tts.SynthesizeTextToStreamAsync(text);
        //            _mediaElement.AutoPlay = true;
        //            _mediaElement.SetSource(results, results.ContentType);
        //            _mediaElement.Play();

        //            //ret.Completed += (info, status) =>
        //            //{
        //            //    var results = info.GetResults();
        //            //    //_mediaElement.AutoPlay = true;
        //            //    _mediaElement.SetSource(results, results.ContentType);
        //            //    _mediaElement.Play();
        //            //};
        //            //SpeakCompleded?.Invoke();
        //        }));
        //}

        public async void doSpeak(string text)
        {
            //if (CurrentPlayer == null)
            //{
            //    return; 
            //}
           
            _currentPlayer = new MediaPlayer();
            var ret = await tts.SynthesizeTextToStreamAsync(text);
            var results = MediaSource.CreateFromStream( ret,ret.ContentType ) ;

            _currentPlayer.AutoPlay = true;

            _currentPlayer.Source=results ;
            _currentPlayer.PlaybackSession.PlaybackRate=1.45;
            _currentPlayer.MediaEnded += CurrentPlayerOnMediaEnded;
            //CurrentPlayer.Play();

        }

       


        //public void Stop()
        //{
        //    _mediaElement.Stop();
        //}

        public void Stop()
        {
            _currentPlayer.PlaybackSession.PlaybackRate = 0;
            _currentPlayer.PlaybackSession.Position = TimeSpan.Zero;
        }



        public void OnUtteranceCompleted(string utteranceId)
        {
            SpeakCompleded?.Invoke();
        }


        public async void GetBookText(string path, Action<object, string> action)
        {
            string result = string.Empty;

            try
            {
               
               // _currentPlayer = null;
                var fn = Path.GetFileName(path);
                var pf = Path.GetDirectoryName(path);
                var folder = await StorageFolder.GetFolderFromPathAsync(pf);

                var file = await folder.GetFileAsync(fn);
                if (file != null)
                {
                    result = await FileIO.ReadTextAsync(file);
                    action(this, result);
                    return;
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
            action(this, string.Empty);
        }


    }
}