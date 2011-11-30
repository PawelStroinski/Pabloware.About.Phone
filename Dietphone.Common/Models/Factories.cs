using System.Collections.Generic;
using System;
using Dietphone.Tools;
using System.Linq;

namespace Dietphone.Models
{
    public interface Factories
    {
        StorageCreator StorageCreator { set; }
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
        private readonly object mealFactoryLock = new object();
        private readonly object mealNameFactoryLock = new object();
        private readonly object productFactoryLock = new object();
        private readonly object categoryFactoryLock = new object();
        private readonly object settingsFactoryLock = new object();

        public FactoriesImpl()
        {
            factoryCreator = new FactoryCreator(this);
            Finder = new FinderImpl(this);
            DefaultEntities = new DefaultEntitiesImpl(this);
        }

        public StorageCreator StorageCreator
        {
            set
            {
                factoryCreator.StorageCreator = value;
            }
        }

        public List<Meal> Meals
        {
            get
            {
                return MealFactory.Entities;
            }
        }

        public List<MealName> MealNames
        {
            get
            {
                return MealNameFactory.Entities;
            }
        }

        public List<Product> Products
        {
            get
            {
                return ProductFactory.Entities;
            }
        }

        public List<Category> Categories
        {
            get
            {
                return CategoryFactory.Entities;
            }
        }

        public Settings Settings
        {
            get
            {
                var entities = SettingsFactory.Entities;
                return entities.First();
            }
        }

        public Meal CreateMeal()
        {
            var meal = MealFactory.CreateEntity();
            meal.Id = Guid.NewGuid();
            meal.DateTime = DateTime.UtcNow;
            var items = new List<MealItem>();
            meal.InitializeItems(items);
            meal.SetNullStringPropertiesToEmpty();
            return meal;
        }

        public MealName CreateMealName()
        {
            var mealName = MealNameFactory.CreateEntity();
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
            var product = ProductFactory.CreateEntity();
            product.Id = Guid.NewGuid();
            var defaultCategory = Finder.FindCategoryFirstAlphabetically();
            product.CategoryId = defaultCategory.Id;
            product.SetNullStringPropertiesToEmpty();
            return product;
        }

        public Category CreateCategory()
        {
            var category = CategoryFactory.CreateEntity();
            category.Id = Guid.NewGuid();
            category.SetNullStringPropertiesToEmpty();
            return category;
        }

        public void Save()
        {
            MealFactory.Save();
            MealNameFactory.Save();
            ProductFactory.Save();
            CategoryFactory.Save();
            SettingsFactory.Save();
        }

        private Factory<Meal> MealFactory
        {
            get
            {
                lock (mealFactoryLock)
                {
                    if (mealFactory == null)
                    {
                        mealFactory = factoryCreator.CreateFactory<Meal>();
                    }
                    return mealFactory;
                }
            }
        }

        private Factory<MealName> MealNameFactory
        {
            get
            {
                lock (mealNameFactoryLock)
                {
                    if (mealNameFactory == null)
                    {
                        mealNameFactory = factoryCreator.CreateFactory<MealName>();
                    }
                    return mealNameFactory;
                }
            }
        }

        private Factory<Product> ProductFactory
        {
            get
            {
                lock (productFactoryLock)
                {
                    if (productFactory == null)
                    {
                        productFactory = factoryCreator.CreateFactory<Product>();
                    }
                    return productFactory;
                }
            }
        }

        private Factory<Category> CategoryFactory
        {
            get
            {
                lock (categoryFactoryLock)
                {
                    if (categoryFactory == null)
                    {
                        categoryFactory = factoryCreator.CreateFactory<Category>();
                    }
                    return categoryFactory;
                }
            }
        }

        private Factory<Settings> SettingsFactory
        {
            get
            {
                lock (settingsFactoryLock)
                {
                    if (settingsFactory == null)
                    {
                        settingsFactory = factoryCreator.CreateFactory<Settings>();
                    }
                    return settingsFactory;
                }
            }
        }
    }
}