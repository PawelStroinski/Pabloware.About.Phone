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
    public class Calculator
    {
        public float? Protein { get; set; }
        public float? Fat { get; set; }
        public float? DigestibleCarbs { get; set; }

        public short Energy
        {
            get
            {
                var energy = Protein.Value * 4 + Fat.Value * 9 + DigestibleCarbs.Value * 4;
                var roundedEnergy = Math.Round(energy);
                return (short)roundedEnergy;
            }
        }

        public float Cu
        {
            get
            {
                var cu = DigestibleCarbs.Value / 10.0;
                var roundedCu = Math.Round(cu, 1);
                return (float)roundedCu;
            }
        }

        public float Fpu
        {
            get
            {
                var fpuEnergy = Protein.Value * 4 + Fat.Value * 9;
                var fpu = fpuEnergy / 100.0;
                var roundedFpu = Math.Round(fpu, 1);
                return (float)roundedFpu;
            }
        }
    }
}
