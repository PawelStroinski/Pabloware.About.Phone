using System;
using System.Collections.Generic;
using Dietphone.Tools;
using Dietphone.Views;

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
        public bool AddedByUser { get; set; }
        private const byte ENERGY_DIFF_TOLERANCE = 10;
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
                                      ValidateFiber(), ValidateServingPresence(), ValidateServingSizeUnit(), ValidateServingNutrients() };
            return validation.JoinOptionalSentences();
        }

        private string ValidateNutrientsPer100gPresence()
        {
            if (!AnyNutrientsPer100gPresent)
            {
                return Translations.NotSpecifiedNutritionalValuePer100g;
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
                return String.Format(Translations.In100gOfProductProbablyShouldBeCaloriesButIs,
                    calculated, ENERGY_DIFF_TOLERANCE, typed);
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
                return String.Format(Translations.InServingOfProductProbablyShouldBeCaloriesButIs,
                    calculated, ENERGY_DIFF_TOLERANCE, typed);
            }
            return string.Empty;
        }

        private string ValidateFiber()
        {
            if (FiberPer100g > CarbsTotalPer100g || FiberPerServing > CarbsTotalPerServing)
            {
                return Translations.ThereMayNotBeMoreDietaryFiberThanCarbohydrates;
            }
            return string.Empty;
        }

        private string ValidateServingPresence()
        {
            var descriptionPresent = !string.IsNullOrEmpty(ServingSizeDescription);
            var sizePresent = ServingSizeValue != 0;
            if (descriptionPresent & !sizePresent)
            {
                return Translations.SpecifiedDescriptionOfServingSizeButNotTheSize;
            }
            if (sizePresent & !descriptionPresent)
            {
                return Translations.SpecifiedServingSizeButNotDescriptionOfServingSize;
            }
            var sizeInGrams = ServingSizeUnit == Unit.Gram;
            if (descriptionPresent & !sizeInGrams & !AnyNutrientsPerServingPresent)
            {
                return Translations.ServingSizeInADifferentUnitThanGramsIsSpecifiedBut;
            }
            if (AnyNutrientsPerServingPresent & !descriptionPresent)
            {
                return Translations.NutritionalValuePerServingSizeIsSpecifiedButDesc;
            }
            return string.Empty;
        }

        private string ValidateServingSizeUnit()
        {
            if (ServingSizeUnit == Unit.ServingSize)
            {
                return Translations.CantUseServingSizeAsAUnitOfSizeOfServingSize;
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
                    return Translations.TheQuantityOfCaloriesInAServingSizeOfTheProductIs;
                }
                if (!IsServingNutrientProportional(ProteinPer100g, ProteinPerServing))
                {
                    return Translations.TheQuantityOfProteinInAServingSizeOfTheProductIs;
                }
                if (!IsServingNutrientProportional(FatPer100g, FatPerServing))
                {
                    return Translations.TheQuantityOfFatInAServingSizeOfTheProductIsNot;
                }
                if (!IsServingNutrientProportional(CarbsTotalPer100g, CarbsTotalPerServing))
                {
                    return Translations.TheQuantityOfTotalCarbohydratesInAServingSizeOfThe;
                }
                if (!IsServingNutrientProportional(FiberPer100g, FiberPerServing))
                {
                    return Translations.TheQuantityOfDietaryFiberInAServingSizeOfThe;
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
        public static List<string> GetAbbreviationsFiltered(Func<Unit, bool> filterIn)
        {
            var abbreviations = new List<string>();
            var units = MyEnum.GetValues<Unit>();
            foreach (var unit in units)
            {
                if (filterIn(unit))
                {
                    abbreviations.Add(unit.GetAbbreviation());
                }
            }
            return abbreviations;
        }

        public static List<string> GetAbbreviationsOrServingSizeDetalisFiltered(Func<Unit, bool> filterIn,
            Product servingSizeInfo)
        {
            var abbreviations = new List<string>();
            var units = MyEnum.GetValues<Unit>();
            foreach (var unit in units)
            {
                if (filterIn(unit))
                {
                    abbreviations.Add(unit.GetAbbreviationOrServingSizeDetalis(servingSizeInfo));
                }
            }
            return abbreviations;
        }

        public static Unit TryGetValueOfAbbreviationOrServingSizeDetalis(this Unit caller, string abbreviation,
            Product servingSizeInfo)
        {
            var servingSizeDetalisOfServingSize = Unit.ServingSize.
                GetAbbreviationOrServingSizeDetalis(servingSizeInfo);
            if (abbreviation == servingSizeDetalisOfServingSize)
            {
                return Unit.ServingSize;
            }
            return caller.TryGetValueOfAbbreviation(abbreviation);
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

        public static string GetAbbreviationOrServingSizeDetalis(this Unit unit, Product servingSizeInfo)
        {
            if (unit == Unit.ServingSize)
            {
                var desc = GetAbbreviationOrServingSizeDesc(unit, servingSizeInfo);
                var value = servingSizeInfo.ServingSizeValue;
                var valueUnit = servingSizeInfo.ServingSizeUnit;
                return string.Format("{0} {1} {2}", desc, value, valueUnit.GetAbbreviation());
            }
            return unit.GetAbbreviation();
        }

        public static string GetAbbreviationOrServingSizeDesc(this Unit unit, Product servingSizeInfo)
        {
            var desc = servingSizeInfo.ServingSizeDescription;
            var descPresent = !string.IsNullOrEmpty(desc);
            if (unit == Unit.ServingSize && descPresent)
            {
                return desc;
            }
            return unit.GetAbbreviation();
        }

        public static string GetAbbreviation(this Unit unit)
        {
            switch (unit)
            {
                case Unit.Gram:
                    return "g";
                case Unit.Mililiter:
                    return "ml";
                case Unit.ServingSize:
                    return Translations.Serving;
                default:
                    return string.Empty;
            }
        }
    }

    public enum Unit
    {
        Gram,
        Mililiter,
        ServingSize
    }
}
