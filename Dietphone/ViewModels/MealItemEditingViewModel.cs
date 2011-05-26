using System;

namespace Dietphone.ViewModels
{
    public class MealItemEditingViewModel : ViewModelBase
    {
        public MealItemViewModel MealItem { get; set; }
        public bool CanDelete { get; set; }
        public event EventHandler Showing;
        public event EventHandler Confirming;
        public event EventHandler Cancelling;
        public event EventHandler Deleting;

        public void Show()
        {
            OnShowing();
        }

        public void Confirm()
        {
            OnConfirming();
        }

        public void Cancel()
        {
            OnCancelling();
        }

        public void Delete()
        {
            OnDeleting();
        }

        protected void OnShowing()
        {
            if (Showing != null)
            {
                Showing(this, EventArgs.Empty);
            }
        }

        protected void OnConfirming()
        {
            if (Confirming != null)
            {
                Confirming(this, EventArgs.Empty);
            }
        }

        protected void OnCancelling()
        {
            if (Cancelling != null)
            {
                Cancelling(this, EventArgs.Empty);
            }
        }

        protected void OnDeleting()
        {
            if (Deleting != null)
            {
                Deleting(this, EventArgs.Empty);
            }
        }
    }
}