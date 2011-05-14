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

namespace Dietphone.Tools
{
    public class Constrains
    {
        public double Min { get; set; }
        public double Max { get; set; }

        public double Constraint(double value)
        {
            if (value < Min)
            {
                value = Min;
            }
            if (value > Max)
            {
                value = Max;
            }
            return value;
        }

        public float Constraint(float value)
        {
            return (float)Constraint((double)value);
        }

        public short Constraint(short value)
        {
            return (short)Constraint((double)value);
        }
    }
}
