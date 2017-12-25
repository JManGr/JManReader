﻿using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace JManReader.ViewModels
{
    public class Chapter: INotifyPropertyChanged
    {
        #region Properties

        private string _chapterText;

        public string ChapterText
        {
            get => _chapterText;
            set => SetProperty(ref _chapterText, value);
        }

        private double _fontSize =14;

        public double FontSize
        {
            get => _fontSize;
            set => SetProperty(ref _fontSize, value);
        }

        private bool _isSelected;

        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (_isSelected != value)
                {
                    FontSize = value ? 22 : 14;
                }
                SetProperty(ref _isSelected, value);

            }
        }

        void SetProperty<T>(ref T property, T value, [CallerMemberName] string propertyName = null) where T : IEquatable<T>
        {
            if(!(property?.Equals(value)??false))
            {
                property = value;
                OnPropertyChanged(propertyName);
            }
        }
 

        #endregion

        #region PropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}