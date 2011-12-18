// Uwaga: włączenie property z grupy Score* może automatycznie wyłączyć inne property z tej grupy.
using System;
using System.Collections.Generic;
using Dietphone.Models;
using System.Globalization;
using System.Windows;
using Dietphone.Tools;

namespace Dietphone.ViewModels
{
    public class SettingsViewModel : PivotTombstoningViewModel
    {
        public List<string> UiCultures { get; private set; }
        public List<string> ProductCultures { get; private set; }
        private readonly Settings settings;
        private const byte MAX_SCORES = 4;

        public SettingsViewModel(Factories factories)
        {
            settings = factories.Settings;
            UiCultures = new List<string>();
            ProductCultures = new List<string>();
            BuildUiCulturesAndProductCultures();
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

        public string UiCulture
        {
            get
            {
                var cultureName = settings.NextUiCulture;
                return GetUiCultureFromCultureName(cultureName);
            }
            set
            {
                var cultureName = FindCultureNameByUiCulture(value);
                if (settings.NextUiCulture != cultureName)
                {
                    settings.NextUiCulture = cultureName;
                    OnPropertyChanged("CultureChanged");
                }
            }
        }

        public string ProductCulture
        {
            get
            {
                var cultureName = settings.NextProductCulture;
                return GetProductCultureFromCultureName(cultureName);
            }
            set
            {
                var cultureName = FindCultureNameByProductCulture(value);
                if (settings.NextProductCulture != cultureName)
                {
                    settings.NextProductCulture = cultureName;
                    OnPropertyChanged("CultureChanged");
                }
            }
        }

        public Visibility CultureChanged
        {
            get
            {
                var uiCultureChanged = settings.CurrentUiCulture != settings.NextUiCulture;
                var productCultureChanged = settings.CurrentProductCulture != settings.NextProductCulture;
                var cultureChanged = uiCultureChanged || productCultureChanged;
                return cultureChanged.ToVisibility();
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

        private void BuildUiCulturesAndProductCultures()
        {
            var cultures = new Cultures();
            foreach (var cultureName in cultures.SupportedCultures)
            {
                var uiCulture = GetUiCultureFromCultureName(cultureName);
                var productCulture = GetProductCultureFromCultureName(cultureName);
                UiCultures.Add(uiCulture);
                ProductCultures.Add(productCulture);
            }
        }

        private string GetUiCultureFromCultureName(string cultureName)
        {
            var result = GetProductCultureFromCultureName(cultureName);
            var bracketPos = result.IndexOf('(');
            if (bracketPos != -1)
            {
                result = result.Remove(bracketPos);
                result = result.Trim();
            }
            return result;
        }

        private string GetProductCultureFromCultureName(string cultureName)
        {
            var culture = new CultureInfo(cultureName);
            return culture.DisplayName;
        }

        private string FindCultureNameByUiCulture(string uiCulture)
        {
            var index = UiCultures.IndexOf(uiCulture);
            var cultures = new Cultures();
            return cultures.SupportedCultures[index];
        }

        private string FindCultureNameByProductCulture(string productCulture)
        {
            var index = ProductCultures.IndexOf(productCulture);
            var cultures = new Cultures();
            return cultures.SupportedCultures[index];
        }
    }
}
