using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using Dietphone.Models;

namespace Dietphone.ViewModels
{
    public class MealViewModel : ViewModelBase
    {
        private readonly Meal meal;

        public MealViewModel(Meal meal)
        {
            if (meal == null)
            {
                throw new NullReferenceException("meal");
            }
            this.meal = meal;
        }

        public Meal Meal
        {
            get
            {
                return meal;
            }
        }

        public DateTime Date
        {
            get
            {
                return meal.Date;
            }
            set
            {
                if (value != meal.Date)
                {
                    meal.Date = value;
                    OnPropertyChanged("Date");
                }
            }
        }

        public float CU
        {
            get
            {
                return meal.CU;
            }
            set
            {
                if (value != meal.CU)
                {
                    meal.CU = value;
                    OnPropertyChanged("CU");
                }
            }
        }

        public float FPU
        {
            get
            {
                return meal.FPU;
            }
            set
            {
                if (value != meal.FPU)
                {
                    meal.FPU = value;
                    OnPropertyChanged("FPU");
                }
            }
        }

        public short Energy
        {
            get
            {
                return meal.Energy;
            }
            set
            {
                if (value != meal.Energy)
                {
                    meal.Energy = value;
                    OnPropertyChanged("Energy");
                }
            }
        }

        public string Description
        {
            get
            {
                return meal.Description;
            }
            set
            {
                if (value != meal.Description)
                {
                    meal.Description = value;
                    OnPropertyChanged("Description");
                }
            }
        }
    }
}