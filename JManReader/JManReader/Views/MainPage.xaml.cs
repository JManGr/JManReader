using Xamarin.Forms;

namespace JManReader.Views
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private void ListView_OnItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var l = sender as ListView;
            if (l?.SelectedItem != null)
            {
                try
                {
                    l.ScrollTo(l.SelectedItem, ScrollToPosition.Center, false);
                }
                catch
                {
                    //ignore
                }
            }
        }

    }
}