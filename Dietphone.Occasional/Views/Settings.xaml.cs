using System.Windows;
using Microsoft.Phone.Controls;
using Dietphone.ViewModels;

namespace Dietphone.Views
{
    public partial class Settings : PhoneApplicationPage
    {
        private SettingsViewModel viewModel;

        public Settings()
        {
            InitializeComponent();
            viewModel = new SettingsViewModel(MyApp.Factories);
            DataContext = viewModel;
        }

        private void LearnCuFpu_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(@"WW czyli wymiennik węglowodanowy to ilość węglowodanów przyswajalnych w gramach podzielona przez 10.
Został wprowadzony, żeby łatwiej liczyło się węglowodany w posiłku.

WBT czyli wymiennik białkowo-tłuszczowy to wartość energetyczna (kcal) pochodząca z białka i tłuszczu podzielona przez 100.
Pozwala w jednej poręcznej liczbie określić zawartość białka i tłuszczu w posiłku.");
        }
    }
}