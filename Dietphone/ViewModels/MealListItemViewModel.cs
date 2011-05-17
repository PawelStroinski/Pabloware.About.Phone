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
using Dietphone.Models;
using Telerik.Windows.Controls;

namespace Dietphone.ViewModels
{
    public class MealListItemViewModel : LoopingListDataItem
    {
        private string calendarDate;
        private string dayOfWeek;
        private string time;
        private string cu;
        private string fpu;
        private string energy;
        private string description;
        private string summary;

        public void LoadMeal(Meal meal)
        {
            var roundedCu = Math.Round(meal.Cu, 1);
            var roundedFpu = Math.Round(meal.Fpu, 1);
            this.CalendarDate = meal.Date.ToString("dd.MM.yyyy");
            this.DayOfWeek = meal.Date.ToString("ddd");
            this.Time = meal.Date.ToString("HH:mm");
            this.CU = roundedCu.ToString() + " WW";
            this.FPU = roundedFpu.ToString() + " WBT";
            this.Energy = meal.Energy.ToString("0 kcal");
            this.Description = "meal.Name";
            this.Summary = MakeSummary(meal);
        }

        public string CalendarDate
        {
            get
            {
                return this.calendarDate;
            }
            set
            {
                if (this.calendarDate == value)
                {
                    return;
                }
                this.calendarDate = value;
                this.OnPropertyChanged("CalendarDate");
            }
        }

        public string DayOfWeek
        {
            get
            {
                return this.dayOfWeek;
            }
            set
            {
                if (this.dayOfWeek == value)
                {
                    return;
                }
                this.dayOfWeek = value;
                this.OnPropertyChanged("DayOfWeek");
            }
        }

        public string Time
        {
            get
            {
                return this.time;
            }
            set
            {
                if (this.time == value)
                {
                    return;
                }
                this.time = value;
                this.OnPropertyChanged("Time");
            }
        }

        public string CU
        {
            get
            {
                return this.cu;
            }
            set
            {
                if (this.cu == value)
                {
                    return;
                }
                this.cu = value;
                this.OnPropertyChanged("CU");
            }
        }

        public string FPU
        {
            get
            {
                return this.fpu;
            }
            set
            {
                if (this.fpu == value)
                {
                    return;
                }
                this.fpu = value;
                this.OnPropertyChanged("FPU");
            }
        }

        public string Energy
        {
            get
            {
                return this.energy;
            }
            set
            {
                if (this.energy == value)
                {
                    return;
                }
                this.energy = value;
                this.OnPropertyChanged("Energy");
            }
        }

        public string Description
        {
            get
            {
                return this.description;
            }
            set
            {
                if (this.description == value)
                {
                    return;
                }
                this.description = value;
                this.OnPropertyChanged("Description");
            }
        }

        public string Summary
        {
            get
            {
                return this.summary;
            }
            set
            {
                if (this.summary == value)
                {
                    return;
                }
                this.summary = value;
                this.OnPropertyChanged("Summary");
            }
        }

        private string MakeSummary(Meal meal)
        {
            switch (meal.Date.Hour)
            {
                case 14:
                    return "Pizza z pieczarkami i cebulą";
                case 19:
                    return "Surówka z kapusty pekińskiej, jabłek i papryki z olejem";
                default:
                    return "Chleb zwykły, masło, ser gouda, keczup";
            }
        }
    }
}