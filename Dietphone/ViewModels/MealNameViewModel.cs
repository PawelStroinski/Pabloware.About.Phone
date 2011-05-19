using System;
using Dietphone.Models;

namespace Dietphone.ViewModels
{
    public class MealNameViewModel : ViewModelBuffer<MealName>
    {
        public MealNameViewModel(MealName mealName)
        {
            Model = mealName;
        }

        public Guid Id
        {
            get
            {
                return Model.Id;
            }
        }

        public string Name
        {
            get
            {
                return BufferOrModel.Name;
            }
            set
            {
                BufferOrModel.Name = value;
                OnPropertyChanged("Name");
            }
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
