using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using Dietphone.Tools;

namespace Dietphone.Models
{
    public sealed class Product : EntityWithId
    {
        public string Name { get; set; }
        public Guid CategoryId { get; set; }
        public float ServingSizeValue { get; set; }
        public Unit ServingSizeUnit { get; set; }
        public string ServingSizeDescription { get; set; }
        public short EnergyPer100g { get; set; }
        public short EnergyPerServing { get; set; }
        public float ProteinPer100g { get; set; }
        public float ProteinPerServing { get; set; }
        public float FatPer100g { get; set; }
        public float FatPerServing { get; set; }
        public float CarbsTotalPer100g { get; set; }
        public float CarbsTotalPerServing { get; set; }
        public float FiberPer100g { get; set; }
        public float FiberPerServing { get; set; }
        private const byte ENERGY_DIFF_TOLERANCE = 5;
        private const byte NUTRIENT_PROP_TOLERANCE = 1;

        public float DigestibleCarbsPer100g
        {
            get
            {
                var digestible = CarbsTotalPer100g - FiberPer100g;
                if (digestible < 0)
                {
                    digestible = 0;
                }
                return digestible;
            }
        }

        public float DigestibleCarbsPerServing
        {
            get
            {
                var digestible = CarbsTotalPerServing - FiberPerServing;
                if (digestible < 0)
                {
                    digestible = 0;
                }
                return digestible;
            }
        }

        public short CalculatedEnergyPer100g
        {
            get
            {
                var calculator = new Calculator()
                {
                    Protein = ProteinPer100g,
                    Fat = FatPer100g,
                    DigestibleCarbs = DigestibleCarbsPer100g
                };
                return calculator.Energy;
            }
        }

        public short CalculatedEnergyPerServing
        {
            get
            {
                var calculator = new Calculator()
                {
                    Protein = ProteinPerServing,
                    Fat = FatPerServing,
                    DigestibleCarbs = DigestibleCarbsPerServing
                };
                return calculator.Energy;
            }
        }

        public float CuPer100g
        {
            get
            {
                var calculator = new Calculator()
                {
                    DigestibleCarbs = DigestibleCarbsPer100g
                };
                return calculator.Cu;
            }
        }

        public float FpuPer100g
        {
            get
            {
                var calculator = new Calculator()
                {
                    Protein = ProteinPer100g,
                    Fat = FatPer100g
                };
                return calculator.Fpu;
            }
        }

        public Category Category
        {
            get
            {
                return Finder.FindCategoryById(CategoryId);
            }
        }

        public bool AnyNutrientsPer100gPresent
        {
            get
            {
                return EnergyPer100g != 0 || ProteinPer100g != 0 || FatPer100g != 0 ||
                    CarbsTotalPer100g != 0 || FiberPer100g != 0;
            }
        }

        public bool AnyNutrientsPerServingPresent
        {
            get
            {
                return EnergyPerServing != 0 || ProteinPerServing != 0 || FatPerServing != 0 ||
                    CarbsTotalPerServing != 0 || FiberPerServing != 0;
            }
        }

        public string Validate()
        {
            string[] validation = { ValidateNutrientsPer100gPresence(), ValidateEnergyPer100g(), ValidateEnergyPerServing(), 
                                      ValidateFiber(), ValidateServingPresence(), ValidateServingNutrients() };
            return validation.ContactOptionalSentences();
        }

        private string ValidateNutrientsPer100gPresence()
        {
            if (!AnyNutrientsPer100gPresent)
            {
                return "Nie podano żadnych wartości odżywczych w 100 g.";
            }
            return string.Empty;
        }

        private string ValidateEnergyPer100g()
        {
            var typed = EnergyPer100g;
            var calculated = CalculatedEnergyPer100g;
            var diff = Math.Abs(typed - calculated);
            if (diff > ENERGY_DIFF_TOLERANCE)
            {
                return String.Format("W 100 g produktu prawdopodobnie powinno być {0} kcal (+/-{1} kcal) a jest {2} kcal.", calculated, ENERGY_DIFF_TOLERANCE, typed);
            }
            return string.Empty;
        }

