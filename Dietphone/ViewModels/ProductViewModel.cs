using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using Dietphone.Models;
using System.Collections.ObjectModel;

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
                Product.Name = value;
                OnPropertyChanged("Name");
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
                var oldCategory = Product.CategoryId;
                Product.CategoryId = value.Id;
                maxNutritives.ResetCategory(oldCategory);
                InvalidateMaxNutritives();
                OnPropertyChanged("Category");
            }
        }

        public float ServingSizeValue
        {
            get
            {
                return Product.ServingSizeValue;
            }
            set
            {
                Product.ServingSizeValue = value;
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

        public short EnergyPer100g
        {
            get
            {
                return Product.EnergyPer100g;
            }
            set
            {
                Product.EnergyPer100g = value;
                OnPropertyChanged("EnergyPer100g");
            }
        }

        public short EnergyPerServing
        {
            get
            {
                return Product.EnergyPerServing;
            }
            set
            {
                Product.EnergyPerServing = value;
                OnPropertyChanged("EnergyPerServing");
            }
        }

        public float ProteinPer100g
        {
            get
            {
                return Product.ProteinPer100g;
            }
            set
            {
                Product.ProteinPer100g = value;
                InvalidateMaxNutritives();
                OnPropertyChanged("ProteinPer100g");
            }
        }

        public float ProteinPerServing
        {
            get
            {
                return Product.ProteinPerServing;
            }
            set
            {
                Product.ProteinPerServing = value;
                OnPropertyChanged("ProteinPerServing");
            }
        }

        public float FatPer100g
        {
            get
            {
                return Product.FatPer100g;
            }
            set
            {
                Product.FatPer100g = value;
                InvalidateMaxNutritives();
                OnPropertyChanged("FatPer100g");
            }
        }

        public float FatPerServing
        {
            get
            {
                return Product.FatPerServing;
            }
            set
            {
                Product.FatPerServing = value;
                OnPropertyChanged("FatPerServing");
            }
        }

        public float CarbsTotalPer100g
        {
            get
            {
                return Product.CarbsTotalPer100g;
            }
            set
            {
                Product.CarbsTotalPer100g = value;
                InvalidateMaxNutritives();
                OnPropertyChanged("CarbsTotalPer100g");
            }
        }

        public float CarbsTotalPerServing
        {
            get
            {
                return Product.CarbsTotalPerServing;
            }
            set
            {
                Product.CarbsTotalPerServing = value;
                OnPropertyChanged("CarbsTotalPerServing");
            }
        }

        public float FiberPer100g
        {
            get
            {
                return Product.FiberPer100g;
            }
            set
            {
                Product.FiberPer100g = value;
                InvalidateMaxNutritives();
                OnPropertyChanged("FiberPer100g");
            }
        }

        public float FiberPerServing
        {
            get
            {
                return Product.FiberPerServing;
            }
            set
            {
                Product.FiberPerServing = value;
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
