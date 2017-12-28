using System;
using Plugin.FilePicker;
using Prism.Commands;
using Prism.Navigation;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.IO.Compression;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Xml.Serialization;
using JManReader.Helpers;
using JManReader.Model;
using PCLStorage;
using Prism.Services;
using SpeakIt;
using Xamarin.Forms;
using FileAccess = PCLStorage.FileAccess;

namespace JManReader.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        public Dictionary<string, string> ContentMetaData { get; set; } = new Dictionary<string, string>();
        private package _content;
        private ncx _tableOfContents;
        private container _container;
       
        private IFolder _epupTmp;
        internal enum ReadState { None=0,Reading=1}

        private readonly int _storedChapterDescriptions;
        private readonly int _storedParagraph;
       
       

        private ISpeakThis _tts;
        private IDeviceService _deviceService;

       
        public MainPageViewModel(INavigationService navigationService, ISpeakThis tts, IDeviceService deviceService)
            : base(navigationService)
        {
            _storedChapterDescriptions = Settings.CurStoreChapterDescriptions;
            _storedParagraph = Settings.CurStoredParagraph;
            _tts = tts;
            _deviceService = deviceService;
            _tts.InitSpeech();
            if (_storedChapterDescriptions >= 0 && _storedParagraph >= 0)
            {
                ReadLastRead();
            }
            Title = "Main Page1";
        }

         void ReadLastRead()
         {
             Task.Run(async () =>
             {
                 if (await FileSystem.Current.LocalStorage.CheckExistsAsync("EPubTmp") == ExistenceCheckResult.FolderExists)
                 {
                     _epupTmp = await FileSystem.Current.LocalStorage.GetFolderAsync("EPubTmp");
                     if (_epupTmp != null)
                     {
                         await LoadEpubFile();
                         CurChapterDescriptions = _storedChapterDescriptions;

                         await ReadChapter(ChapterDescriptions[CurChapterDescriptions].Filename);
                         await Task.Delay(100);
                         CurParagraph = Paragraphs.Count <= _storedParagraph ? Paragraphs.Count - 1 : _storedParagraph;
                         ;
                         SelParagraph = Paragraphs[CurParagraph];
                         DoRead();
                     }
                 }
             });
         }

        private void TtsOnSpeakCompleded()
        {
            if(CurReadState == ReadState.Reading && HasNextParagraph)
            {

                _tts.Speak(Paragraphs[++CurParagraph].ParagraphText);
                SelParagraph = Paragraphs[CurParagraph];
            }
        }

        private ObservableCollection<Paragraph> _paragraphrs = new ObservableCollection<Paragraph> { new Paragraph { ParagraphText = "Text,Text" } };
        public ObservableCollection<Paragraph> Paragraphs
        {
            get { return _paragraphrs; }
            set
            {
                _deviceService.BeginInvokeOnMainThread(() => SetProperty(ref _paragraphrs, value));
                CurParagraph = -1;
            }
        }

        private ReadState _readState = ReadState.None;
        internal ReadState CurReadState
        {
            get => _readState;
            set => SetProperty(ref _readState, value);
        }

        private List<ChapterDescription> _chapterDescriptions = new List<ChapterDescription>();
        private List<ChapterDescription> ChapterDescriptions
        {
            get { return _chapterDescriptions; }
            set
            {
                SetProperty(ref _chapterDescriptions, value);
                CurChapterDescriptions = 0;
            }
        }

        private int _curParagraph;
        public int CurParagraph
        {
            get => _curParagraph;
            set
            {
                SetProperty(ref _curParagraph, value);
                Settings.CurStoredParagraph = _curParagraph;
            }
        }


        private Paragraph _selParagraph;
        public Paragraph SelParagraph
        {
            get => _selParagraph;
            set
            {
                _deviceService.BeginInvokeOnMainThread(() =>
                {
                    if (_selParagraph != null)
                    {
                        _selParagraph.IsSelected = false;
                    }

                    SetProperty(ref _selParagraph, value);
                    if (_selParagraph != null)
                    {
                        _selParagraph.IsSelected = true;
                    }
                });
            }
        }

        private bool _isLoaded;

        public bool IsLoaded
        {
            get => _isLoaded;
            set => SetProperty(ref _isLoaded, value);
        }

        public bool HasNextParagraph => CurParagraph + 1 < Paragraphs.Count;

        
       
        
        private int _curChapterDescriptions = -1;
        public int CurChapterDescriptions
        {
            get => _curChapterDescriptions;
            set
            {
                SetProperty(ref _curChapterDescriptions, value);
                Settings.CurStoreChapterDescriptions = _curChapterDescriptions;
            }
        }

        private Uri _webViewContent;

        public Uri WebViewContent
        {
            get => _webViewContent;
            set
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    SetProperty(ref _webViewContent, value);
                    
                });
            }
        }

        private DelegateCommand _cmdForward;

        public DelegateCommand CmdForward
        {
            get
            {
                return _cmdForward ?? (_cmdForward = new DelegateCommand(
                           async () =>
                           {
                               CurChapterDescriptions++;
                               if (CurChapterDescriptions >= ChapterDescriptions.Count)
                               {
                                   CurChapterDescriptions--;
                               }

                               await ReadChapter(ChapterDescriptions[CurChapterDescriptions].Filename);

                           },
                 () => CurChapterDescriptions + 1 < ChapterDescriptions.Count)).ObservesProperty( () => CurChapterDescriptions);
            }
        }

        private DelegateCommand _cmdBack;

        public DelegateCommand CmdBack
        {
            get
            {
                return _cmdBack ?? (_cmdBack = new DelegateCommand(
                           async () =>
                           {
                               CurChapterDescriptions--;
                               if (CurChapterDescriptions < 0)
                               {
                                   CurChapterDescriptions=0;
                               }

                               await ReadChapter(ChapterDescriptions[CurChapterDescriptions].Filename);

                           },
                           () => CurChapterDescriptions-1>=0)).ObservesProperty(() => CurChapterDescriptions); 
            }
        }

        private DelegateCommand _cmdRead;

        public DelegateCommand CmdRead
        {
            get => _cmdRead ?? (_cmdRead = new DelegateCommand(DoRead,() => IsLoaded&&CurReadState==ReadState.None)).ObservesProperty(() => CurReadState).ObservesProperty(()=>IsLoaded);
        }

        private void DoRead()
        {
            if (HasNextParagraph)
            {
                _deviceService.BeginInvokeOnMainThread(() =>
                {
                    _tts.SpeakCompleded += TtsOnSpeakCompleded;
                    
                    CurReadState = ReadState.Reading;
                    _tts.Speak(Paragraphs[++CurParagraph].ParagraphText);
                    SelParagraph = Paragraphs[CurParagraph];
                });
            }
        }

        private DelegateCommand _cmdStop;
        public DelegateCommand CmdStop
        {
            get
            {
                return _cmdStop ?? (_cmdStop = new DelegateCommand(DoStopRead,() => CurReadState==ReadState.Reading)).ObservesProperty(()=> CurReadState);
            }
        }

        private void DoStopRead()
        {
            _deviceService.BeginInvokeOnMainThread(() =>
            {
                CurReadState = ReadState.None;
                _tts.Stop();
                _tts.SpeakCompleded -= TtsOnSpeakCompleded;
                CurParagraph--;
            });
        }

        DelegateCommand _cmdOpen;
        public DelegateCommand CmdOpen
        {
            get
            {
                return _cmdOpen?? (_cmdOpen=new DelegateCommand(async () =>
                {
                    
                    try
                    {
                        if (!await SelectFileAndExtractToTmpFolder())
                        {
                            return;
                        }
                        await LoadEpubFile();
                        
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                }));
            }
        }

        private async Task LoadEpubFile()
        {
            IsLoaded = false;
            var cf = PortablePath.Combine(new string[] { _epupTmp.Path, "META-INF", "container.xml" });
           

            _container = await GetData<container>(cf);

            if (_container != null)
            {
                cf = PortablePath.Combine(new string[] {_epupTmp.Path, _container.rootfiles.rootfile.fullpath.Replace('/', PortablePath.DirectorySeparatorChar)});
                var workDir = Path.GetDirectoryName(cf);
                _content = await GetData<package>(cf);

                if (_content != null)
                {
                    GetContentMetaData(_content);
                    _tableOfContents = await GetData<ncx>(PortablePath.Combine(new string[] {workDir, "toc.ncx"}));
                    if (_tableOfContents?.navMap != null)
                    {
                        foreach (var navPoint in _tableOfContents?.navMap)
                        {
                            Debug.WriteLine(navPoint.navLabel.text);
                            if (navPoint.navPoint != null)
                            {
                                foreach (var navPoint1 in navPoint.navPoint)
                                {
                                    Debug.WriteLine("\t " + navPoint1.navLabel.text);
                                }
                            }
                        }
                    }

                    var order = 0;
                    var chd = new List<ChapterDescription>();

                    foreach (var itmRef in _content.spine.itemref)
                    {
                        var fname = Path.Combine(workDir, _content.manifest.FirstOrDefault(p => p.id == itmRef.idref).href.Replace('/', PortablePath.DirectorySeparatorChar));
                        var chapterDescription = new ChapterDescription
                        {
                            Order = ++order,
                            Filename = fname,
                            Title = await GetFirstValueFromTagInHtmlFile(fname, "title")
                        };
                        chd.Add(chapterDescription);
                        //Debug.WriteLine(chapterDescription.Title);
                    }

                    ChapterDescriptions = chd;
                    //CurChapterDescriptions = 0;
                    await ReadChapter(ChapterDescriptions.FirstOrDefault(p => !string.IsNullOrEmpty(p.Title)).Filename);
                    IsLoaded = true;
                }
            }
        }


        private async Task<bool> SelectFileAndExtractToTmpFolder()
        {
            try
            {
                var fd = await CrossFilePicker.Current.PickFile();
                if(fd!=null)
                {

                    using (var zip = new ZipArchive(new MemoryStream(fd.DataArray)))
                    {
                        foreach (var entry in zip.Entries)
                        {
                            Debug.WriteLine(entry.FullName);

                        }

                        Settings.CurStoredReadFile = fd.FilePath;
                        await DeleteExistinfTempFolder();
                        await CreateTempFolder(zip);
                        return true;
                    }
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
            return false;
        }


       
        private async Task CreateTempFolder(ZipArchive zip)
        {
            _epupTmp = await FileSystem.Current.LocalStorage.CreateFolderAsync("EPubTmp", CreationCollisionOption.ReplaceExisting);
            Debug.WriteLine($"Epub -Tmp Verzeichnis neu erzeugt unter: {_epupTmp.Path}");
            zip.ExtractToDirectory(_epupTmp.Path);
        }

        private async Task DeleteExistinfTempFolder()
        {
            if ((await FileSystem.Current.LocalStorage.CheckExistsAsync("EPubTmp")) == ExistenceCheckResult.FolderExists)
            {
                _epupTmp = await FileSystem.Current.LocalStorage.GetFolderAsync("EPubTmp");
                Debug.WriteLine("Epub -Tmp Verzeichnis wird gelöscht.");
                await _epupTmp.DeleteAsync();
                //await DeleteDir(_epupTmp);
                Debug.WriteLine("Epub -Tmp Verzeichnis gelöscht.");
            }
        }

        private async Task ReadChapter(string chapterName)
        {
            
            Debug.WriteLine(chapterName);
            var httpfile = await FileSystem.Current.GetFileFromPathAsync(chapterName);
            Debug.WriteLine($"httpFile found.");

            if (httpfile != null)
            {
                //var serhtml = new XmlSerializer(typeof(html));
                var st = await httpfile.OpenAsync(PCLStorage.FileAccess.Read);
                var ms = new MemoryStream();
                st.CopyTo(ms);
                ms.Seek(0, SeekOrigin.Begin);
                //Debug.WriteLine(System.Net.WebUtility.HtmlDecode(Encoding.UTF8.GetString(ms.ToArray())));
                var tx= GetFirstTagFromHtml(Encoding.UTF8.GetString(ms.ToArray()), "body");
                var text = HttpUtility.HtmlDecode(Regex.Replace(tx, "<(.|\n)*?>", ""));

                var chapterText = text.Split('\n');
                Debug.WriteLine(text);


                var list = new ObservableCollection<Paragraph>();
                foreach (var s in chapterText)
                {
                    list.Add(new Paragraph { ParagraphText = s.Trim() });
                }

                Paragraphs = list;
            }
        }

   
        private void GetContentMetaData(package epContent)
        {
            ContentMetaData.Clear();
            for (int i = 0; i < epContent.metadata.Items.Length; i++)
            {
                string  elementName = epContent.metadata.ItemsElementName[i].ToString();
                string value;
                switch (epContent.metadata.Items[i])
                {
                    case packageMetadataMeta tg:
                        elementName=$"Meta_{tg.name}";
                        value = tg.content;
                        break;
                    case creator tg:
                        value = tg.Value;
                        break;
                    case date tg:
                        value = tg.Value;
                        break;
                    case identifier tg:
                        value = tg.Value;
                        break;
                    default:
                        value = epContent.metadata.Items[i].ToString();
                        break;
                }

                if (ContentMetaData.ContainsKey(elementName))
                {
                    elementName += "_"+ContentMetaData.Count;
                }
                ContentMetaData.Add(elementName, value);
                Debug.WriteLine($"{elementName} /t {value}");
            }
        }

        private async Task<T> GetData<T>(string filename) where T : class
        {

            try
            {
                var f = await FileSystem.Current.GetFileFromPathAsync(filename);
                if(f!=null)
                {
                    var ser = new XmlSerializer(typeof(T));
                    return (T)ser.Deserialize(await f.OpenAsync(PCLStorage.FileAccess.Read));
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }

            return null;
        }

        private async Task<string> GetFirstValueFromTagInHtmlFile(string fileName, string tag)
        {
            var ret = string.Empty;
            try
            {
                Debug.WriteLine(fileName);
                var f = await FileSystem.Current.GetFileFromPathAsync(fileName);
                var s = await f.ReadAllTextAsync();
                //var sTag = $"<{tag}>";
                var eTag = $"</{tag}>";
                var m = Regex.Match(s, $"<{tag}[^>]*>");
                if(m.Success)
                {
                    var p1 = m.Index;
                    var p2 = s.IndexOf(eTag, StringComparison.InvariantCultureIgnoreCase);
                    if (p1 >= 0 && p2 >= 0)
                    {
                        p1 += m.Length;
                        ret = s.Substring(p1, p2 - p1);
                    }
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                
            }
            return ret;
        }


        private string GetFirstTagFromHtml(string html, string tag)
        {
            var ret = string.Empty;
            try
            {
                
                
                var eTag = $"</{tag}>";
                //<body [^>]*>
                var m = Regex.Match(html, $"<{tag}[^>]*>");
                if(m.Success)
                {
                    var p1 =m.Index;
                    //var p1 = html.IndexOf(sTag, StringComparison.InvariantCultureIgnoreCase);
                    var p2 = html.IndexOf(eTag, StringComparison.InvariantCultureIgnoreCase);
                    if (p1 >= 0 && p2 >= 0)
                    {
                        //p1 += sTag.Length;
                        ret = html.Substring(p1, p2 - p1);
                    }
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);

            }
            return ret;
        }

    }


    class ChapterDescription
    {
        public int Order { get; set; }

        public string Filename { get; set; }
        public string Title { get; set; }
    }
}
