using System.Collections.Generic;
using System;

namespace Dietphone.Models
{
    public interface Factories
    {
        Finder Finder { get; }
        List<Product> Products { get; }
        List<Category> Categories { get; }

        Product CreateProduct();
        Category CreateCategory();
    }

    public class FactoriesImpl : Factories
    {
        public Finder Finder { get; private set; }
        private Factory<Product> productFactory;
        private Factory<Category> categoryFactory;
        private readonly FactoryCreator factoryCreator;

        public FactoriesImpl(StorageCreator storageCreator)
        {
            factoryCreator = new FactoryCreator(this, storageCreator);
            CreateFactories();
            Finder = new FinderImpl(this);
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

        public Product CreateProduct()
        {
            var product = productFactory.CreateEntity();
            product.Id = Guid.NewGuid();
            var defaultCategory = Finder.FindCategoryFirstAlphabetically();
            product.CategoryId = defaultCategory.Id;
            return product;
        }

        public Category CreateCategory()
        {
            var category = categoryFactory.CreateEntity();
            category.Id = Guid.NewGuid();
            return category;
        }

        private void CreateFactories()
        {
            productFactory = factoryCreator.CreateFactory<Product>();
            categoryFactory = factoryCreator.CreateFactory<Category>();
        }
    }
}