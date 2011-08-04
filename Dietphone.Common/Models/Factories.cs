using System.Collections.Generic;
using System;
using Dietphone.Tools;
using System.Linq;

namespace Dietphone.Models
{
    public interface Factories
    {
        Finder Finder { get; }
        DefaultEntities DefaultEntities { get; }
        List<Meal> Meals { get; }
        List<MealName> MealNames { get; }
        List<Product> Products { get; }
        List<Category> Categories { get; }
        Settings Settings { get; }

        Meal CreateMeal();
        MealName CreateMealName();
        MealItem CreateMealItem();
        Product CreateProduct();
        Category CreateCategory();
        void Save();
    }

    public sealed class FactoriesImpl : Factories
    {
        public Finder Finder { get; private set; }
        public DefaultEntities DefaultEntities { get; private set; }
        private Factory<Meal> mealFactory;
        private Factory<MealName> mealNameFactory;
        private Factory<Product> productFactory;
        private Factory<Category> categoryFactory;
        private Factory<Settings> settingsFactory;
        private readonly FactoryCreator factoryCreator;

        public FactoriesImpl(StorageCreator storageCreator)
        {
            factoryCreator = new FactoryCreator(this, storageCreator);
            CreateFactories();
            Finder = new FinderImpl(this);
            DefaultEntities = new DefaultEntitiesImpl(this);
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

        public Settings Settings
        {
            get
            {
                var entities = settingsFactory.Entities;
                return entities.First();
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

        public MealItem CreateMealItem()
        {
            var mealItem = new MealItem();
            mealItem.SetOwner(this);
            return mealItem;
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
            settingsFactory.Save();
        }

        private void CreateFactories()
        {
            mealFactory = factoryCreator.CreateFactory<Meal>();
            mealNameFactory = factoryCreator.CreateFactory<MealName>();
            productFactory = factoryCreator.CreateFactory<Product>();
            categoryFactory = factoryCreator.CreateFactory<Category>();
            settingsFactory = factoryCreator.CreateFactory<Settings>();
        }
    }
}