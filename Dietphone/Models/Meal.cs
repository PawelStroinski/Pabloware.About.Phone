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

namespace Dietphone.Models
{
    public class Meal
    {
        public int ID { get; set; }
        public DateTime Date { get; set; }
        public float CU { get; set; }
        public float FPU { get; set; }
        public short Energy { get; set; }
        public string Description { get; set; }
    }
}
