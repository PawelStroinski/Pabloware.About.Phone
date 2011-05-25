using System;
using Dietphone.Models;
using Dietphone.Tools;
using System.Collections.Generic;

namespace Dietphone.ViewModels
{
    public class MealItemViewModel : ViewModelBase
    {
        public MealItem MealItem { get; private set; }
        public event EventHandler ItemChanged;
        private static readonly Constrains big = new Constrains { Max = 10000 };

        public MealItemViewModel(MealItem mealItem)
        {
            MealItem = mealItem;
        }

        public Guid ProductId
        {
            get
            {
                return MealItem.ProductId;
            }
            set
            {
                MealItem.ProductId = value;
                OnPropertyChanged("ProductName");
                OnPropertyChanged("AllUsableUnitsWithDetalis");
                OnPropertyChanged("HasManyUsableUnits");
                OnItemChanged();
            }
        }

        public string ProductName
        {
            get
            {
                var product = MealItem.Product;
                return product.Name;
            }
        }

        public string Value
        {
            get
            {
                var result = MealItem.Value;
                return result.ToStringOrEmpty();
            }
            set
            {
                var oldValue = MealItem.Value;
                var newValue = oldValue.TryGetValueOf(value);
                MealItem.Value = big.Constraint(newValue);
                OnItemChanged();
            }
        }

        public string ValueWithUnit
        {
            get
            {
                var value = MealItem.Value;
                return string.Format("{0} {1}", value, Unit);
            }
        }

        public string Unit
        {
            get
            {
                var result = MealItem.Unit;
                return result.GetAbbreviationOrServingSizeDesc(MealItem.Product);
            }
        }

        // Uwaga: zmiana UnitWithDetalis może zmienić Value za pomocą SetOneServingIfIsZeroServings().
        public string UnitWithDetalis
        {
            get
            {
                var result = MealItem.Unit;
                return result.GetAbbreviationOrServingSizeDetalis(MealItem.Product);
            }
            set
            {
                var oldValue = MealItem.Unit;
                var newValue = oldValue.TryGetValueOfAbbreviationOrServingSizeDetalis(value, MealItem.Product);
                MealItem.Unit = newValue;
                SetOneServingIfIsZeroServings();
                OnItemChanged();
            }
        }

        public string Energy
        {
            get
            {
                var result = MealItem.Energy;
                return string.Format("{0} kcal", result);
            }
        }

        public string Cu
        {
            get
            {
                var result = MealItem.Cu;
                return string.Format("{0} WW", result);
            }
        }

        public string Fpu
        {
            get
            {
                var result = MealItem.Fpu;
                return string.Format("{0} WBT", result);
            }
        }

        public bool HasManyUsableUnits
        {
            get
            {
                return AllUsableUnitsWithDetalis.Count > 1;
            }
        }

        public List<string> AllUsableUnitsWithDetalis
        {
            get
            {
                return UnitAbbreviations.GetAbbreviationsOrServingSizeDetalisFiltered(IsUnitUsable, MealItem.Product);
            }
        }

        private bool IsUnitUsable(Models.Unit unit)
        {
            var unitUsability = new UnitUsability()
            {
                Product = MealItem.Product,
                Unit = unit
            };
            return unitUsability.AnyNutrientsPerUnitPresent;
        }

        private void SetOneServingIfIsZeroServings()
        {
            if (MealItem.Unit == Models.Unit.ServingSize && MealItem.Value == 0)
            {
                MealItem.Value = 1;
            }
        }

        protected void OnItemChanged()
        {
            OnPropertyChanged("Value");
            OnPropertyChanged("ValueWithUnit");
            OnPropertyChanged("Unit");
            OnPropertyChanged("UnitWithDetalis");
            OnPropertyChanged("Energy");
            OnPropertyChanged("Cu");
            OnPropertyChanged("Fpu");
            if (ItemChanged != null)
            {
                ItemChanged(this, EventArgs.Empty);
            }
        }
    }
}
