using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using Dietphone.ViewModels;
using Dietphone.Models;
using Microsoft.Phone.Controls;
using Telerik.Windows.Data;
using System.Linq;
using Telerik.Windows.Controls;
using System.Windows.Media.Animation;

namespace Dietphone.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private string search = "";

        public string Search
        {
            get
            {
                return search;
            }
            set
            {
                if (search != value)
                {
                    search = value;
                    OnPropertyChanged("Search");
                }
            }
        }
    }
}