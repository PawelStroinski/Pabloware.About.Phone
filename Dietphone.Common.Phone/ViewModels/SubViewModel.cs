using System;
using Dietphone.Tools;
using System.ComponentModel;
using System.Windows;

namespace Dietphone.ViewModels
{
    public abstract class SubViewModel : ViewModelBase
    {
        public Navigator Navigator { protected get; set; }
        public event EventHandler Loaded;
        public event EventHandler Refreshing;
        public event EventHandler Refreshed;
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

        public abstract void Load();

        public abstract void Refresh();

        public virtual void Add()
        {
        }

        protected abstract void OnSearchChanged();

        protected void OnLoaded()
        {
            if (Loaded != null)
            {
                Loaded(this, EventArgs.Empty);
            }
        }

        protected void OnRefreshing()
        {
            if (Refreshing != null)
            {
                Refreshing(this, EventArgs.Empty);
            }
        }

        protected void OnRefreshed()
        {
            if (Refreshed != null)
            {
                Refreshed(this, EventArgs.Empty);
            }
        }
    }

    public class SubViewModelConnector
    {
        private SubViewModel subViewModel;
        private Navigator navigator;
        private readonly MainViewModel mainViewModel;

        public SubViewModelConnector(MainViewModel mainViewModel)
        {
            this.mainViewModel = mainViewModel;
            mainViewModel.PropertyChanged += mainViewModel_PropertyChanged;
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

        public void Add()
        {
            if (subViewModel != null)
            {
                subViewModel.Add();
            }
        }

        protected virtual void OnSubViewModelChanged()
        {
            subViewModel.Search = mainViewModel.Search;
            subViewModel.Navigator = navigator;
            subViewModel.Loaded += subViewModel_Loaded;
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

        private void subViewModel_Loaded(object sender, EventArgs e)
        {
            mainViewModel.Loaded();
        }

        protected void OnNavigatorChanged()
        {
            if (subViewModel != null)
            {
                subViewModel.Navigator = navigator;
            }
        }
    }
}
