using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Xamarin.Forms;

namespace JManReader.ViewModels
{
    public class Paragraph: INotifyPropertyChanged
    {
        #region Properties

        private string _paragraphText;

        public string ParagraphText
        {
            get => _paragraphText;
            set => SetProperty(ref _paragraphText, value);
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
                if (_isSelected != value && Device.RuntimePlatform!= Device.iOS)
                {
                    FontSize = value ? 16 : 14;
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