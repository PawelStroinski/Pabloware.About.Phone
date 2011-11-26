using System.Windows;

namespace Dietphone.Views
{
    public class LearningCuAndFpu
    {
        public void LearnCuAndFpu()
        {
            var both = string.Format("{0}\r\n\r\n{1}", Translations.CuIs, Translations.FpuIs);
            MessageBox.Show(both);
        }

        public void LearnCu()
        {
            MessageBox.Show(Translations.CuIs);
        }

        public void LearnFpu()
        {
            MessageBox.Show(Translations.FpuIs);
        }
    }
}
