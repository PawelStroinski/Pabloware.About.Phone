using System;
using Dietphone.Models;

namespace Dietphone.ViewModels
{
    public class CategoryViewModel : ViewModelWithBufferAcc<Category>, IComparable, HasId
    {
        public CategoryViewModel(Category model, Factories factories)
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

        public int CompareTo(object obj)
        {
            var another = obj as CategoryViewModel;
            if (another == null)
            {
                throw new ArgumentException("Object is not CategoryViewModel");
            }
            else
            {
                return string.Compare(Name, another.Name);
            }
        }

        public override string ToString()
        {
            return Name;
        }
    }
}