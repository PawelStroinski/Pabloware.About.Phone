using System;
using Dietphone.Models;
using System.ComponentModel;

namespace Dietphone.ViewModels
{
    public abstract class LoaderBase
    {
        public event EventHandler Loaded;
        protected Factories factories;
        protected bool isLoading;
        protected SubViewModel viewModel;

        public void LoadAsync()
        {
            if (viewModel == null)
            {
                throw new InvalidOperationException("Pass ViewModel in constructor for this operation.");
            }
            if (isLoading)
            {
                return;
            }
            var worker = new BackgroundWorker();
            worker.DoWork += delegate { DoWork(); };
            worker.RunWorkerCompleted += delegate { WorkCompleted(); };
            viewModel.IsBusy = true;
            isLoading = true;
            worker.RunWorkerAsync();
        }

        protected abstract void DoWork();

        protected virtual void WorkCompleted()
        {
            viewModel.IsBusy = false;
            isLoading = false;
            OnLoaded();
        }

        protected void OnLoaded()
        {
            if (Loaded != null)
            {
                Loaded(this, EventArgs.Empty);
            }
        }
    }
}
