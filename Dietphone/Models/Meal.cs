using System;
using System.Collections.Generic;
using System.Linq;

namespace Dietphone.Models
{
    public sealed class Meal : EntityWithId
    {
        public DateTime Date { get; set; }
        public Guid NameId { get; set; }
        public string Note { get; set; }
        public List<MealItem> Items { get; set; }

        public float Cu
        {
            get
            {
                var digestibleCarbsSum = Items.Sum(item => item.DigestibleCarbs);
                var calculator = new Calculator()
                {
                    DigestibleCarbs = digestibleCarbsSum
                };
                return calculator.Cu;
            }
        }

        public float Fpu
        {
            get
            {
                var proteinSum = Items.Sum(item => item.Protein);
                var fatSum = Items.Sum(item => item.Fat);
                var calculator = new Calculator()
                {
                    Protein = proteinSum,
                    Fat = fatSum
                };
                return calculator.Fpu;
            }
        }

        public short Energy
        {
            get
            {
                var energySum = Items.Sum(item => item.Energy);
                return (short)energySum;
            }
        }

        public MealName Name
        {
            get
            {
                return Finder.FindMealNameById(NameId);
            }
        }

        public string Validate()
        {
            return ValidateItems();
        }

        private string ValidateItems()
        {
            var validation = string.Empty;
            foreach (var item in Items)
            {
                var itemValidation = item.Validate();
                if (!string.IsNullOrEmpty(itemValidation))
                {
                    var itemNumber = Items.IndexOf(item) + 1;
                    var itemFormatted = string.Format("Element nr {0} zawiera następujące błędy. {1}\r\n",
                        itemNumber, itemValidation);
                    validation += itemFormatted;
                }
            }
            return validation;
        }
    }
}
