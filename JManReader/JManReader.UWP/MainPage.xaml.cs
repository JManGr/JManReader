using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Prism;
using Prism.Ioc;
using Unity;
using Prism.Unity;
using SpeakIt;

namespace JManReader.UWP
{
    public sealed partial class MainPage
    {
        public MainPage()
        {
            this.InitializeComponent();

            LoadApplication(new JManReader.App(new UwpInitializer()));
        }
    }

    public class UwpInitializer : IPlatformInitializer
    {
       
        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            var tts = new SpeakIt.UWP.SpeakThis();
            containerRegistry.RegisterInstance(typeof(ISpeakThis), tts);
        }
    }
}
