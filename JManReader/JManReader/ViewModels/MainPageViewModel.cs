using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reactive.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Xml.Serialization;
using JManReader.Helpers;
using JManReader.Model;
using JManReader.Model.toc;
using Newtonsoft.Json;
using PCLStorage;
using Plugin.FilePicker;
using Plugin.Sensors;
using Prism.Commands;
using Prism.Navigation;
using Prism.Services;
using SpeakIt;
using Xamarin.Forms;
using FileAccess = PCLStorage.FileAccess;
//using iText.Kernel.Pdf;
//using iText.Kernel.Pdf.Canvas.Parser;
//using iText.Kernel.Pdf.Canvas.Parser.Listener;

namespace JManReader.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        private const string Books = "Books";
        private readonly int _storedChapterDescriptions;
        private readonly int _storedParagraph;
       private IDisposable _acc;


        private IFolder _booksFolder;

        private List<ChapterDescription> _chapterDescriptions = new List<ChapterDescription>();
        private TcpClient _client;
        private container _container;

        private package _content;

        private int _curChapterDescriptions = -1;

        private int _curParagraph;
        private readonly IDeviceService _deviceService;
        private IFolder _epupTmp;

        private bool _isLoaded;

        private MotionReading _lastMotionReading;
        private readonly INavigationService _navigationService;

        private ObservableCollection<Paragraph> _paragraphrs = new ObservableCollection<Paragraph> { new Paragraph { ParagraphText = "" } };

        private ReadState _readState = ReadState.None;


        private Paragraph _selParagraph;

        private readonly string _storedReadFile;
        private ncx _tableOfContents;

        private readonly ISpeakThis _tts;

        private Uri _webViewContent;

        public MainPageViewModel(ISpeakThis tts, IDeviceService deviceService, INavigationService navigationService)
            : base(navigationService)
        {
            var documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            Task.Run(async () =>
            {

                //if (await FileSystem.Current.LocalStorage.CheckExistsAsync(Books) == ExistenceCheckResult.NotFound)
                //{
                //    await FileSystem.Current.LocalStorage.CreateFolderAsync(Books, CreationCollisionOption.FailIfExists);
                //}
                //_booksFolder = await FileSystem.Current.LocalStorage.GetFolderAsync(Books);
                _booksFolder = await FileSystem.Current.GetFolderFromPathAsync(documents);
            });
            _storedReadFile = Settings.CurStoredReadFile;
            _storedChapterDescriptions = Settings.CurStoreChapterDescriptions;
            _storedParagraph = Settings.CurStoredParagraph;
            _tts = tts;
            _deviceService = deviceService;
            _navigationService = navigationService;
            _tts.InitSpeech();
            if (_storedChapterDescriptions >= 0) // && _storedParagraph >= 0)
            {
                ReadLastRead();
            }

            Title = "JMan Reader";
        }

        public Dictionary<string, string> ContentMetaData { get; set; } = new Dictionary<string, string>();

        public ObservableCollection<Paragraph> Paragraphs
        {
            get => _paragraphrs;
            set
            {
                SetProperty(ref _paragraphrs, value);
                CurParagraph = -1;
            }
        }

        internal ReadState CurReadState
        {
            get => _readState;
            set => SetProperty(ref _readState, value);
        }

        private List<ChapterDescription> ChapterDescriptions
        {
            get => _chapterDescriptions;
            set
            {
                SetProperty(ref _chapterDescriptions, value);
                CurChapterDescriptions = 0;
            }
        }

        public int CurParagraph
        {
            get => _curParagraph;
            set
            {
                SetProperty(ref _curParagraph, value);
                Settings.CurStoredParagraph = _curParagraph;
            }
        }

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
                        if (CurParagraph == -1 || !Paragraphs[CurParagraph].Equals(_selParagraph))
                        {
                            var k = Paragraphs.IndexOf(_selParagraph);
                            CurParagraph = k > 0 ? k - 1 : CurParagraph;
                        }
                    }
                });
            }
        }

        public bool IsLoaded
        {
            get => _isLoaded;
            set => SetProperty(ref _isLoaded, value);
        }

        public bool HasNextParagraph => CurParagraph + 1 < Paragraphs.Count;

        public int CurChapterDescriptions
        {
            get => _curChapterDescriptions;
            set
            {
                SetProperty(ref _curChapterDescriptions, value);
                Settings.CurStoreChapterDescriptions = _curChapterDescriptions;
            }
        }

        public Uri WebViewContent
        {
            get => _webViewContent;
            set { Device.BeginInvokeOnMainThread(() => { SetProperty(ref _webViewContent, value); }); }
        }

        private void ReadLastRead()
        {
            Task.Run(async () =>
            {
                if (await FileSystem.Current.LocalStorage.CheckExistsAsync("EPubTmp") == ExistenceCheckResult.FolderExists)
                {
                    _epupTmp = await FileSystem.Current.LocalStorage.GetFolderAsync("EPubTmp");
                    if (_epupTmp != null)
                    {
                        //var f = await FileSystem.Current.GetFileFromPathAsync(_storedReadFile);

                        switch (Path.GetExtension(_storedReadFile).ToLower().Trim('.', ' '))
                        {
                            case "epub":

                                await LoadEpubFileAsync();
                                CurChapterDescriptions = _storedChapterDescriptions;
                                await ReadChapterAsync(ChapterDescriptions[CurChapterDescriptions]);
                                break;
                            case "pdf":
                            case "txt":
                                var f = await FileSystem.Current.GetFileFromPathAsync(Path.Combine(_epupTmp.Path, _storedReadFile));
                                if (f == null)
                                {
                                    _deviceService.BeginInvokeOnMainThread(async () => await Application.Current.MainPage.DisplayAlert("", "File existiert nicht mehr.", "Ok"));
                                    return;
                                }

                                ChapterDescriptions = CreateChapterDescription(f.Path, f.Name, 1);
                                CurChapterDescriptions = _storedChapterDescriptions;
                                await ReadChapterAsync(ChapterDescriptions[CurChapterDescriptions]);
                                IsLoaded = true;
                                break;
                            default:
                                _deviceService.BeginInvokeOnMainThread(async () => await Application.Current.MainPage.DisplayAlert("", "Unbekannter Buchtype", "Ok"));
                                return;
                        }

                        await Task.Delay(100);

                        CurParagraph = Paragraphs.Count <= _storedParagraph ? Paragraphs.Count - 1 : _storedParagraph == 0 ? -1 : _storedParagraph;
                        SelParagraph = CurParagraph > 0 ? Paragraphs[CurParagraph - 1] : null;
                        DoRead();
                    }
                }
            });
        }

        private async void TtsOnSpeakCompleded()
        {
            if (CurReadState == ReadState.Reading)
            {
                if (!HasNextParagraph)
                {
                    if (CmdForward.CanExecute())
                    {
                        await NextChapter();
                    }
                    else
                    {
                        return;
                    }
                }

                _tts.Speak(Paragraphs[++CurParagraph].ParagraphText);

                SelParagraph = Paragraphs[CurParagraph];
            }
        }

        private async Task NextChapter()
        {
            CurChapterDescriptions++;
            if (CurChapterDescriptions >= ChapterDescriptions.Count)
            {
                CurChapterDescriptions--;
            }

            await ReadChapterAsync(ChapterDescriptions[CurChapterDescriptions]);
        }

        private void OnAccelerator(MotionReading motionReading)
        {
            if (_lastMotionReading != null)
            {
                if (Device.RuntimePlatform == Device.iOS)
                {
                    if (Math.Abs(_lastMotionReading.Z - motionReading.Z) > 1.60)
                    {
                        DoStopRead();
                        return;
                    }
                }

                if (Math.Abs(_lastMotionReading.Z - motionReading.Z) > 16.0)
                {
                    DoStopRead();
                    return;
                }

                //Debug.WriteLine("OnAccelerator:====> " + motionReading + "   --->   LastMotionReading: " + _lastMotionReading);
            }

            _lastMotionReading = motionReading;
        }


        private void DoRead()
        {
            if (HasNextParagraph)
            {
                _deviceService.BeginInvokeOnMainThread(() =>
                {
                    try
                    {
                        if (CrossSensors.Accelerometer.IsAvailable)
                        {
                            _acc = CrossSensors.Accelerometer.WhenReadingTaken().Sample(TimeSpan.FromMilliseconds(500)).Subscribe(OnAccelerator);
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }


                    _tts.SpeakCompleded += TtsOnSpeakCompleded;

                    CurReadState = ReadState.Reading;
                    _tts.Speak(Paragraphs[++CurParagraph].ParagraphText);
                    SelParagraph = Paragraphs[CurParagraph];
                });
            }
        }


        private void DoStopRead()
        {
            _acc?.Dispose();
            _acc = null;
            _lastMotionReading = null;
            _deviceService.BeginInvokeOnMainThread(() =>
            {
                CurReadState = ReadState.None;
                _tts.Stop();
                _tts.SpeakCompleded -= TtsOnSpeakCompleded;
                CurParagraph--;
            });
        }

        private async Task<string> DoReceiveAsync()
        {
            var stream = _client.GetStream();
            var buffer = new byte[1024 * 1];
            var ms = new MemoryStream();


            var k = 0;
            do
            {
                k = stream.Read(buffer, 0, buffer.Length);
                await ms.WriteAsync(buffer, 0, k);
                if (!stream.DataAvailable)
                {
                    await Task.Delay(TimeSpan.FromMilliseconds(300));
                }
            } while (stream.DataAvailable);

            var data = Encoding.UTF8.GetString(ms.GetBuffer());
            return data;
        }


        private async Task DoReceiveFileAsync(string filename)
        {
            if (string.IsNullOrEmpty(filename))
            {
                return;
            }

            // IFile f = await _epupTmp.CreateFileAsync(filename, CreationCollisionOption.ReplaceExisting);
            //IFile f = await PCLStorage.FileSystem.Current.LocalStorage.CreateFileAsync(filename, CreationCollisionOption.ReplaceExisting);
            var f = await _booksFolder.CreateFileAsync(filename, CreationCollisionOption.ReplaceExisting);
            await Task.Run(async () =>
            {
                var stream = _client.GetStream();
                try
                {
                    stream.ReadTimeout = 5000;
                    using (var fileStream = await f.OpenAsync(FileAccess.ReadAndWrite))
                    {

                        var buffer = new byte[1024 * 4];
                        long i = 0;
                        do
                        {
                            var k = stream.Read(buffer, 0, buffer.Length);
                            await fileStream.WriteAsync(buffer, 0, k);
                            i += k;
                            Debug.WriteLine($"{k} Bytes read. Total Bytes: {i}");
                            if (!stream.DataAvailable)
                            {
                                await Task.Delay(500);
                            }
                        } while (stream.DataAvailable);

                        await fileStream.FlushAsync();
                        Debug.WriteLine($"{i} Bytes read.");
                    }

                    var c = new Cmds { Cmd = "Disconnect", Parameter = null };
                    var serObj = JsonConvert.SerializeObject(c);
                    var b = Encoding.UTF8.GetBytes(serObj);
                    stream.Write(b, 0, b.Length);
                }
                finally
                {
                    _client.Close();
                }


            });
            switch (Path.GetExtension(filename).ToLower().Trim('.', ' '))
            {
                case "epub":
                    using (var strm1 = await f.OpenAsync(FileAccess.Read))
                    {
                        if (strm1 != null)
                        {
                            await ExtractToTmpFolderAsync(strm1);
                            await LoadEpubFileAsync();
                        }
                    }

                    break;
                case "pdf":
                case "txt":
                    await CopyTxtToTmpFolderAsync(f);
                    break;
            }
        }

        private async Task GetBookListAsync()
        {
            var stream = _client.GetStream();
            var c = new Cmds { Cmd = "GetBookList", Parameter = null };
            var serObj = JsonConvert.SerializeObject(c);
            var b = Encoding.UTF8.GetBytes(serObj);
            stream.Write(b, 0, b.Length);
            var data = await DoReceiveAsync();
            var list = data.Split('|');

            await _navigationService.NavigateAsync("SelectBookFromPc", new NavigationParameters { { "Books", list } });
        }

        private async Task GetBookAsync(string bookName)
        {
            var c = new Cmds { Cmd = "GetBook", Parameter = new Dictionary<string, string> { { "Bookname", bookName } } };
            var serObj = JsonConvert.SerializeObject(c);
            var res = DoReceiveFileAsync(bookName);
            var b = Encoding.UTF8.GetBytes(serObj);
            _client.GetStream().Write(b, 0, b.Length);
            await res;
        }


        private async Task<bool> GetLocalBookAsync(string s)
        {
            try
            {
                var f = await _booksFolder.GetFileAsync(s);
                switch (Path.GetExtension(f.Name).ToLower().Trim('.', ' '))
                {
                    case "epub":
                        await LoadEpubAsync(f);
                        break;
                    case "pdf":
                    case "txt":
                        await CopyTxtToTmpFolderAsync(f);
                        break;
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }

            return false;
        }

        private async Task<bool> LoadEpubAsync(IFile f)
        {
            var ms = new MemoryStream();
            using (var strm = await f.OpenAsync(FileAccess.Read))
            {
                await strm.CopyToAsync(ms);
            }

            var buf = ms.GetBuffer();

            if (await ExtractToTmpFolderAsync(buf))
            {
                await LoadEpubFileAsync();
                return true;
            }

            return false;
        }


        private async Task LoadEpubFileAsync()
        {
            IsLoaded = false;
            var cf = PortablePath.Combine(_epupTmp.Path, "META-INF", "container.xml");


            _container = await GetDataAsync<container>(cf);

            if (_container != null)
            {
                cf = PortablePath.Combine(_epupTmp.Path, _container.rootfiles.rootfile.fullpath.Replace('/', PortablePath.DirectorySeparatorChar));
                var workDir = Path.GetDirectoryName(cf);
                _content = await GetDataAsync<package>(cf);

                if (_content != null)
                {
                    GetContentMetaData(_content);
                    _tableOfContents = await GetDataAsync<ncx>(PortablePath.Combine(workDir, "toc.ncx"));
                    if (_tableOfContents?.navMap != null)
                    {
                        foreach (var navPoint in _tableOfContents?.navMap)
                        {
                            Debug.WriteLine(navPoint.navLabel.text);
                            if (navPoint.navPoint != null)
                            {
                                foreach (var navPoint1 in navPoint.navPoint) Debug.WriteLine("\t " + navPoint1.navLabel.text);
                            }
                        }
                    }

                    var order = 0;
                    var chd = new List<ChapterDescription>();

                    var tocHref = _content.manifest.FirstOrDefault(p => p.href.Contains("toc.xhtml"))?.href;
                    if (!string.IsNullOrEmpty(tocHref))
                    {
                        var p = PortablePath.Combine(workDir, tocHref);
                        var tocWorkdir = Path.GetDirectoryName(p);
                        var toc = await GetDataAsync<TocXhtml>(p);
                        foreach (var li in toc.body.div.ol.li)
                        {
                            Debug.WriteLine(PortablePath.Combine(tocWorkdir, li.a.href.Split(new []{'#'})[0]) + "\t\t-->\t" + li.a.Value);
                            var chapterDescription = new ChapterDescription
                            {
                                Order = ++order,
                                Filename = PortablePath.Combine(tocWorkdir, li.a.href.Split(new[] { '#' })[0]),
                                Title = li.a.Value
                            };
                            chd.Add(chapterDescription);
                        }
                    }
                    else
                    {
                        foreach (var itmRef in _content.spine.itemref)
                        {
                            var fname = Path.Combine(workDir, _content.manifest.FirstOrDefault(p => p.id == itmRef.idref).href.Replace('/', PortablePath.DirectorySeparatorChar));
                            var title = await GetFirstValueFromTagInHtmlFileAsync(fname, "title");
                            if (string.IsNullOrEmpty(title))
                            {
                                title = Path.GetFileNameWithoutExtension(fname);
                            }

                            title = title + " - " + FindNavPointText(itmRef.idref);
                            var chapterDescription = new ChapterDescription
                            {
                                Order = ++order,
                                Filename = fname,
                                Title = title
                            };
                            chd.Add(chapterDescription);
                            //Debug.WriteLine(chapterDescription.Title);
                        }
                    }

                    CurChapterDescriptions = -1;
                    ChapterDescriptions = chd;
                    var c = ChapterDescriptions.FirstOrDefault(p => !string.IsNullOrEmpty(p.Title));
                    CurChapterDescriptions = ChapterDescriptions.FindIndex(p => p.Equals(c));
                    await ReadChapterAsync(ChapterDescriptions[CurChapterDescriptions]);
                    IsLoaded = true;
                }
            }
        }


        private string FindNavPointText(string id)
        {
            if (_tableOfContents?.navMap != null)
            {
                foreach (var navPoint in _tableOfContents.navMap)
                {
                    if (navPoint.id == id || navPoint.content.src.Contains(id))
                    {
                        return navPoint.navLabel.text;
                    }

                    if (navPoint.navPoint != null)
                    {
                        foreach (var navPoint1 in navPoint.navPoint)
                            if (navPoint1.id == id || navPoint1.content.src.Contains(id))
                            {
                                return navPoint1.navLabel.text;
                            }
                    }
                }
            }

            return null;
        }

        private async Task<bool> SelectFileAndExtractToTmpFolderAsync()
        {
            try
            {
                var fd = await CrossFilePicker.Current.PickFile();
                if (fd != null)
                {
                    Settings.CurStoredReadFile = fd.FileName;
                    var ext = Path.GetExtension(fd.FileName).ToLower().Trim('.');
                    switch (ext)
                    {
                        case "txt":
                            return await CopyTxtToTmpFolderAsync(fd.FileName, fd.DataArray);
                        case "epub":
                            var buf = fd.DataArray;
                            await ExtractToTmpFolderAsync(buf);
                            await LoadEpubFileAsync();
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

        private async Task<bool> ExtractToTmpFolderAsync(byte[] buf)
        {
            using (var zip = new ZipArchive(new MemoryStream(buf)))
            {
                foreach (var entry in zip.Entries) Debug.WriteLine(entry.FullName);

                await DeleteExistinfTempFolderAsync();
                await CreateTempFolderAsync(zip);
                return true;
            }
        }

        private async Task<bool> ExtractToTmpFolderAsync(Stream stream)
        {
            using (var zip = new ZipArchive(stream))
            {
                foreach (var entry in zip.Entries) Debug.WriteLine(entry.FullName);

                await DeleteExistinfTempFolderAsync();
                await CreateTempFolderAsync(zip);
                return true;
            }
        }

        private async Task<bool> CopyTxtToTmpFolderAsync(IFile f)
        {
            try
            {
                await DeleteExistinfTempFolderAsync();
                _epupTmp = await FileSystem.Current.LocalStorage.CreateFolderAsync("EPubTmp", CreationCollisionOption.ReplaceExisting);
                Debug.WriteLine($"Epub -Tmp Verzeichnis neu erzeugt unter: {_epupTmp.Path}");
                var destFile = await _epupTmp.CreateFileAsync(f.Name, CreationCollisionOption.ReplaceExisting);
                using (var sourceStream = await f.OpenAsync(FileAccess.Read))
                {
                    using (var destStream = await destFile.OpenAsync(FileAccess.ReadAndWrite))
                    {
                        await sourceStream.CopyToAsync(destStream);
                    }
                }

                await LoadTxtFileAsync(f.Path, destFile);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }

            return true;
        }


        private async Task<bool> CopyTxtToTmpFolderAsync(string filename, byte[] buffer)
        {
            try
            {
                IsLoaded = false;
                await DeleteExistinfTempFolderAsync();
                _epupTmp = await FileSystem.Current.LocalStorage.CreateFolderAsync("EPubTmp", CreationCollisionOption.ReplaceExisting);
                Debug.WriteLine($"Epub -Tmp Verzeichnis neu erzeugt unter: {_epupTmp.Path}");
                var destFile = await _epupTmp.CreateFileAsync(filename, CreationCollisionOption.ReplaceExisting);

                using (var destStream = await destFile.OpenAsync(FileAccess.ReadAndWrite))
                {
                    await destStream.WriteAsync(buffer, 0, buffer.Length);
                }


                await LoadTxtFileAsync(filename, destFile);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }

            return true;
        }

        private async Task LoadTxtFileAsync(string path, IFile fileToLoad)
        {
            ChapterDescriptions = CreateChapterDescription(fileToLoad.Path, fileToLoad.Name, 1);
            await ReadChapterAsync(ChapterDescriptions.FirstOrDefault(p => !string.IsNullOrEmpty(p.Title)));
            Device.BeginInvokeOnMainThread(() => IsLoaded = true);
        }

        private static List<ChapterDescription> CreateChapterDescription(string path, string title, int order)
        {
            var chd = new List<ChapterDescription>();
            var chapterDescription = new ChapterDescription
            {
                Order = order,
                Filename = path,
                Title = title
            };
            chd.Add(chapterDescription);
            return chd;
        }

        private async Task CreateTempFolderAsync(ZipArchive zip)
        {
            _epupTmp = await FileSystem.Current.LocalStorage.CreateFolderAsync("EPubTmp", CreationCollisionOption.ReplaceExisting);
            Debug.WriteLine($"Epub -Tmp Verzeichnis neu erzeugt unter: {_epupTmp.Path}");
            zip.ExtractToDirectory(_epupTmp.Path);
        }

        private async Task DeleteExistinfTempFolderAsync()
        {
            if (await FileSystem.Current.LocalStorage.CheckExistsAsync("EPubTmp") == ExistenceCheckResult.FolderExists)
            {
                _epupTmp = await FileSystem.Current.LocalStorage.GetFolderAsync("EPubTmp");
                Debug.WriteLine("Epub -Tmp Verzeichnis wird gelöscht.");
                await _epupTmp.DeleteAsync();
                //await DeleteDir(_epupTmp);
                Debug.WriteLine("Epub -Tmp Verzeichnis gelöscht.");
            }
        }

        private async Task ReadChapterAsync(ChapterDescription chapter)
        {
            var chapterName = chapter.Filename;
            Debug.WriteLine(chapterName);
            Title = chapter.Title;
            var f = await FileSystem.Current.GetFileFromPathAsync(chapterName);
            Debug.WriteLine($"httpFile found.");

            if (f != null)
            {
                var text = string.Empty;
                var tx = string.Empty;
                var ms = new MemoryStream();
                if (Path.GetExtension(chapterName).ToLower().EndsWith(".txt"))
                {
                    text = GetBookText(chapterName).Replace("\t", " ").Replace("\r", "");
                    //PdfDocument pdf = new PdfDocument(new PdfReader(await f.OpenAsync(FileAccess.Read)));
                    //tx= PdfTextExtractor.GetTextFromPage(pdf.GetPage(1), new LocationTextExtractionStrategy());
                    //pdf.Close();
                }
                else
                {
                    var st = await f.OpenAsync(FileAccess.Read);
                    st.CopyTo(ms);
                    ms.Seek(0, SeekOrigin.Begin);
                    //Debug.WriteLine(System.Net.WebUtility.HtmlDecode(Encoding.UTF8.GetString(ms.ToArray())));
                    tx = GetFirstTagFromHtml(Encoding.UTF8.GetString(ms.ToArray()), "body");


                    if (!string.IsNullOrEmpty(tx))
                    {
                        text = HttpUtility.HtmlDecode(Regex.Replace(tx, "<(.|\n)*?>", "\n")).Replace("\t", ""); //
                    }
                }

                var chapterText = Regex.Split(text, "(?<=[!.\"?«])[ \\t]*[\\r|\\n]"); //text.Split('\n');
                Debug.WriteLine(text);


                var list = new ObservableCollection<Paragraph>();
                foreach (var s in chapterText)
                    if (!string.IsNullOrEmpty(s.Trim()))
                    {
                        list.Add(new Paragraph { ParagraphText = s.Replace("\n", " ").Replace("-", "") + (s.EndsWith("\n") ? "\n" : "") });
                        IsLoaded = true;
                    }

                Paragraphs = list;
            }
        }

        private string GetBookText(string path)
        {
            var result = string.Empty;
            if (File.Exists(path))
            {
                
                var encoding = Encoding.GetEncodings().FirstOrDefault(p => p.Name == "iso-8859-15")?.GetEncoding() ?? Encoding.ASCII;

                result = File.ReadAllText(path, encoding);
                if (result.IndexOf('ü') < 0)
                {
                    encoding = Encoding.UTF8;
                    result = File.ReadAllText(path, encoding);
                }
            }

            return result;
        }


        private void GetContentMetaData(package epContent)
        {
            ContentMetaData.Clear();
            for (var i = 0; i < epContent.metadata.Items.Length; i++)
            {
                var elementName = epContent.metadata.ItemsElementName[i].ToString();
                string value;
                switch (epContent.metadata.Items[i])
                {
                    case packageMetadataMeta tg:
                        elementName = $"Meta_{tg.name}";
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
                    elementName += "_" + ContentMetaData.Count;
                }

                ContentMetaData.Add(elementName, value);
                Debug.WriteLine($"{elementName} /t {value}");
            }
        }

        private async Task<T> GetDataAsync<T>(string filename) where T : class
        {
            try
            {
                var f = await FileSystem.Current.GetFileFromPathAsync(filename);
                if (f != null)
                {
                    var ser = new XmlSerializer(typeof(T));
                    ser.UnknownNode += (sender, args) => { };
                    return (T)ser.Deserialize(await f.OpenAsync(FileAccess.Read));
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }

            return null;
        }

        private async Task<string> GetFirstValueFromTagInHtmlFileAsync(string fileName, string tag)
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
                if (m.Success)
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
                if (m.Success)
                {
                    var p1 = m.Index;
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

        public override void OnNavigatedTo(NavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
            if (parameters.Keys.Contains("SelectedBook"))
            {
                Settings.CurStoredReadFile = parameters["SelectedBook"] as string;
                if (parameters["IsLocalFile"] is bool isLocalFile && isLocalFile)
                {
                    GetLocalBookAsync(Settings.CurStoredReadFile);
                }
                else
                {
                    GetBookAsync(Settings.CurStoredReadFile);
                }
            }
        }

        internal enum ReadState
        {
            None = 0,
            Reading = 1
        }


        private class Cmds
        {
            public string Cmd { get; set; }
            public Dictionary<string, string> Parameter { get; set; }
        }


        #region Commands

        private DelegateCommand _cmdForward;

        public DelegateCommand CmdForward
        {
            get
            {
                return _cmdForward ?? (_cmdForward = new DelegateCommand(
                           async () => { await NextChapter(); },
                           () => CurChapterDescriptions + 1 < ChapterDescriptions.Count)).ObservesProperty(() => CurChapterDescriptions);
            }
        }

        private DelegateCommand _cmdStop;

        public DelegateCommand CmdStop
        {
            get { return _cmdStop ?? (_cmdStop = new DelegateCommand(DoStopRead, () => CurReadState == ReadState.Reading)).ObservesProperty(() => CurReadState); }
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
                                   CurChapterDescriptions = 0;
                               }

                               await ReadChapterAsync(ChapterDescriptions[CurChapterDescriptions]);
                           },
                           () => CurChapterDescriptions - 1 >= 0)).ObservesProperty(() => CurChapterDescriptions);
            }
        }

        private DelegateCommand _cmdRead;

        public DelegateCommand CmdRead => _cmdRead ?? (_cmdRead = new DelegateCommand(DoRead, () => IsLoaded && CurReadState == ReadState.None))
                                          .ObservesProperty(() => CurReadState).ObservesProperty(() => IsLoaded);

        private DelegateCommand _cmdConPc;

        public DelegateCommand CmdConPc
        {
            get
            {
                return _cmdConPc ?? (_cmdConPc = new DelegateCommand(async
                               () =>
                           {
                               var fList = await _booksFolder.GetFilesAsync();
                               foreach (var file in fList) Debug.WriteLine($"{file.Name}");
                               try
                               {
                                   var address = IPAddress.Parse("192.168.3.2");

                                   var port = 5050;
                                   var buffer = new byte[1024 * 10];
                                   var ipe = new IPEndPoint(address, port);
                                   _client = new TcpClient();
                                   _client.Connect(ipe);

                                   var stream = _client.GetStream();

                                   await GetBookListAsync();
                                   //_socket.Disconnect(false);
                               }
                               catch (SocketException e)
                               {
                                   Debug.WriteLine(e);
                                   _client.Close();
                               }
                               catch (Exception e)
                               {
                                   Debug.WriteLine(e);
                                   _client.Close();
                               }
                           },
                           () => true));
            }
        }

        private DelegateCommand _cmdOpen;

        public DelegateCommand CmdOpen
        {
            get
            {
                return _cmdOpen ?? (_cmdOpen = new DelegateCommand(async () =>
                {
                    try
                    {
                        //if (Device.RuntimePlatform == Device.iOS)
                        {
                            var fList = await _booksFolder.GetFilesAsync();
                            await _navigationService.NavigateAsync("SelectBookFromPc", new NavigationParameters { { "FileList", fList } });
                        }
                        //if (!await SelectFileAndExtractToTmpFolder())
                        //{
                        //    return;
                        //}
                        //await LoadEpubFile();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                }));
            }
        }

        private DelegateCommand _cmdOpenFromFolder;

        public DelegateCommand CmdOpenFromFolder
        {
            get
            {
                return _cmdOpenFromFolder ?? (_cmdOpenFromFolder = new DelegateCommand(
                           async () => { await SelectFileAndExtractToTmpFolderAsync(); },
                           () => true));
            }
        }
        #endregion
    }
}