using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Dietphone.Tools;

namespace Dietphone.Models
{
    public class MealItemBase : Entity
    {
        public float Value { get; set; }
        public Unit Unit { get; set; }
        private Guid productId;
        private Product foundProduct;
        private bool searchedForProduct;
        private Product emptyProduct;

        public Guid ProductId
        {
            get
            {
                return productId;
            }
            set
            {
                if (productId != value)
                {
                    productId = value;
                    searchedForProduct = false;
                }
            }
        }

        public Product Product
        {
            // Dyskusjne. Być może wyszukiwanie za każdym razem
            // będzie wystarczająco szybkie.
            get
            {
                VerifySearchedForProduct();
                if (!searchedForProduct)
                {
                    foundProduct = Finder.FindProductById(ProductId);
                    searchedForProduct = true;
                }
                if (foundProduct == null)
                {
                    return EmptyProduct;
                }
                else
                {
                    return foundProduct;
                }
            }
        }

        private void VerifySearchedForProduct()
        {
            var canVerify = foundProduct != null;
            if (searchedForProduct && canVerify)
            {
                var products = Owner.Products;
                var removedOrReplaced = !products.Contains(foundProduct);
                if (removedOrReplaced)
                {
                    searchedForProduct = false;
                }
            }
        }

        private Product EmptyProduct
        {
            get
            {
                if (emptyProduct == null)
                {
                    emptyProduct = Owner.CreateProduct();
                    var products = Owner.Products;
                    products.Remove(emptyProduct);
                    emptyProduct.Id = Guid.Empty;
                }
                return emptyProduct;
            }
        }
    }

    public class MealItemWithNutrientsPerUnit : MealItemBase
    {
        private const float BASE_PER_100G = 100;

        public bool AnyNutrientsPerUnitPresent
        {
            get
            {
                return AreNutrientsPer100gUsable || AreNutrientsPerServingUsable;
            }
        }

        protected float EnergyPerUnit
        {
            get
            {
                if (AreNutrientsPer100gUsable)
                {
                    return Product.EnergyPer100g / BASE_PER_100G;
                }
                else
                    if (AreNutrientsPerServingUsable)
                    {
                        return Product.EnergyPerServing / BasePerServing;
                    }
                    else
                    {
                        return 0;
                    }
            }
        }

        protected float ProteinPerUnit
        {
            get
            {
                if (AreNutrientsPer100gUsable)
                {
                    return Product.ProteinPer100g / BASE_PER_100G;
                }
                else
                    if (AreNutrientsPerServingUsable)
                    {
                        return Product.ProteinPerServing / BasePerServing;
                    }
                    else
                    {
                        return 0;
                    }
            }
        }

        protected float FatPerUnit
        {
            get
            {
                if (AreNutrientsPer100gUsable)
                {
                    return Product.FatPer100g / BASE_PER_100G;
                }
                else
                    if (AreNutrientsPerServingUsable)
                    {
                        return Product.FatPerServing / BasePerServing;
                    }
                    else
                    {
                        return 0;
                    }
            }
        }

        protected float DigestibleCarbsPerUnit
        {
            get
            {
                if (AreNutrientsPer100gUsable)
                {
                    return Product.DigestibleCarbsPer100g / BASE_PER_100G;
                }
                else
                    if (AreNutrientsPerServingUsable)
                    {
                        return Product.DigestibleCarbsPerServing / BasePerServing;
                    }
                    else
                    {
                        return 0;
                    }
            }
        }

        private bool AreNutrientsPer100gUsable
        {
            get
            {
                var unitsMatches = Unit == Unit.Gram;
                return unitsMatches && Product.AnyNutrientsPer100gPresent;
            }
        }

        private bool AreNutrientsPerServingUsable
        {
            get
            {
                var unitsMatches = Unit == Product.ServingSizeUnit;
                var sizePresent = Product.ServingSizeValue != 0;
                return unitsMatches && sizePresent && Product.AnyNutrientsPerServingPresent;
            }
        }

        private float BasePerServing
        {
            get
            {
                return Product.ServingSizeValue;
            }
        }
    }

    public class MealItemWithNutrients : MealItemWithNutrientsPerUnit
    {
        public short Energy
        {
            get
            {
                var energy = EnergyPerUnit * Value;
                var roundedEnergy = Math.Round(energy);
                return (short)roundedEnergy;
            }
        }

        public float Protein
        {
            get
            {
                return ProteinPerUnit * Value;
            }
        }

        public float Fat
        {
            get
            {
                return FatPerUnit * Value;
            }
        }

        public float DigestibleCarbs
        {
            get
            {
                return DigestibleCarbsPerUnit * Value;
            }
        }

        public float Cu
        {
            get
            {
                var calculator = new Calculator()
                {
                    DigestibleCarbs = DigestibleCarbs
                };
                return calculator.Cu;
            }
        }

        public float Fpu
        {
            get
            {
                var calculator = new Calculator()
                {
                    Protein = Protein,
                    Fat = Fat
                };
                return calculator.Fpu;
            }
        }
    }

    public class MealItemWithValidation : MealItemWithNutrients
    {
        public string Validate()
        {
            string[] validation = { ValidateProduct(), ValidateValue(), ValidateUnit() };
            return validation.ContactOptionalSentences();
        }

        private string ValidateProduct()
        {
            if (Product.Id == Guid.Empty)
            {
                return "Produkt nie istnieje.";
            }
            return string.Empty;
        }

        private string ValidateValue()
        {
            if (Value == 0)
            {
                return "Nie podano ilości produktu.";
            }
            return string.Empty;
        }

        private string ValidateUnit()
        {
            var canValidate = Product.Id != Guid.Empty;
            if (canValidate && !AnyNutrientsPerUnitPresent)
            {
                var unit = Unit.GetAbbreviation();
                return string.Format("Brak informacji o wartościach odżywczych na jednostkę {0}. "
                    + "Wybierz inną jednostkę.", unit);
            }
            return string.Empty;
        }
    }

    public class MealItem : MealItemWithValidation
    {
    }
}
