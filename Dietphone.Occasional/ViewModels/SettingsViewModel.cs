// Uwaga: włączenie property z grupy Calculate* może automatycznie wyłączyć inne property z tej grupy.
using System;
using System.Collections.Generic;
using Dietphone.Models;

namespace Dietphone.ViewModels
{
    public class SettingsViewModel : ViewModelBase
    {
        public List<string> Languages { get; private set; }
        public List<string> ProductLocalisations { get; private set; }
        private readonly Settings settings;
        private const byte CALCULATE_MAX_ITEMS = 4;

        public SettingsViewModel(Factories factories)
        {
            settings = factories.Settings;
            Languages = new List<string>();
            ProductLocalisations = new List<string>();
            Languages.Add("polski");
            Languages.Add("angielski");
            ProductLocalisations.Add("polski (Polska)");
            ProductLocalisations.Add("angielski (Stany Zjednoczone)");
        }

        public bool CalculateEnergy
        {
            get
            {
                return settings.CalculateEnergy;
            }
            set
            {
                if (settings.CalculateEnergy != value)
                {
                    settings.CalculateEnergy = value;
                    OnPropertyChanged("CalculateEnergy");
                    if (value)
                    {
                        DisableFpuAndCuIfCalculateTooMany();
                    }
                }
            }
        }

        public bool CalculateProteinInGrams
        {
            get
            {
                return settings.CalculateProteinInGrams;
            }
            set
            {
                if (settings.CalculateProteinInGrams != value)
                {
                    settings.CalculateProteinInGrams = value;
                    OnPropertyChanged("CalculateProteinInGrams");
                    if (value)
                    {
                        DisableFpuAndCuIfCalculateTooMany();
                    }
                }
            }
        }

        public bool CalculateDigestibleCarbsInGrams
        {
            get
            {
                return settings.CalculateDigestibleCarbsInGrams;
            }
            set
            {
                if (settings.CalculateDigestibleCarbsInGrams != value)
                {
                    settings.CalculateDigestibleCarbsInGrams = value;
                    OnPropertyChanged("CalculateDigestibleCarbsInGrams");
                    if (value)
                    {
                        DisableFpuAndCuIfCalculateTooMany();
                    }
                }
            }
        }

        public bool CalculateFatInGrams
        {
            get
            {
                return settings.CalculateFatInGrams;
            }
            set
            {
                if (settings.CalculateFatInGrams != value)
                {
                    settings.CalculateFatInGrams = value;
                    OnPropertyChanged("CalculateFatInGrams");
                    if (value)
                    {
                        DisableFpuAndCuIfCalculateTooMany();
                    }
                }
            }
        }

        public bool CalculateCu
        {
            get
            {
                return settings.CalculateCu;
            }
            set
            {
                if (settings.CalculateCu != value)
                {
                    settings.CalculateCu = value;
                    OnPropertyChanged("CalculateCu");
                    if (value)
                    {
                        DisableNutrientsIfCalculateTooMany();
                    }
                }
            }
        }

        public bool CalculateFpu
        {
            get
            {
                return settings.CalculateFpu;
            }
            set
            {
                if (settings.CalculateFpu != value)
                {
                    settings.CalculateFpu = value;
                    OnPropertyChanged("CalculateFpu");
                    if (value)
                    {
                        DisableNutrientsIfCalculateTooMany();
                    }
                }
            }
        }

        private void DisableFpuAndCuIfCalculateTooMany()
        {
            if (IsCalculateTooMany)
            {
                CalculateFpu = false;
            }
            if (IsCalculateTooMany)
            {
                CalculateCu = false;
            }
        }

        private void DisableNutrientsIfCalculateTooMany()
        {
            if (IsCalculateTooMany)
            {
                CalculateProteinInGrams = false;
            }
            if (IsCalculateTooMany)
            {
                CalculateDigestibleCarbsInGrams = false;
            }
            if (IsCalculateTooMany)
            {
                CalculateFatInGrams = false;
            }
        }

        private bool IsCalculateTooMany
        {
            get
            {
                var count = Convert.ToByte(CalculateEnergy) + Convert.ToByte(CalculateProteinInGrams) +
                    Convert.ToByte(CalculateDigestibleCarbsInGrams) + Convert.ToByte(CalculateFatInGrams) +
                    Convert.ToByte(CalculateCu) + Convert.ToByte(CalculateFpu);
                return count > CALCULATE_MAX_ITEMS;
            }
        }
    }
}
