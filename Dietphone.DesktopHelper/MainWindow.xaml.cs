using System.Windows;
using Dietphone.Models;
using Dietphone.BinarySerializers;
using System.IO;
using System.Diagnostics;

namespace Dietphone.DesktopHelper
{
    public partial class MainWindow : Window
    {
        private Factories factories;
        private StorageCreator storageCreator;

        public MainWindow()
        {
            InitializeComponent();
            storageCreator = new DesktopBinaryStorageCreator();
            factories = new FactoriesImpl(storageCreator);
        }

        private void Window_Closed(object sender, System.EventArgs e)
        {
            factories.Save();
        }

        private void CreateAndShowDirectory_Click(object sender, RoutedEventArgs e)
        {
            var directory = new DirectoryInfo(DesktopBinaryStreamProvider.DIRECTORY);
            directory.Create();
            Process.Start("explorer.exe", "/select," + DesktopBinaryStreamProvider.DIRECTORY);
        }

        private void InitializeSettings_Click(object sender, RoutedEventArgs e)
        {
            var factoryCreator = new FactoryCreator(factories, storageCreator);
            var settingsFactory = factoryCreator.CreateFactory<Settings>();
            if (settingsFactory.Entities.Count > 0)
            {
                return;
            }
            settingsFactory.CreateEntity();
            settingsFactory.Save();
        }

        private void SetDefaultSettings_Click(object sender, RoutedEventArgs e)
        {
            var settings = factories.Settings;
            settings.ScoreEnergy = true;
            settings.ScoreProtein = true;
            settings.ScoreDigestibleCarbs = true;
            settings.ScoreFat = true;
            settings.ScoreCu = false;
            settings.ScoreFpu = false;
            // These strings should be emptied manually in binary file (including setting their length at beginning)
            settings.NextProductCulture = "DELETEME";
            settings.NextUiCulture = "DELETEME";
        }

        private void ClearMeals_Click(object sender, RoutedEventArgs e)
        {
            factories.Meals.Clear();
        }

        private void OpenProducts_Click(object sender, RoutedEventArgs e)
        {
            var products = factories.Products;
        }

        private void SetDefaultMealNames_Click(object sender, RoutedEventArgs e)
        {
            var mealNames = factories.MealNames;
            mealNames.Clear();
            var breakfast = factories.CreateMealName();
            breakfast.Kind = MealNameKind.Breakfast;
            var lunch = factories.CreateMealName();
            lunch.Kind = MealNameKind.Lunch;
            var dinner = factories.CreateMealName();
            dinner.Kind = MealNameKind.Dinner;
        }
    }
}
