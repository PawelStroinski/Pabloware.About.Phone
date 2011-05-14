using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using Dietphone.Tools;

namespace Dietphone.Models
{
    public enum Unit
    {
        Gram,
        Mililiter
    };

    public class Product : Entity
    {
        public Guid Id { get; set; }
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
        private const byte NUTRITIVE_PROP_TOLERANCE = 1;

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
                return (short)Math.Round(ProteinPer100g * 4 + FatPer100g * 9 + DigestibleCarbsPer100g * 4);
            }
        }

        public short CalculatedEnergyPerServing
        {
            get
            {
                return (short)Math.Round(ProteinPerServing * 4 + FatPerServing * 9 + DigestibleCarbsPerServing * 4);
            }
        }

        public float CuPer100g
        {
            get
            {
                var cu = DigestibleCarbsPer100g / 10.0;
                var roundedCu = Math.Round(cu, 1);
                return (float)roundedCu;
            }
        }

        public float FpuPer100g
        {
            get
            {
                var fpuEnergy = ProteinPer100g * 4 + FatPer100g * 9;
                var fpu = fpuEnergy / 100.0;
                var roundedFpu = Math.Round(fpu, 1);
                return (float)roundedFpu;
            }
        }

        public Category Category
        {
            get
            {
                var result = from category in Owner.Categories
                             where category.Id == CategoryId
                             select category;
                return result.FirstOrDefault();
            }
        }

        public string Validate()
        {
            string[] validation = { ValidateNutritivePer100gPresence(), ValidateEnergyPer100g(), ValidateEnergyPerServing(), 
                                      ValidateFiber(), ValidateServingPresence(), ValidateServingNutritives() };
            String result = "";
            foreach (var text in validation)
            {
                if (text != "")
                    result += text + " ";
            }
            return result;
        }

        private string ValidateNutritivePer100gPresence()
        {
            if (EnergyPer100g == 0 & ProteinPer100g == 0 & FatPer100g == 0 & CarbsTotalPer100g == 0 & FiberPer100g == 0)
                return "Nie podano żadnych wartości odżywczych w 100 g.";
            return "";
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
            return "";
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
            return "";
        }

        private string ValidateFiber()
        {
            if (FiberPer100g > CarbsTotalPer100g || FiberPerServing > CarbsTotalPerServing)
            {
                return "Nie może być więcej błonnika niż węglowodanów ogółem.";
            }
            return "";
        }

        private string ValidateServingPresence()
        {
            var descriptionPresent = ServingSizeDescription != "";
            var sizePresent = ServingSizeValue != 0;
            if (descriptionPresent & !sizePresent)
            {
                return "Podano opis porcji ale nie podano jej miary.";
            }
            if (sizePresent & !descriptionPresent)
            {
                return "Podano miarę porcji ale nie podano jej opisu.";
            }
            var nutritivePresent = EnergyPerServing != 0 || ProteinPerServing != 0 || FatPerServing != 0 || CarbsTotalPerServing != 0 || FiberPerServing != 0;
            var sizeInGrams = ServingSizeUnit == Unit.Gram;
            if (descriptionPresent & !sizeInGrams & !nutritivePresent)
            {
                return "Podano porcję w innej jednostce niż gramy ale nie podano jej wartości odżywczych.";
            }
            if (nutritivePresent & !descriptionPresent)
            {
                return "Podano wartości odżywcze porcji ale nie podano jej opisu.";
            }
            return "";
        }

        private string ValidateServingNutritives()
        {
            var nutritivePer100gPresent = EnergyPer100g != 0 || ProteinPer100g != 0 || FatPer100g != 0 || CarbsTotalPer100g != 0 || FiberPer100g != 0;
            var nutritivePerServingPresent = EnergyPerServing != 0 || ProteinPerServing != 0 || FatPerServing != 0 || CarbsTotalPerServing != 0 || FiberPerServing != 0;
            var sizePresent = ServingSizeValue != 0;
            var supportedUnits = ServingSizeUnit == Unit.Gram || ServingSizeUnit == Unit.Mililiter;
            if (nutritivePer100gPresent & nutritivePerServingPresent & sizePresent & supportedUnits)
            {
                if (!IsServingNutritiveProportional(EnergyPer100g, EnergyPerServing))
                {
                    return "Ilość kalorii w porcji produktu nie jest proporcjonalna do ilości w 100 g produktu.";
                }
                if (!IsServingNutritiveProportional(ProteinPer100g, ProteinPerServing))
                {
                    return "Ilość białka w porcji produktu nie jest proporcjonalna do ilości w 100 g produktu.";
                }
                if (!IsServingNutritiveProportional(FatPer100g, FatPerServing))
                {
                    return "Ilość tłuszczu w porcji produktu nie jest proporcjonalna do ilości w 100 g produktu.";
                }
                if (!IsServingNutritiveProportional(CarbsTotalPer100g, CarbsTotalPerServing))
                {
                    return "Ilość węglowodanów ogółem w porcji produktu nie jest proporcjonalna do ilości w 100 g produktu.";
                }
                if (!IsServingNutritiveProportional(FiberPer100g, FiberPerServing))
                {
                    return "Ilość błonnika pokarmowego w porcji produktu nie jest proporcjonalna do ilości w 100 g produktu.";
                }
            }
            return "";
        }

        private bool IsServingNutritiveProportional(float nutritivePer100g, float nutritivePerServing)
        {
            var multiplier = ServingSizeValue / 100;
            var calculated = Math.Round(nutritivePer100g * multiplier);
            var diff = Math.Abs(nutritivePerServing - calculated);
            return (diff <= NUTRITIVE_PROP_TOLERANCE);
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
}
