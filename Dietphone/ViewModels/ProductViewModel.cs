using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using Dietphone.Models;
using System.Collections.ObjectModel;
using Dietphone.Tools;

namespace Dietphone.ViewModels
{
    public class ProductViewModel : ViewModelBase
    {
        public Product Product { get; private set; }
        public IEnumerable<CategoryViewModel> Categories { private get; set; }
        public MaxCuAndFpuInCategories MaxCuAndFpu { private get; set; }
        private bool autoCalculatingEnergyPer100g;
        private bool autoCalculatingEnergyPerServing;
        private static readonly Constrains max100g = new Constrains { Max = 100 };
        private static readonly Constrains big = new Constrains { Max = 10000 };
        private const byte RECT_WIDTH = 25;

        public ProductViewModel(Product product)
        {
            Product = product;
            autoCalculatingEnergyPer100g = Product.EnergyPer100g == 0;
            autoCalculatingEnergyPerServing = Product.EnergyPerServing == 0;
        }

        public Guid Id
        {
            get
            {
                return Product.Id;
            }
        }

        public string Name
        {
            get
            {
                return Product.Name;
            }
            set
            {
                if (value != Product.Name)
                {
                    Product.Name = value;
                    OnPropertyChanged("Name");
                }
            }
        }

        public CategoryViewModel Category
        {
            get
            {
                return FindCategory();
            }
            set
            {
                if (value != null)
                {
                    SetCategory(value);
                }
            }
        }

        public string ServingSizeValue
        {
            get
            {
                var result = Product.ServingSizeValue;
                return result.ToStringOrEmpty();
            }
            set
            {
                var oldValue = Product.ServingSizeValue;
                var newValue = oldValue.TryGetValueOf(value);
                Product.ServingSizeValue = big.Constraint(newValue);
                OnPropertyChanged("ServingSizeValue");
            }
        }

        public string ServingSizeUnit
        {
            get
            {
                var result = Product.ServingSizeUnit;
                return result.GetAbbreviation();
            }
            set
            {
                var oldValue = Product.ServingSizeUnit;
                var newValue = oldValue.TryGetValueOfAbbreviation(value);
                Product.ServingSizeUnit = newValue;
                OnPropertyChanged("ServingSizeUnit");
            }
        }

        public string ServingSizeDescription
        {
            get
            {
                return Product.ServingSizeDescription;
            }
            set
            {
                Product.ServingSizeDescription = value;
                OnPropertyChanged("ServingSizeDescription");
            }
        }

        public string EnergyPer100g
        {
            get
            {
                var result = Product.EnergyPer100g;
                return result.ToStringOrEmpty();
            }
            set
            {
                var oldValue = Product.EnergyPer100g;
                var newValue = oldValue.TryGetValueOf(value);
                Product.EnergyPer100g = big.Constraint(newValue);
                autoCalculatingEnergyPer100g = false;
                OnPropertyChanged("EnergyPer100g");
            }
        }

        public string EnergyPerServing
        {
            get
            {
                var result = Product.EnergyPerServing;
                return result.ToStringOrEmpty();
            }
            set
            {
                var oldValue = Product.EnergyPerServing;
                var newValue = oldValue.TryGetValueOf(value);
                Product.EnergyPerServing = big.Constraint(newValue);
                autoCalculatingEnergyPerServing = false;
                OnPropertyChanged("EnergyPerServing");
            }
        }

        public string ProteinPer100g
        {
            get
            {
                var result = Product.ProteinPer100g;
                return result.ToStringOrEmpty();
            }
            set
            {
                var oldValue = Product.ProteinPer100g;
                var newValue = oldValue.TryGetValueOf(value);
                Product.ProteinPer100g = max100g.Constraint(newValue);
                InvalidateCuAndFpu();
                AutoCalculateEnergyPer100g();
                OnPropertyChanged("ProteinPer100g");
            }
        }

        public string ProteinPerServing
        {
            get
            {
                var result = Product.ProteinPerServing;
                return result.ToStringOrEmpty();
            }
            set
            {
                var oldValue = Product.ProteinPerServing;
                var newValue = oldValue.TryGetValueOf(value);
                Product.ProteinPerServing = big.Constraint(newValue);
                AutoCalculateEnergyPerServing();
                OnPropertyChanged("ProteinPerServing");
            }
        }

        public string FatPer100g
        {
            get
            {
                var result = Product.FatPer100g;
                return result.ToStringOrEmpty();
            }
            set
            {
                var oldValue = Product.FatPer100g;
                var newValue = oldValue.TryGetValueOf(value);
                Product.FatPer100g = max100g.Constraint(newValue);
                InvalidateCuAndFpu();
                AutoCalculateEnergyPer100g();
                OnPropertyChanged("FatPer100g");
            }
        }

