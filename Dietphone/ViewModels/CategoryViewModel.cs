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
using Dietphone.Models;

namespace Dietphone.ViewModels
{
    public class CategoryViewModel : ViewModelBase, IComparable
    {
        public Category Category { get; private set; }

        public CategoryViewModel(Category category)
        {
            Category = category;
        }

        public Guid Id
        {
            get
            {
                return Category.Id;
            }
        }

        public string Name
        {
            get
            {
                return Category.Name;
            }
            set
            {
                Category.Name = value;
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
                var anotherCategory = another.Category;
                return string.Compare(Category.Name, anotherCategory.Name);
            }
        }
    }
}