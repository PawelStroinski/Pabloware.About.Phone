using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dietphone.Models
{
    public interface Finder
    {
        Product FindProductById(Guid productId);
        IEnumerable<Product> FindProductsByCategory(Guid categoryId);
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

        public IEnumerable<Product> FindProductsByCategory(Guid categoryId)
        {
            var result = from product in factories.Products
                         where product.CategoryId == categoryId
                         select product;
            return result;
        }
    }
}
