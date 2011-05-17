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
                return meal.Cu;
            }
            set
            {
                if (value != meal.Cu)
                {
                    //meal.Cu = value;
                    OnPropertyChanged("CU");
                }
            }
        }

        public float FPU
        {
            get
            {
                return meal.Fpu;
            }
            set
            {
                if (value != meal.Fpu)
                {
                    //meal.Fpu = value;
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
                    //meal.Energy = value;
                    OnPropertyChanged("Energy");
                }
            }
        }

        public string Description
        {
            get
            {
                return "meal.Name";
            }
            set
            {
                if (value != "meal.Name")
                {
                    //meal.Name = value;
                    OnPropertyChanged("Description");
                }
            }
        }
    }
}