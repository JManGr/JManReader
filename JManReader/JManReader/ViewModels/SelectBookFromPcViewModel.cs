using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using PCLStorage;
using Prism.Navigation;

namespace JManReader.ViewModels
{
    public class SelectBookFromPcViewModel : BindableBase, INavigationAware
    {

        public SelectBookFromPcViewModel()
        {

        }

        #region Properties

        private ObservableCollection<string> _bookList;

        public ObservableCollection<string> BookList
        {
            get => _bookList;
            set => SetProperty(ref _bookList, value);
        }

        private string _selectedBook;
        private bool _isLocalFile;

        public string SelectedBook
        {
            get => _selectedBook;
            set => SetProperty(ref _selectedBook, value);
        }

        #endregion




        #region Navigation

        public void OnNavigatedFrom(NavigationParameters parameters)
        {
            parameters.Add("SelectedBook", SelectedBook);
            parameters.Add("IsLocalFile", _isLocalFile);
        }

        public void OnNavigatedTo(NavigationParameters parameters)
        {
            if (parameters.Keys.Contains("Books"))
            {
                _isLocalFile = false;
                var books = parameters["Books"] as string[];
                BookList = new ObservableCollection<string>(books);
            }
            else
            {
                if (parameters.Keys.Contains("FileList"))
                {
                    _isLocalFile = true;
                    BookList = new ObservableCollection<string>();
                    if (parameters["FileList"] is IList<IFile> flist)
                    {
                        foreach (var f in flist)
                        {
                            BookList.Add(f.Name);
                        }
                    }
                }
                else
                {
                    BookList = new ObservableCollection<string>();
                }
            }
        }

        public void OnNavigatingTo(NavigationParameters parameters)
        {

        }

        #endregion
    }
}
