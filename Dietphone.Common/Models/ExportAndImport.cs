using System.Collections.Generic;
using Dietphone.Tools;

namespace Dietphone.Models
{
    public class ExportAndImport
    {
        private ExportAndImportDTO dto;
        private readonly Factories factories;
        private readonly Finder finder;
        private readonly AppVersion appVersion = new AppVersion();
        private const string NAMESPACE = "http://www.pabloware.com/wp7";

        public ExportAndImport(Factories factories)
        {
            this.factories = factories;
            finder = factories.Finder;
        }

        public string Export()
        {
            dto = new ExportAndImportDTO
            {
                AppVersion = appVersion.GetAppVersion(),
                Meals = ExportMeals(),
                MealNames = factories.MealNames,
                Products = finder.FindProductsAddedByUser(),
                Categories = factories.Categories,
                Settings = factories.Settings
            };
            return dto.Serialize(NAMESPACE);
        }

        public void Import(string data)
        {
            dto = data.Deserialize<ExportAndImportDTO>(NAMESPACE);
            ImportMeals();
            ImportMealNames();
            ImportProducts();
            ImportCategories();
            ImportSettings();
        }

        private List<MealDTO> ExportMeals()
        {
            var targets = new List<MealDTO>();
            foreach (var source in factories.Meals)
            {
                var target = new MealDTO();
                target.CopyFrom(source);
                target.DTOCopyItemsFrom(source);
                targets.Add(target);
            }
            return targets;
        }

        private void ImportMeals()
        {
            foreach (var source in dto.Meals)
            {
                var target = finder.FindMealById(source.Id);
                if (target == null)
                {
                    target = factories.CreateMeal();
                }
                target.CopyFrom(source);
                target.CopyItemsFrom(source);
            }
        }

        private void ImportMealNames()
        {
            var importer = new GenericImporter<MealName>
            {
                Sources = dto.MealNames,
                Targets = factories.MealNames
            };
            importer.Create += factories.CreateMealName;
            importer.Execute();
        }

        private void ImportProducts()
        {
            var importer = new GenericImporter<Product>
            {
                Sources = dto.Products,
                Targets = factories.Products
            };
            importer.Create += factories.CreateProduct;
            importer.Execute();
        }

        private void ImportCategories()
        {
            var importer = new GenericImporter<Category>
            {
                Sources = dto.Categories,
                Targets = factories.Categories
            };
            importer.Create += factories.CreateCategory;
            importer.Execute();
        }

        private void ImportSettings()
        {
            var source = dto.Settings;
            var target = factories.Settings;
            target.CopyFrom(source);
        }

        public sealed class ExportAndImportDTO
        {
            public string AppVersion { get; set; }
            public List<MealDTO> Meals { get; set; }
            public List<MealName> MealNames { get; set; }
            public List<Product> Products { get; set; }
            public List<Category> Categories { get; set; }
            public Settings Settings { get; set; }
        }

        private sealed class GenericImporter<T> where T : EntityWithId
        {
            public delegate T CreateHandler();

            public List<T> Sources { get; set; }
            public List<T> Targets { get; set; }
            public event CreateHandler Create;

            public void Execute()
            {
                foreach (var source in Sources)
                {
                    var target = Targets.FindById(source.Id);
                    if (target == null)
                    {
                        target = Create();
                    }
                    target.CopyFrom(source);
                }
            }
        }
    }

    public sealed class MealDTO : Meal
    {
        public new List<MealItem> Items
        {
            get
            {
                return items;
            }
            set
            {
                items = value;
            }
        }

        public void DTOCopyItemsFrom(Meal source)
        {
            InternalCopyItemsFrom(source);
        }
    }
}
