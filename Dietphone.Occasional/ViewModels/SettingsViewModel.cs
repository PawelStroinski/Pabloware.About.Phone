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
using System.Collections.Generic;
using Dietphone.Models;

namespace Dietphone.ViewModels
{
    public class SettingsViewModel : ViewModelBase
    {
        public List<string> Languages { get; private set; }
        public List<string> ProductLocalisations { get; private set; }
        private readonly Factories factories;

        public SettingsViewModel(Factories factories)
        {
            this.factories = factories;
            Languages = new List<string>();
            ProductLocalisations = new List<string>();
            Languages.Add("polski");
            Languages.Add("angielski");
            ProductLocalisations.Add("polski (Polska)");
            ProductLocalisations.Add("angielski (Stany Zjednoczone)");
        }
    }
}
