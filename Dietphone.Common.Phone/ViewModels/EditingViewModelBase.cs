using System;
using Dietphone.Models;

namespace Dietphone.ViewModels
{
    public abstract class EditingViewModelBase<TModel> : ViewModelBase
    {
        public event EventHandler GotDirty;
        public event EventHandler<CannotSaveEventArgs> CannotSave;
        protected TModel modelCopy;
        protected TModel modelSource;
        protected readonly Factories factories;
        protected readonly Navigator navigator;
        protected readonly Finder finder;

        public EditingViewModelBase(Factories factories, Navigator navigator)
        {
            this.factories = factories;
            this.navigator = navigator;
            finder = factories.Finder;
            Load();
        }

        public virtual bool CanSave()
        {
            var validation = Validate();
            if (!string.IsNullOrEmpty(validation))
            {
                var args = new CannotSaveEventArgs();
                args.Reason = validation;
                OnCannotSave(args);
                return args.Ignore;
            }
            return true;
        }

        public virtual void CancelAndReturn()
        {
            navigator.GoBack();
        }

        private void Load()
        {
            FindAndCopyModel();
            if (modelCopy == null)
            {
                navigator.GoBack();
            }
            else
            {
                MakeViewModel();
            }
        }

        protected abstract void FindAndCopyModel();

        protected abstract void MakeViewModel();

        protected abstract string Validate();

        protected void OnGotDirty()
        {
            if (GotDirty != null)
            {
                GotDirty(this, EventArgs.Empty);
            }
        }

        protected void OnCannotSave(CannotSaveEventArgs e)
        {
            if (CannotSave != null)
            {
                CannotSave(this, e);
            }
        }
    }

    public class CannotSaveEventArgs : EventArgs
    {
        public string Reason { get; set; }
        public bool Ignore { get; set; }
    }
}
