// Uwaga: włączenie property z grupy Score* może automatycznie wyłączyć inne property z tej grupy.
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
        private const byte MAX_SCORES = 4;

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

        public bool ScoreEnergy
        {
            get
            {
                return settings.ScoreEnergy;
            }
            set
            {
                if (settings.ScoreEnergy != value)
                {
                    settings.ScoreEnergy = value;
                    OnPropertyChanged("ScoreEnergy");
                    if (value)
                    {
                        DisableFpuAndCuIfTooManyScores();
                    }
                }
            }
        }

        public bool ScoreProtein
        {
            get
            {
                return settings.ScoreProtein;
            }
            set
            {
                if (settings.ScoreProtein != value)
                {
                    settings.ScoreProtein = value;
                    OnPropertyChanged("ScoreProtein");
                    if (value)
                    {
                        DisableFpuAndCuIfTooManyScores();
                    }
                }
            }
        }

        public bool ScoreDigestibleCarbs
        {
            get
            {
                return settings.ScoreDigestibleCarbs;
            }
            set
            {
                if (settings.ScoreDigestibleCarbs != value)
                {
                    settings.ScoreDigestibleCarbs = value;
                    OnPropertyChanged("ScoreDigestibleCarbs");
                    if (value)
                    {
                        DisableFpuAndCuIfTooManyScores();
                    }
                }
            }
        }

        public bool ScoreFat
        {
            get
            {
                return settings.ScoreFat;
            }
            set
            {
                if (settings.ScoreFat != value)
                {
                    settings.ScoreFat = value;
                    OnPropertyChanged("ScoreFat");
                    if (value)
                    {
                        DisableFpuAndCuIfTooManyScores();
                    }
                }
            }
        }

        public bool ScoreCu
        {
            get
            {
                return settings.ScoreCu;
            }
            set
            {
                if (settings.ScoreCu != value)
                {
                    settings.ScoreCu = value;
                    OnPropertyChanged("ScoreCu");
                    if (value)
                    {
                        DisableNutrientsIfTooManyScores();
                    }
                }
            }
        }

        public bool ScoreFpu
        {
            get
            {
                return settings.ScoreFpu;
            }
            set
            {
                if (settings.ScoreFpu != value)
                {
                    settings.ScoreFpu = value;
                    OnPropertyChanged("ScoreFpu");
                    if (value)
                    {
                        DisableNutrientsIfTooManyScores();
                    }
                }
            }
        }

        private void DisableFpuAndCuIfTooManyScores()
        {
            if (IsTooManyScores)
            {
                ScoreFpu = false;
            }
            if (IsTooManyScores)
            {
                ScoreCu = false;
            }
        }

        private void DisableNutrientsIfTooManyScores()
        {
            if (IsTooManyScores)
            {
                ScoreProtein = false;
            }
            if (IsTooManyScores)
            {
                ScoreDigestibleCarbs = false;
            }
            if (IsTooManyScores)
            {
                ScoreFat = false;
            }
        }

        private bool IsTooManyScores
        {
            get
            {
                var scoresCount = Convert.ToByte(ScoreEnergy) + Convert.ToByte(ScoreProtein) +
                    Convert.ToByte(ScoreDigestibleCarbs) + Convert.ToByte(ScoreFat) +
                    Convert.ToByte(ScoreCu) + Convert.ToByte(ScoreFpu);
                return scoresCount > MAX_SCORES;
            }
        }
    }
}
