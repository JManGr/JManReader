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
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Xml.Serialization;
using JManReader.Model;
using PCLStorage;
using Xamarin.Forms;
using FileAccess = System.IO.FileAccess;

namespace JManReader.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        public MainPageViewModel(INavigationService navigationService)
            : base(navigationService)
        {
            Title = "Main Page1";
        }

        private ObservableCollection<Chapter> _chapters = new ObservableCollection<Chapter>{new Chapter{ChapterText = "Text,Text"}};

        public ObservableCollection<Chapter> Chapters
        {
            get { return _chapters; }
            set { SetProperty(ref _chapters, value); }
        }

        public Dictionary<string, string> ContentMetaData { get; set; } = new Dictionary<string, string>();
        private package content;
        private container _container;
        DelegateCommand _cmdOpen;
        private IFolder _epupTmp;

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


        public DelegateCommand CmdOpen
        {
            get
            {
                return new DelegateCommand(async () =>
                {
                    try
                    {

                        var fd = await CrossFilePicker.Current.PickFile();
                        {
                            using (var zip = new ZipArchive(new MemoryStream(fd.DataArray)))
                            {
                                foreach (var entry in zip.Entries)
                                {
                                    Debug.WriteLine(entry.FullName);

                                }


                                if ((await FileSystem.Current.LocalStorage.CheckExistsAsync("EPubTmp")) == ExistenceCheckResult.FolderExists)
                                {
                                    _epupTmp = await FileSystem.Current.LocalStorage.GetFolderAsync("EPubTmp");
                                    Debug.WriteLine("Epub -Tmp Verzeichnis wird gelöscht.");
                                    await _epupTmp.DeleteAsync();
                                    //await DeleteDir(_epupTmp);
                                    Debug.WriteLine("Epub -Tmp Verzeichnis gelöscht.");
                                }

                                _epupTmp = await FileSystem.Current.LocalStorage.CreateFolderAsync("EPubTmp", CreationCollisionOption.ReplaceExisting);
                                Debug.WriteLine($"Epub -Tmp Verzeichnis neu erzeugt unter: {_epupTmp.Path}");
                                zip.ExtractToDirectory(_epupTmp.Path);

                                var cf = PortablePath.Combine(new string[] { _epupTmp.Path, "META-INF", "container.xml" });
                                var contFile = await FileSystem.Current.GetFileFromPathAsync(cf);
                                if (contFile != null)
                                {
                                    Debug.WriteLine($"Container File found.");
                                    var serc = new XmlSerializer(typeof(container));
                                    _container = (container)serc.Deserialize(await contFile.OpenAsync(PCLStorage.FileAccess.Read));

                                    cf = PortablePath.Combine(new string[] { _epupTmp.Path, _container.rootfiles.rootfile.fullpath.Replace('/', PortablePath.DirectorySeparatorChar) });
                                    IFile contentfile = null;

                                    Debug.WriteLine($"Open ContentFile: {cf}");
                                    contentfile = await FileSystem.Current.GetFileFromPathAsync(cf);
                                    Debug.WriteLine($"ContentFile found.");

                                    if (contentfile != null)
                                    {
                                        var ser = new XmlSerializer(typeof(package));
                                        content = (package)ser.Deserialize(await contentfile.OpenAsync(PCLStorage.FileAccess.Read));
                                        GetContentMetaData(content);
                                        foreach (var itmRef in content.spine.itemref)
                                        {
                                            Debug.WriteLine(content.manifest.FirstOrDefault(p => p.id == itmRef.idref)?.href ?? "???");
                                        }

                                        var a = Path.GetDirectoryName(cf);
                                        a = Path.Combine(a, content.manifest.FirstOrDefault(p => p.id == content.spine.itemref[2].idref).href);
                                       // WebViewContent = new Uri( a, UriKind.RelativeOrAbsolute);
                                        //a = $"ms-appx-web:///{content.manifest.FirstOrDefault(p => p.id == content.spine.itemref[0].idref).href}";
                                        Debug.WriteLine(a);
                                        var httpfile = await FileSystem.Current.GetFileFromPathAsync(a);
                                        Debug.WriteLine($"httpFile found.");

                                        if (httpfile != null)
                                        {
                                            var serhtml = new XmlSerializer(typeof(html));
                                            var st = await httpfile.OpenAsync(PCLStorage.FileAccess.Read);
                                            var ms = new MemoryStream();
                                            st.CopyTo(ms);
                                            ms.Seek(0, SeekOrigin.Begin);
                                            //Debug.WriteLine(System.Net.WebUtility.HtmlDecode(Encoding.UTF8.GetString(ms.ToArray())));
                                            var text = HttpUtility.HtmlDecode(Regex.Replace(Encoding.UTF8.GetString(ms.ToArray()), "<(.|\n)*?>", ""));

                                            var chapterText = text.Split('\n'); 
                                            Debug.WriteLine(text);


                                            var list = new ObservableCollection<Chapter>();
                                            foreach (var s in chapterText)
                                            {
                                                list.Add(new Chapter{ ChapterText = s.Trim()});
                                            }

                                            Chapters = list;
                                        }
                                    }
                                }

                            }
                        }

                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        throw;
                    }
                });
            }
        }



        async Task DeleteDir(IFolder f)
        {
            var subFoder = await f.GetFoldersAsync();
            if (subFoder.Count > 0)
            {
                foreach (var folder in subFoder)
                {
                    await DeleteDir(folder);
                }
            }

            var files = await f.GetFilesAsync();
            foreach (var file in files)
            {
                await file.DeleteAsync();
            }
            await f.DeleteAsync();
        }

        private void GetContentMetaData(package epContent)
        {
            ContentMetaData.Clear();
            for (int i = 0; i < epContent.metadata.Items.Length; i++)
            {
                switch (epContent.metadata.Items[i])
                {
                    case packageMetadataMeta tg:
                        Debug.WriteLine($"{epContent.metadata.ItemsElementName[i]} {tg.content}");
                        ContentMetaData.Add($"Meta_{tg.name}", tg.content);
                        break;
                    case creator tg:
                        Debug.WriteLine($"{epContent.metadata.ItemsElementName[i]} {tg.Value}");
                        ContentMetaData.Add($"{epContent.metadata.ItemsElementName[i]}", tg.Value);
                        break;
                    case date tg:
                        Debug.WriteLine($"{epContent.metadata.ItemsElementName[i]} {tg.Value}");
                        ContentMetaData.Add($"{epContent.metadata.ItemsElementName[i]}", tg.Value.ToLongDateString());
                        break;
                    case identifier tg:
                        Debug.WriteLine($"{epContent.metadata.ItemsElementName[i]} {tg.Value}");
                        ContentMetaData.Add($"{epContent.metadata.ItemsElementName[i]}", tg.Value);
                        break;
                    default:
                        Debug.WriteLine($"{epContent.metadata.ItemsElementName[i]} {epContent.metadata.Items[i]}");
                        ContentMetaData.Add($"{epContent.metadata.ItemsElementName[i]}", epContent.metadata.Items[i].ToString());
                        break;
                }

            }
        }
    }







    

}
