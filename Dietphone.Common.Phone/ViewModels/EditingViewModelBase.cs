using System;
using Dietphone.Models;
using Dietphone.Tools;

namespace Dietphone.ViewModels
{
    public abstract class EditingViewModelBase<TModel> : ViewModelBase
    {
        public Navigator Navigator { get; set; }
        public int Pivot { get; set; }
        public event EventHandler<CannotSaveEventArgs> CannotSave;
        public event EventHandler IsDirtyChanged;
        protected TModel modelCopy;
        protected TModel modelSource;
        protected readonly Factories factories;
        protected readonly Finder finder;
        protected readonly StateProvider stateProvider;
        private bool isDirty;
        private const string PIVOT = "PIVOT";
        private const string IS_DIRTY = "IS_DIRTY";

        public EditingViewModelBase(Factories factories, StateProvider stateProvider)
        {
            this.factories = factories;
            finder = factories.Finder;
            this.stateProvider = stateProvider;
        }

        public bool IsDirty
        {
            get
            {
                return isDirty;
            }
            set
            {
                if (isDirty != value)
                {
                    isDirty = value;
                    OnIsDirtyChanged();
                }
            }
        }

        public void Load()
        {
            FindAndCopyModel();
            if (modelCopy == null)
            {
                Navigator.GoBack();
            }
            else
            {
                UntombstoneModel();
                OnModelReady();
                MakeViewModel();
                UntombstoneOthers();
                UntombstoneCommonUi();
            }
        }

        public void Tombstone()
        {
            TombstoneModel();
            TombstoneOthers();
            TombstoneCommonUi();
        }

        public bool CanSave()
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

        public void CancelAndReturn()
        {
            Navigator.GoBack();
        }

        protected abstract void FindAndCopyModel();

        protected abstract void MakeViewModel();

        protected abstract string Validate();

        protected abstract void TombstoneModel();

        protected abstract void UntombstoneModel();

        protected virtual void TombstoneOthers()
        {
        }

        protected virtual void UntombstoneOthers()
        {
        }

        private void TombstoneCommonUi()
        {
            var state = stateProvider.State;
            state[PIVOT] = Pivot;
            state[IS_DIRTY] = IsDirty;
        }

        private void UntombstoneCommonUi()
        {
            var state = stateProvider.State;
            if (state.ContainsKey(PIVOT))
            {
                Pivot = (int)state[PIVOT];
                IsDirty = (bool)state[IS_DIRTY];
            }
        }

        protected void OnCannotSave(CannotSaveEventArgs e)
        {
            if (CannotSave != null)
            {
                CannotSave(this, e);
            }
        }

        protected void OnIsDirtyChanged()
        {
            if (IsDirtyChanged != null)
            {
                IsDirtyChanged(this, EventArgs.Empty);
            }
        }

        protected virtual void OnModelReady()
        {
        }
    }

    public class CannotSaveEventArgs : EventArgs
    {
        public string Reason { get; set; }
        public bool Ignore { get; set; }
    }
}
