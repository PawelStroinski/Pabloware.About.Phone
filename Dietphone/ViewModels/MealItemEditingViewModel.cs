using System;
using Dietphone.Models;

namespace Dietphone.ViewModels
{
    public class MealItemEditingViewModel : ViewModelBase
    {
        public MealItemViewModel MealItem { get; private set; }
        public bool CanDelete { get; set; }
        public event EventHandler NeedToShow;
        public event EventHandler Confirmed;
        public event EventHandler Cancelled;
        public event EventHandler NeedToDelete;

        public void Show(MealItemViewModel mealItem)
        {
            MealItem = mealItem;
            OnNeedToShow();
        }

        public void Confirm()
        {
            OnConfirmed();
        }

        public void Cancel()
        {
            OnCancelled();
        }

        public void Delete()
        {
            OnNeedToDelete();
        }

        protected void OnNeedToShow()
        {
            if (NeedToShow != null)
            {
                NeedToShow(this, EventArgs.Empty);
            }
        }

        protected void OnConfirmed()
        {
            if (Confirmed != null)
            {
                Confirmed(this, EventArgs.Empty);
            }
        }

        protected void OnCancelled()
        {
            if (Cancelled != null)
            {
                Cancelled(this, EventArgs.Empty);
            }
        }

        protected void OnNeedToDelete()
        {
            if (NeedToDelete != null)
            {
                NeedToDelete(this, EventArgs.Empty);
            }
        }
    }
}