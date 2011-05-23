using System.Collections.Generic;
using System;
using Dietphone.Tools;

namespace Dietphone.Models
{
    public interface Factories
    {
        Finder Finder { get; }
        DefaultsFactory DefaultsFactory { get; }
        List<Meal> Meals { get; }
        List<MealName> MealNames { get; }
        List<Product> Products { get; }
        List<Category> Categories { get; }

        Meal CreateMeal();
        MealName CreateMealName();
        Product CreateProduct();
        Category CreateCategory();
        void Save();
    }

    public sealed class FactoriesImpl : Factories
    {
        public Finder Finder { get; private set; }
        public DefaultsFactory DefaultsFactory { get; private set; }
        private Factory<Meal> mealFactory;
        private Factory<MealName> mealNameFactory;
        private Factory<Product> productFactory;
        private Factory<Category> categoryFactory;
        private readonly FactoryCreator factoryCreator;

        public FactoriesImpl(StorageCreator storageCreator)
        {
            factoryCreator = new FactoryCreator(this, storageCreator);
            CreateFactories();
            Finder = new FinderImpl(this);
            DefaultsFactory = new DefaultsFactoryImpl(this);
        }

        public List<Meal> Meals
        {
            get
            {
                return mealFactory.Entities;
            }
        }

        public List<MealName> MealNames
        {
            get
            {
                return mealNameFactory.Entities;
            }
        }

        public List<Product> Products
        {
            get
            {
                return productFactory.Entities;
            }
        }

        public List<Category> Categories
        {
            get
            {
                return categoryFactory.Entities;
            }
        }

        public Meal CreateMeal()
        {
            var meal = mealFactory.CreateEntity();
            meal.Id = Guid.NewGuid();
            meal.DateTime = DateTime.UtcNow;
            var items = new List<MealItem>();
            meal.InitializeItems(items);
            meal.SetNullStringPropertiesToEmpty();
            return meal;
        }

        public MealName CreateMealName()
        {
            var mealName = mealNameFactory.CreateEntity();
            mealName.Id = Guid.NewGuid();
            mealName.SetNullStringPropertiesToEmpty();
            return mealName;
        }

        public Product CreateProduct()
        {
            var product = productFactory.CreateEntity();
            product.Id = Guid.NewGuid();
            var defaultCategory = Finder.FindCategoryFirstAlphabetically();
            product.CategoryId = defaultCategory.Id;
            product.SetNullStringPropertiesToEmpty();
            return product;
        }

        public Category CreateCategory()
        {
            var category = categoryFactory.CreateEntity();
            category.Id = Guid.NewGuid();
            category.SetNullStringPropertiesToEmpty();
            return category;
        }

        public void Save()
        {
            mealFactory.Save();
            mealNameFactory.Save();
            productFactory.Save();
            categoryFactory.Save();
        }

        private void CreateFactories()
        {
            mealFactory = factoryCreator.CreateFactory<Meal>();
            mealNameFactory = factoryCreator.CreateFactory<MealName>();
            productFactory = factoryCreator.CreateFactory<Product>();
            categoryFactory = factoryCreator.CreateFactory<Category>();
        }
    }
}