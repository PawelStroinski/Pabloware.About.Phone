using System;
using Dietphone.Models;
using Dietphone.Tools;
using System.Collections.Generic;
using System.Windows;

namespace Dietphone.ViewModels
{
    public class MealItemViewModel : ViewModelWithBuffer<MealItem>
    {
        public event EventHandler ItemChanged;
        private readonly ScoreSelector scores;
        private static readonly Constrains big = new Constrains { Max = 10000 };

        public MealItemViewModel(MealItem model, Factories factories)
            : base(model, factories)
        {
            scores = new MealItemScoreSelector(this);
        }

        public Guid ProductId
        {
            get
            {
                return BufferOrModel.ProductId;
            }
            set
            {
                BufferOrModel.ProductId = value;
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
                var product = BufferOrModel.Product;
                return product.Name;
            }
        }

        public string Value
        {
            get
            {
                var result = BufferOrModel.Value;
                return result.ToStringOrEmpty();
            }
            set
            {
                var oldValue = BufferOrModel.Value;
                var newValue = oldValue.TryGetValueOf(value);
                BufferOrModel.Value = big.Constraint(newValue);
                OnItemChanged();
            }
        }

        public string ValueWithUnit
        {
            get
            {
                var value = BufferOrModel.Value;
                return string.Format("{0} {1}", value, Unit);
            }
        }

        public string Unit
        {
            get
            {
                var result = BufferOrModel.Unit;
                return result.GetAbbreviationOrServingSizeDesc(BufferOrModel.Product);
            }
        }

        // Uwaga: zmiana UnitWithDetalis może zmienić Value za pomocą SetOneServingIfIsZeroServings().
        public string UnitWithDetalis
        {
            get
            {
                var result = BufferOrModel.Unit;
                return result.GetAbbreviationOrServingSizeDetalis(BufferOrModel.Product);
            }
            set
            {
                var oldValue = BufferOrModel.Unit;
                var newValue = oldValue.TryGetValueOfAbbreviationOrServingSizeDetalis(value, BufferOrModel.Product);
                BufferOrModel.Unit = newValue;
                SetOneServingIfIsZeroServings();
                OnItemChanged();
            }
        }

        public ScoreSelector Scores
        {
            get
            {
                return scores;
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
                return UnitAbbreviations.GetAbbreviationsOrServingSizeDetalisFiltered(IsUnitUsable, BufferOrModel.Product);
            }
        }

        public void CopyFromModel(MealItem model)
        {
            BufferOrModel.CopyFrom(model);
            OnItemChanged();
        }

        public void Invalidate()
        {
            OnItemChanged();
        }

        private string Energy
        {
            get
            {
                var result = BufferOrModel.Energy;
                return string.Format("{0} kcal", result);
            }
        }

        private string Protein
        {
            get
            {
                var value = BufferOrModel.Protein;
                var rounded = (int)Math.Round(value);
                return string.Format("{0} białk", rounded);
            }
        }

        private string Fat
        {
            get
            {
                var value = BufferOrModel.Fat;
                var rounded = (int)Math.Round(value);
                return string.Format("{0} tł", rounded);
            }
        }

        private string DigestibleCarbs
        {
            get
            {
                var value = BufferOrModel.DigestibleCarbs;
                var rounded = (int)Math.Round(value);
                return string.Format("{0} węgl", rounded);
            }
        }

        private string Cu
        {
            get
            {
                var result = BufferOrModel.Cu;
                return string.Format("{0} WW", result);
            }
        }

        private string Fpu
        {
            get
            {
                var result = BufferOrModel.Fpu;
                return string.Format("{0} WBT", result);
            }
        }

        private bool IsUnitUsable(Models.Unit unit)
        {
            var unitUsability = new UnitUsability()
            {
                Product = BufferOrModel.Product,
                Unit = unit
            };
            return unitUsability.AnyNutrientsPerUnitPresent;
        }

        private void SetOneServingIfIsZeroServings()
        {
            if (BufferOrModel.Unit == Models.Unit.ServingSize && BufferOrModel.Value == 0)
            {
                BufferOrModel.Value = 1;
            }
        }

        protected void OnItemChanged()
        {
            OnPropertyChanged("Value");
            OnPropertyChanged("ValueWithUnit");
            OnPropertyChanged("Unit");
            OnPropertyChanged("UnitWithDetalis");
            OnPropertyChanged("Scores");
            if (ItemChanged != null)
            {
                ItemChanged(this, EventArgs.Empty);
            }
        }

        private class MealItemScoreSelector : ScoreSelector
        {
            private readonly MealItemViewModel item;

            public MealItemScoreSelector(MealItemViewModel item)
                : base(item.factories)
            {
                this.item = item;
            }

            protected override string GetCurrent()
            {
                if (settingsCopy.ScoreEnergy)
                {
                    return item.Energy;
                }
                if (settingsCopy.ScoreProtein)
                {
                    return item.Protein;
                }
                if (settingsCopy.ScoreDigestibleCarbs)
                {
                    return item.DigestibleCarbs;
                }
                if (settingsCopy.ScoreFat)
                {
                    return item.Fat;
                }
                if (settingsCopy.ScoreCu)
                {
                    return item.Cu;
                }
                if (settingsCopy.ScoreFpu)
                {
                    return item.Fpu;
                }
                return string.Empty;
            }
        }
    }
}
