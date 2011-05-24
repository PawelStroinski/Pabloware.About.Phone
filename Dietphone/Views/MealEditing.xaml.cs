using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;

namespace Dietphone.Views
{
    public partial class MealEditing : PhoneApplicationPage
    {
        public List<string> MealNames { get; set; }
        public MealEditing()
        {
            InitializeComponent();
            MealNames = new List<string>();
            MealNames.Add("Bez nazwy");
            MealNames.Add("Śniadanie");
            MealNames.Add("Obiad");
            MealNames.Add("Kolacja");
            DataContext = this;
        }

        private void AddMealName_Click(object sender, RoutedEventArgs e)
        {

        }

        private void EditMealName_Click(object sender, RoutedEventArgs e)
        {

        }

        private void DeleteMealName_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Save_Click(object sender, EventArgs e)
        {

        }

        private void Cancel_Click(object sender, EventArgs e)
        {

        }

        private void Delete_Click(object sender, EventArgs e)
        {

        }
    }
}