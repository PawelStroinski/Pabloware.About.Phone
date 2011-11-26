using System;
using Dietphone.Views;

namespace Dietphone.Models
{
    public interface DefaultEntities
    {
        MealName MealName { get; }
        Product Product { get; }
    }

    public sealed class DefaultEntitiesImpl : DefaultEntities
    {
        private MealName mealName;
        private Product product;
        private readonly Factories owner;
        private readonly object mealNameLock = new object();
        private readonly object productLock = new object();

        public DefaultEntitiesImpl(Factories owner)
        {
            this.owner = owner;
        }

        public MealName MealName
        {
            get
            {
                lock (mealNameLock)
                {
                    if (mealName == null)
                    {
                        mealName = owner.CreateMealName();
                        var mealNames = owner.MealNames;
                        mealNames.Remove(mealName);
                        mealName.Id = Guid.Empty;
                        mealName.Name = Translations.NoName;
                    }
                    return mealName;
                }
            }
        }

        public Product Product
        {
            get
            {
                lock (productLock)
                {
                    if (product == null)
                    {
                        product = owner.CreateProduct();
                        var products = owner.Products;
                        products.Remove(product);
                        product.Id = Guid.Empty;
                        product.Name = Translations.NoProduct;
                    }
                    return product;
                }
            }
        }
    }
}
