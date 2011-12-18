using System;
using Dietphone.Models;
using Dietphone.Tools;

namespace Dietphone.ViewModels
{
    public abstract class EditingViewModelBase<TModel> : PivotTombstoningViewModel
    {
        public Navigator Navigator { get; set; }
        public event EventHandler<CannotSaveEventArgs> CannotSave;
        public event EventHandler IsDirtyChanged;
        protected TModel modelCopy;
        protected TModel modelSource;
        protected readonly Factories factories;
        protected readonly Finder finder;
        private bool isDirty;
        private const string IS_DIRTY = "IS_DIRTY";

        public EditingViewModelBase(Factories factories)
        {
            this.factories = factories;
            finder = factories.Finder;
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
                OnCommonUiReady();
            }
        }

        public override void Tombstone()
        {
            TombstoneModel();
            TombstoneOthers();
            TombstoneCommonUi();
        }

        public override void Untombstone()
        {
            throw new NotSupportedException("Use Load() instead");
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
            var state = StateProvider.State;
            state[IS_DIRTY] = IsDirty;
            TombstonePivot();
        }

        private void UntombstoneCommonUi()
        {
            var state = StateProvider.State;
            if (state.ContainsKey(IS_DIRTY))
            {
                IsDirty = (bool)state[IS_DIRTY];
                UntombstonePivot();
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

        protected virtual void OnCommonUiReady()
        {
        }
    }

    public class CannotSaveEventArgs : EventArgs
    {
        public string Reason { get; set; }
        public bool Ignore { get; set; }
    }
}
