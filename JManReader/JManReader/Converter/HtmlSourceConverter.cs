﻿using System;
using System.Globalization;
using Xamarin.Forms;

namespace JManReader.ViewModels
{
    public class HtmlSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var html = new HtmlWebViewSource();

            if (value != null)
            {
                html.Html = "Test,Test";// value.ToString();
            }

            return html;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}