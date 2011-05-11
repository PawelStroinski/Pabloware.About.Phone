using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dietphone.Models
{
    public interface Finder
    {
        Product FindProductById(Guid productId);
        Category FindCategoryById(Guid categoryId);
        List<Product> FindProductsByCategory(Guid categoryId);
    }

    public class FinderImpl : Finder
    {
        private Factories factories;

        public FinderImpl(Factories factories)
        {
            this.factories = factories;
        }

        public Product FindProductById(Guid productId)
        {
            var result = from product in factories.Products
                         where product.Id == productId
                         select product;
            return result.FirstOrDefault();
        }

        public Category FindCategoryById(Guid categoryId)
        {
            var result = from category in factories.Categories
                         where category.Id == categoryId
                         select category;
            return result.FirstOrDefault();
        }

        public List<Product> FindProductsByCategory(Guid categoryId)
        {
            var result = from product in factories.Products
                         where product.CategoryId == categoryId
                         select product;
            return result.ToList();
        }
    }
}