        public string FatPerServing
        {
            get
            {
                var result = Product.FatPerServing;
                return result.ToStringOrEmpty();
            }
            set
            {
                var oldValue = Product.FatPerServing;
                var newValue = oldValue.TryGetValueOf(value);
                Product.FatPerServing = big.Constraint(newValue);
                AutoCalculateEnergyPerServing();
                OnPropertyChanged("FatPerServing");
            }
        }

        public string CarbsTotalPer100g
        {
            get
            {
                var result = Product.CarbsTotalPer100g;
                return result.ToStringOrEmpty();
            }
            set
            {
                var oldValue = Product.CarbsTotalPer100g;
                var newValue = oldValue.TryGetValueOf(value);
                Product.CarbsTotalPer100g = max100g.Constraint(newValue);
                InvalidateCuAndFpu();
                AutoCalculateEnergyPer100g();
                OnPropertyChanged("CarbsTotalPer100g");
                OnPropertyChanged("DigestibleCarbsPer100g");
            }
        }

        public string CarbsTotalPerServing
        {
            get
            {
                var result = Product.CarbsTotalPerServing;
                return result.ToStringOrEmpty();
            }
            set
            {
                var oldValue = Product.CarbsTotalPerServing;
                var newValue = oldValue.TryGetValueOf(value);
                Product.CarbsTotalPerServing = big.Constraint(newValue);
                AutoCalculateEnergyPerServing();
                OnPropertyChanged("CarbsTotalPerServing");
                OnPropertyChanged("DigestibleCarbsPerServing");
            }
        }

        public string FiberPer100g
        {
            get
            {
                var result = Product.FiberPer100g;
                return result.ToStringOrEmpty();
            }
            set
            {
                var oldValue = Product.FiberPer100g;
                var newValue = oldValue.TryGetValueOf(value);
                Product.FiberPer100g = max100g.Constraint(newValue);
                InvalidateCuAndFpu();
                AutoCalculateEnergyPer100g();
                OnPropertyChanged("FiberPer100g");
                OnPropertyChanged("DigestibleCarbsPer100g");
            }
        }

        public string FiberPerServing
        {
            get
            {
                var result = Product.FiberPerServing;
                return result.ToStringOrEmpty();
            }
            set
            {
                var oldValue = Product.FiberPerServing;
                var newValue = oldValue.TryGetValueOf(value);
                Product.FiberPerServing = big.Constraint(newValue);
                AutoCalculateEnergyPerServing();
                OnPropertyChanged("FiberPerServing");
                OnPropertyChanged("DigestibleCarbsPerServing");
            }
        }

        public string DigestibleCarbsPer100g
        {
            get
            {
                var result = Product.DigestibleCarbsPer100g;
                return result.ToStringOrEmpty();
            }
        }

        public string DigestibleCarbsPerServing
        {
            get
            {
                var result = Product.DigestibleCarbsPerServing;
                return result.ToStringOrEmpty();
            }
        }

        public string CuPer100g
        {
            get
            {
                var result = Product.CuPer100g;
                return string.Format("{0} WW", result);
            }
        }

        public string FpuPer100g
        {
            get
            {
                var result = Product.FpuPer100g;
                return string.Format("{0} WBT", result);
            }
        }

        public byte WidthOfFilledCuRect
        {
            get
            {
                var maxInCategory = MaxCuAndFpu.Get(Product.CategoryId);
                return GetWidthOfFilledRect(Product.CuPer100g, maxInCategory.CuPer100g);
            }
        }

        public byte WidthOfEmptyCuRect
        {
            get
            {
                return (byte)(RECT_WIDTH - WidthOfFilledCuRect);
            }
        }

        public byte WidthOfFilledFpuRect
        {
            get
            {
                var maxInCategory = MaxCuAndFpu.Get(Product.CategoryId);
                return GetWidthOfFilledRect(Product.FpuPer100g, maxInCategory.FpuPer100g);
            }
        }

        public byte WidthOfEmptyFpuRect
        {
            get
            {
                return (byte)(RECT_WIDTH - WidthOfFilledFpuRect);
            }
        }

        public byte DoubledWidthOfFilledCuRect
        {
            get
            {
                return (byte)(WidthOfFilledCuRect * 2);
            }
        }

        public byte DoubledWidthOfEmptyCuRect
        {
            get
            {
                return (byte)(WidthOfEmptyCuRect * 2);
            }
        }