        private string ValidateEnergyPerServing()
        {
            var typed = EnergyPerServing;
            var calculated = CalculatedEnergyPerServing;
            var diff = Math.Abs(typed - calculated);
            if (diff > ENERGY_DIFF_TOLERANCE)
            {
                return String.Format("W porcji produktu prawdopodobnie powinno być {0} kcal (+/-{1} kcal) a jest {2} kcal.", calculated, ENERGY_DIFF_TOLERANCE, typed);
            }
            return string.Empty;
        }

        private string ValidateFiber()
        {
            if (FiberPer100g > CarbsTotalPer100g || FiberPerServing > CarbsTotalPerServing)
            {
                return "Nie może być więcej błonnika niż węglowodanów ogółem.";
            }
            return string.Empty;
        }

        private string ValidateServingPresence()
        {
            var descriptionPresent = !string.IsNullOrEmpty(ServingSizeDescription);
            var sizePresent = ServingSizeValue != 0;
            if (descriptionPresent & !sizePresent)
            {
                return "Podano opis porcji ale nie podano jej miary.";
            }
            if (sizePresent & !descriptionPresent)
            {
                return "Podano miarę porcji ale nie podano jej opisu.";
            }
            var sizeInGrams = ServingSizeUnit == Unit.Gram;
            if (descriptionPresent & !sizeInGrams & !AnyNutrientsPerServingPresent)
            {
                return "Podano porcję w innej jednostce niż gramy ale nie podano jej wartości odżywczych.";
            }
            if (AnyNutrientsPerServingPresent & !descriptionPresent)
            {
                return "Podano wartości odżywcze porcji ale nie podano jej opisu.";
            }
            return string.Empty;
        }

        private string ValidateServingNutrients()
        {
            var sizePresent = ServingSizeValue != 0;
            var supportedUnits = ServingSizeUnit == Unit.Gram || ServingSizeUnit == Unit.Mililiter;
            if (AnyNutrientsPer100gPresent & AnyNutrientsPerServingPresent & sizePresent & supportedUnits)
            {
                if (!IsServingNutrientProportional(EnergyPer100g, EnergyPerServing))
                {
                    return "Ilość kalorii w porcji produktu nie jest proporcjonalna do ilości w 100 g produktu.";
                }
                if (!IsServingNutrientProportional(ProteinPer100g, ProteinPerServing))
                {
                    return "Ilość białka w porcji produktu nie jest proporcjonalna do ilości w 100 g produktu.";
                }
                if (!IsServingNutrientProportional(FatPer100g, FatPerServing))
                {
                    return "Ilość tłuszczu w porcji produktu nie jest proporcjonalna do ilości w 100 g produktu.";
                }
                if (!IsServingNutrientProportional(CarbsTotalPer100g, CarbsTotalPerServing))
                {
                    return "Ilość węglowodanów ogółem w porcji produktu nie jest proporcjonalna do ilości w 100 g produktu.";
                }
                if (!IsServingNutrientProportional(FiberPer100g, FiberPerServing))
                {
                    return "Ilość błonnika pokarmowego w porcji produktu nie jest proporcjonalna do ilości w 100 g produktu.";
                }
            }
            return string.Empty;
        }

        private bool IsServingNutrientProportional(float nutrientPer100g, float nutrientPerServing)
        {
            var multiplier = ServingSizeValue / 100;
            var calculated = Math.Round(nutrientPer100g * multiplier);
            var diff = Math.Abs(nutrientPerServing - calculated);
            return (diff <= NUTRIENT_PROP_TOLERANCE);
        }
    }

    public static class UnitAbbreviations
    {
        public static List<string> GetAll()
        {
            var abbreviations = new List<string>();
            var units = MyEnum.GetValues<Unit>();
            foreach (var unit in units)
            {
                abbreviations.Add(unit.GetAbbreviation());
            }
            return abbreviations;
        }

        public static Unit TryGetValueOfAbbreviation(this Unit caller, string abbreviation)
        {
            var units = MyEnum.GetValues<Unit>();
            foreach (var unit in units)
            {
                if (abbreviation == unit.GetAbbreviation())
                {
                    return unit;
                }
            }
            return caller;
        }

        public static string GetAbbreviation(this Unit unit)
        {
            switch (unit)
            {
                case Unit.Gram:
                    return "g";
                case Unit.Mililiter:
                    return "ml";
                default:
                    return string.Empty;
            }
        }
    }

    public enum Unit
    {
        Gram,
        Mililiter
    }
}
