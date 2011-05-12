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
        public Collection<CategoryViewModel> Categories { private get; set; }
        private readonly MaxNutritivesInCategories maxNutritives;
        private const byte RECT_WIDTH = 25;

        public ProductViewModel(Product product, MaxNutritivesInCategories maxNutritives)
        {
            Product = product;
            this.maxNutritives = maxNutritives;
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
                if (Categories == null)
                {
                    throw new InvalidOperationException("Set Categories first.");
                }
                return GetCategory();
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
                var old = Product.ServingSizeValue;
                Product.ServingSizeValue = old.TryGetValueOf(value);
                OnPropertyChanged("ServingSizeValue");
            }
        }

        public Unit ServingSizeUnit
        {
            get
            {
                return Product.ServingSizeUnit;
            }
            set
            {
                Product.ServingSizeUnit = value;
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
                var old = Product.EnergyPer100g;
                Product.EnergyPer100g = old.TryGetValueOf(value);
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
                var old = Product.EnergyPerServing;
                Product.EnergyPerServing = old.TryGetValueOf(value);
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
                var old = Product.ProteinPer100g;
                Product.ProteinPer100g = old.TryGetValueOf(value);
                InvalidateMaxNutritives();
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
                var old = Product.ProteinPerServing;
                Product.ProteinPerServing = old.TryGetValueOf(value);
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
                var old = Product.FatPer100g;
                Product.FatPer100g = old.TryGetValueOf(value);
                InvalidateMaxNutritives();
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
                var old = Product.FatPerServing;
                Product.FatPerServing = old.TryGetValueOf(value);
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
                var old = Product.CarbsTotalPer100g;
                Product.CarbsTotalPer100g = old.TryGetValueOf(value);
                InvalidateMaxNutritives();
                OnPropertyChanged("CarbsTotalPer100g");
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
                var old = Product.CarbsTotalPerServing;
                Product.CarbsTotalPerServing = old.TryGetValueOf(value);
                OnPropertyChanged("CarbsTotalPerServing");
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
                var old = Product.FiberPer100g;
                Product.FiberPer100g = old.TryGetValueOf(value);
                InvalidateMaxNutritives();
                OnPropertyChanged("FiberPer100g");
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
                var old = Product.FiberPerServing;
                Product.FiberPerServing = old.TryGetValueOf(value);
                OnPropertyChanged("FiberPerServing");
            }
        }

        public byte WidthOfFilledCuRect
        {
            get
            {
                var nutritives = maxNutritives.Get(Product.CategoryId);
                return GetWidthOfFilledRect(Product.CuPer100g, nutritives.CuPer100g);
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
                var nutritives = maxNutritives.Get(Product.CategoryId);
                return GetWidthOfFilledRect(Product.FpuPer100g, nutritives.FpuPer100g);
            }
        }

        public byte WidthOfEmptyFpuRect
        {
            get
            {
                return (byte)(RECT_WIDTH - WidthOfFilledFpuRect);
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

        private CategoryViewModel GetCategory()
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
            maxNutritives.ResetCategory(oldCategory);
            InvalidateMaxNutritives();
            OnPropertyChanged("Category");
        }

        private void InvalidateMaxNutritives()
        {
            maxNutritives.ResetCategory(Product.CategoryId);
            OnPropertyChanged("WidthOfFilledCuRect");
            OnPropertyChanged("WidthOfEmptyCuRect");
            OnPropertyChanged("WidthOfFilledFpuRect");
            OnPropertyChanged("WidthOfEmptyFpuRect");
        }
    }

    public class MaxNutritivesInCategories
    {
        private Finder finder;
        private Dictionary<Guid, Nutritives> nutritives = new Dictionary<Guid, Nutritives>();

        public MaxNutritivesInCategories(Finder finder)
        {
            this.finder = finder;
        }

        public void Reset()
        {
            nutritives.Clear();
        }

        public void ResetCategory(Guid categoryId)
        {
            if (nutritives.ContainsKey(categoryId))
            {
                nutritives.Remove(categoryId);
            }
        }

        public Nutritives Get(Guid categoryId)
        {
            Nutritives result;
            if (nutritives.TryGetValue(categoryId, out result))
            {
                return result;
            }
            else
            {
                result = CalculateCategory(categoryId);
                nutritives.Add(categoryId, result);
                return result;
            }
        }

        private Nutritives CalculateCategory(Guid categoryId)
        {
            var productsInCategory = finder.FindProductsByCategory(categoryId);
            var cus = from product in productsInCategory
                      select product.CuPer100g;
            var fpus = from product in productsInCategory
                       select product.FpuPer100g;
            return new Nutritives() { CuPer100g = cus.Max(), FpuPer100g = fpus.Max() };
        }
    }

    public class Nutritives
    {
        public float CuPer100g { get; set; }
        public float FpuPer100g { get; set; }
    }
}
