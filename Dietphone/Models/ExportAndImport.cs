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
using System.Collections.Generic;

namespace Dietphone.Models
{
    public class ExportAndImport : ExporterAndImporter
    {
        public List<Meal> Meals { get; private set; }
        public List<MealName> MealNames { get; private set; }
        public List<Product> Products { get; private set; }
        public List<Category> Categories { get; private set; }
        private readonly Factories factories;

        public ExportAndImport(Factories factories)
        {
            this.factories = factories;
        }

        public string Export()
        {
            throw new NotImplementedException();
        }

        public void Import(string data)
        {
            throw new NotImplementedException();
        }
    }
}
