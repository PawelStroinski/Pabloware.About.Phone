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

namespace Dietphone.Views
{
    public partial class MealItemEditing : UserControl
    {
        public List<string> AllUsableUnits { get; set; }
        public MealItemEditing()
        {
            InitializeComponent();
            AllUsableUnits = new List<string>();
            AllUsableUnits.Add("g");
            AllUsableUnits.Add("ml");
            AllUsableUnits.Add("szklanka");

            DataContext = this;
        }

        private void ApplicationBarInfo_ButtonClick(object sender, Telerik.Windows.Controls.ApplicationBarButtonClickEventArgs e)
        {

        }
    }
}
