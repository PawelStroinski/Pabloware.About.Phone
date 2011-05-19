using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;

namespace Dietphone.Models
{
    public sealed class Meal : EntityWithId
    {
        public DateTime Date { get; set; }
        public Guid NameId { get; set; }
        public string Note { get; set; }
        private List<MealItem> items;

        public ReadOnlyCollection<MealItem> Items
        {
            get
            {
                if (items == null)
                {
                    throw new InvalidOperationException("Call InitializeItems first.");
                }
                return items.AsReadOnly();
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

        public void InitializeItems(List<MealItem> newItems)
        {
            var alreadyInitialized = items != null;
            if (alreadyInitialized)
            {
                throw new InvalidOperationException("Items can only be initialized once.");
            }
            items = newItems;
            AssignOwner();
        }

        public MealItem AddItem()
        {
            var item = new MealItem();
            item.Owner = Owner;
            items.Add(item);
            return item;
        }

        public void DeleteItem(MealItem item)
        {
            items.Remove(item);
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

        private void AssignOwner()
        {
            foreach (var item in items)
            {
                item.Owner = Owner;
            }
        }
    }
}
