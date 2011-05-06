using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.ComponentModel;

namespace Dietphone.ViewModels
{
    public class SubViewModel : ViewModelBase
    {
        public Navigator Navigator { protected get; set; }
        protected string search = "";
        private bool isBusy;

        public string Search
        {
            set
            {
                var trimmedValue = value.Trim();
                var differs = !search.EqualsIgnoringCase(trimmedValue);
                if (differs)
                {
                    search = trimmedValue;
                    OnSearchChanged();
                }
            }
        }

        public bool IsBusy
        {
            get
            {
                return isBusy;
            }
            set
            {
                isBusy = value;
                OnPropertyChanged("IsBusy");
            }
        }

        public virtual void Load()
        {
        }

        public virtual void Refresh()
        {
        }

        protected virtual void OnSearchChanged()
        {
        }
    }

    public class SubViewModelConnector
    {
        private MainViewModel mainViewModel;
        private SubViewModel subViewModel;
        private Navigator navigator;

        public SubViewModelConnector(MainViewModel mainViewModel)
        {
            this.mainViewModel = mainViewModel;
            mainViewModel.PropertyChanged += new PropertyChangedEventHandler(mainViewModel_PropertyChanged);
        }

        public SubViewModel SubViewModel
        {
            set
            {
                if (subViewModel != value)
                {
                    subViewModel = value;
                    OnSubViewModelChanged();
                }
            }
        }

        public Navigator Navigator
        {
            set
            {
                navigator = value;
                OnNavigatorChanged();
            }
        }

        public void Refresh()
        {
            if (subViewModel != null)
            {
                subViewModel.Refresh();
            }
        }

        protected virtual void OnSubViewModelChanged()
        {
            subViewModel.Search = mainViewModel.Search;
            subViewModel.Navigator = navigator;
            subViewModel.Load();
        }

        private void mainViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Search")
            {
                if (subViewModel != null)
                {
                    subViewModel.Search = mainViewModel.Search;
                }
            }
        }

        private void OnNavigatorChanged()
        {
            if (subViewModel != null)
            {
                subViewModel.Navigator = navigator;
            }
        }
    }
}