        public byte DoubledWidthOfFilledFpuRect
        {
            get
            {
                return (byte)(WidthOfFilledFpuRect * 2);
            }
        }

        public byte DoubledWidthOfEmptyFpuRect
        {
            get
            {
                return (byte)(WidthOfEmptyFpuRect * 2);
            }
        }

        private byte GetWidthOfFilledRect(float value, float maxValue)
        {
            if (maxValue == 0)
                return 0;
            var multiplier = value / maxValue;
            var width = multiplier * RECT_WIDTH;
            var roundedWidth = (byte)Math.Round(width);
            return roundedWidth;
        }

        private CategoryViewModel FindCategory()
        {
            var result = from viewModel in Categories
                         where viewModel.Id == Product.CategoryId
                         select viewModel;
            return result.FirstOrDefault();
        }

        private void SetCategory(CategoryViewModel value)
        {
            var oldCategory = Product.CategoryId;
            Product.CategoryId = value.Id;
            MaxCuAndFpu.ResetCategory(oldCategory);
            InvalidateCuAndFpu();
            OnPropertyChanged("Category");
        }

        private void InvalidateCuAndFpu()
        {
            MaxCuAndFpu.ResetCategory(Product.CategoryId);
            OnPropertyChanged("WidthOfFilledCuRect");
            OnPropertyChanged("WidthOfEmptyCuRect");
            OnPropertyChanged("WidthOfFilledFpuRect");
            OnPropertyChanged("WidthOfEmptyFpuRect");
            OnPropertyChanged("DoubledWidthOfFilledCuRect");
            OnPropertyChanged("DoubledWidthOfEmptyCuRect");
            OnPropertyChanged("DoubledWidthOfFilledFpuRect");
            OnPropertyChanged("DoubledWidthOfEmptyFpuRect");
            OnPropertyChanged("CuPer100g");
            OnPropertyChanged("FpuPer100g");
        }

        private void AutoCalculateEnergyPer100g()
        {
            if (autoCalculatingEnergyPer100g)
            {
                var result = Product.CalculatedEnergyPer100g;
                EnergyPer100g = result.ToString();
                autoCalculatingEnergyPer100g = true;
            }
        }

        private void AutoCalculateEnergyPerServing()
        {
            if (autoCalculatingEnergyPerServing)
            {
                var result = Product.CalculatedEnergyPerServing;
                EnergyPerServing = result.ToString();
                autoCalculatingEnergyPerServing = true;
            }
        }
    }

    public class MaxCuAndFpuInCategories
    {
        private Finder finder;
        private Product replacement;
        private Dictionary<Guid, CuAndFpu> values = new Dictionary<Guid, CuAndFpu>();
        private Guid categoryId;
        private List<Product> productsInCategory;

        public MaxCuAndFpuInCategories(Finder finder)
        {
            this.finder = finder;
        }

        public MaxCuAndFpuInCategories(Finder finder, Product replacement)
        {
            this.finder = finder;
            this.replacement = replacement;
        }

        public void Reset()
        {
            values.Clear();
        }

        public void ResetCategory(Guid categoryId)
        {
            if (values.ContainsKey(categoryId))
            {
                values.Remove(categoryId);
            }
        }

        public CuAndFpu Get(Guid categoryId)
        {
            CuAndFpu result;
            if (values.TryGetValue(categoryId, out result))
            {
                return result;
            }
            else
            {
                this.categoryId = categoryId;
                result = CalculateCategory();
                values.Add(categoryId, result);
                return result;
            }
        }

        private CuAndFpu CalculateCategory()
        {
            productsInCategory = finder.FindProductsByCategory(categoryId);
            ReplaceProductWithReplacement();
            var maxCuPer100g = productsInCategory.Max(product => product.CuPer100g);
            var maxFpuPer100g = productsInCategory.Max(product => product.FpuPer100g);
            return new CuAndFpu()
            {
                CuPer100g = maxCuPer100g,
                FpuPer100g = maxFpuPer100g
            };
        }

        private void ReplaceProductWithReplacement()
        {
            if (replacement != null)
            {
                DeleteReplaced();
                AddReplacement();
            }
        }

        private void DeleteReplaced()
        {
            var replaced = productsInCategory.FindById(replacement.Id);
            if (replaced != null)
            {
                productsInCategory.Remove(replaced);
            }
        }

        private void AddReplacement()
        {
            if (replacement.CategoryId == categoryId)
            {
                productsInCategory.Add(replacement);
            }
        }
    }

    public class CuAndFpu
    {
        public float CuPer100g { get; set; }
        public float FpuPer100g { get; set; }
    }
}
