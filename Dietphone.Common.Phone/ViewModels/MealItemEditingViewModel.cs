using System;
using Dietphone.Models;
using Dietphone.Tools;

namespace Dietphone.ViewModels
{
    public class MealItemEditingViewModel : ViewModelBase
    {
        public bool CanDelete { get; set; }
        public StateProvider StateProvider { private get; set; }
        public MealItemViewModel MealItem { get; private set; }
        public bool IsVisible { get; private set; }
        public event EventHandler NeedToShow;
        public event EventHandler Confirmed;
        public event EventHandler Cancelled;
        public event EventHandler NeedToDelete;
        private const string MEAL_ITEM = "MEAL_ITEM";

        public void Show(MealItemViewModel mealItem)
        {
            MealItem = mealItem;
            Untombstone();
            OnNeedToShow();
            IsVisible = true;
        }

        public void Confirm()
        {
            OnConfirmed();
            IsVisible = false;
        }

        public void Cancel()
        {
            OnCancelled();
            IsVisible = false;
        }

        public void Delete()
        {
            OnNeedToDelete();
            IsVisible = false;
        }

        public void Tombstone()
        {
            var state = StateProvider.State;
            state[MEAL_ITEM] = MealItem.SerializeModel();
        }

        private void Untombstone()
        {
            var state = StateProvider.State;
            if (state.ContainsKey(MEAL_ITEM))
            {
                var stateValue = (string)state[MEAL_ITEM];
                var untombstoned = stateValue.Deserialize<MealItem>(string.Empty);
                if (MealItem.ProductId == untombstoned.ProductId)
                {
                    MealItem.CopyFromModel(untombstoned);
                }
            }
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