using System;
using Dietphone.Models;
using System.Collections.Generic;
using Dietphone.Tools;
using System.Linq;

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

        public void AddModelTo(List<MealName> target)
        {
            target.Add(BufferOrModel);
        }

        public void CopyModelFrom(MealName source)
        {
            BufferOrModel.CopyFrom(source);
        }
    }

    public static class MealNameExtensions
    {
        public static MealNameViewModel FindById(this IEnumerable<MealNameViewModel> names, Guid id)
        {
            var result = from name in names where name.Id == id select name;
            return result.FirstOrDefault();
        }
    }
}
