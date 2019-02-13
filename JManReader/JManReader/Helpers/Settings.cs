using System;
using System.Collections.Generic;
using System.Text;
using Plugin.Settings;
using Plugin.Settings.Abstractions;

namespace JManReader.Helpers
{
    /// <summary>
    /// This is the Settings static class that can be used in your Core solution or in any
    /// of your client applications. All settings are laid out the same exact way with getters
    /// and setters. 
    /// </summary>
    public static class Settings
    {
        private static bool _isFirst =true;
        private static ISettings AppSettings => CrossSettings.Current;

        #region Setting Constants


        private const string CurReadFileKey = "CurReadFileKey";
        private const string CurReadFileKeyDefault = "";

        private const string CurChapterDescriptionsKey = "CurChapterDescriptionsKey";
        private static readonly int CurChapterDescriptionsKeyDefault = -1;

        private const string CurParagraphKey = "CurParagraphKey";
        private static readonly int CurParagraphKeyDefault = -1;

        #endregion


        public static string CurStoredReadFile
        {
            get => AppSettings.GetValueOrDefault(CurReadFileKey, CurReadFileKeyDefault);
            set => AppSettings.AddOrUpdateValue(CurReadFileKey, value);
        }

       
        public static int CurStoreChapterDescriptions
        {
            get => AppSettings.GetValueOrDefault(CurChapterDescriptionsKey, CurChapterDescriptionsKeyDefault);
            set => AppSettings.AddOrUpdateValue(CurChapterDescriptionsKey, value);
        }

        public static int CurStoredParagraph
        {
            get => AppSettings.GetValueOrDefault(CurParagraphKey, CurParagraphKeyDefault);
            set => AppSettings.AddOrUpdateValue(CurParagraphKey, value);
        }

    }
}
