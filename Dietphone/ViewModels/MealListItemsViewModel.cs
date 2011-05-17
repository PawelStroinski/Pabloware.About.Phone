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
using Telerik.Windows.Controls;
using Dietphone.Models;

namespace Dietphone.ViewModels
{
    public class MealListItemsViewModel : LoopingListDataSource
    {
        public MealListItemsViewModel(int count)
            : base(count)
        {
        }

        protected override LoopingListDataItem GetItemCore(int index)
        {
            var item = new MealListItemViewModel();
            this.UpdateItemCore(item, index);
            return item;
        }

        protected override void UpdateItemCore(LoopingListDataItem dataItem, int logicalIndex)
        {
            var myItem = dataItem as MealListItemViewModel;
            var testModel = TestModel(logicalIndex);
            myItem.LoadMeal(testModel);
        }

        private Meal TestModel(int logicalIndex)
        {
            Meal testModel = new Meal();
            int hour = 0;
            //string desc = "";
            switch (logicalIndex % 3)
            {
                case 2:
                    //desc = "Śniadanie";
                    hour = 8;
                    break;
                case 1:
                    //desc = "Obiad";
                    hour = 14;
                    break;
                case 0:
                    //desc = "Kolacja";
                    hour = 19;
                    break;
            }
            //testModel.Name = desc;
            testModel.Date = new DateTime(2011, 03, 28 - (logicalIndex / 3), hour, 0, 0);
            var r = new Random();
            //testModel.Cu = r.Next(1, 5);
            //testModel.Fpu = r.Next(1, 5);
            //testModel.Energy = (short)r.Next(100, 500);
            return testModel;
        }
    }
}
