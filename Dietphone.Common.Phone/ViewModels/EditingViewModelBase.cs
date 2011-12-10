using System;
using Dietphone.Models;
using Dietphone.Tools;

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
        protected readonly StateProvider stateProvider;

        public EditingViewModelBase(Factories factories, Navigator navigator, StateProvider stateProvider)
        {
            this.factories = factories;
            this.navigator = navigator;
            finder = factories.Finder;
            this.stateProvider = stateProvider;
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
            UntombstoneModel();
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

        protected abstract void UntombstoneModel();

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
