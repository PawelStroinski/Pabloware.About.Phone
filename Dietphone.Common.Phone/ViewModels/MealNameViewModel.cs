using System;
using Dietphone.Models;

namespace Dietphone.ViewModels
{
    public class MealNameViewModel : ViewModelWithBuffer<MealName>
    {
        public MealNameViewModel(MealName model, Factories factories)
            : base(model, factories)
        {
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
