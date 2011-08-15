using System.Windows;

namespace Dietphone.Views
{
    public class LearningCuAndFpu
    {
        private const string CU = @"WW czyli wymiennik węglowodanowy to ilość węglowodanów przyswajalnych w gramach podzielona przez 10.
Został wprowadzony, żeby ułatwić liczenie węglowodanów w posiłku.";
        private const string FPU = @"WBT czyli wymiennik białkowo-tłuszczowy to wartość energetyczna (kcal) pochodząca z białka i tłuszczu podzielona przez 100.
Pozwala w jednej poręcznej liczbie określić zawartość białka i tłuszczu w posiłku.";

        public void LearnCuAndFpu()
        {
            var both = string.Format("{0}\r\n\r\n{1}", CU, FPU);
            MessageBox.Show(both);
        }

        public void LearnCu()
        {
            MessageBox.Show(CU);
        }

        public void LearnFpu()
        {
            MessageBox.Show(FPU);
        }
    }
}
