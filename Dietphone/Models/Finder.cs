using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dietphone.Models
{
    public interface Finder
    {
        Meal FindMealById(Guid mealId);
        MealName FindMealNameById(Guid mealNameId);
        Product FindProductById(Guid productId);
        Category FindCategoryById(Guid categoryId);
        List<Product> FindProductsByCategory(Guid categoryId);
        Category FindCategoryFirstAlphabetically();
    }

    public sealed class FinderImpl : Finder
    {
        private Factories factories;

        public FinderImpl(Factories factories)
        {
            this.factories = factories;
        }

        public Meal FindMealById(Guid mealId)
        {
            var meals = factories.Meals;
            return meals.FindById(mealId);
        }

        public MealName FindMealNameById(Guid mealNameId)
        {
            var mealNames = factories.MealNames;
            return mealNames.FindById(mealNameId);
        }

        public Product FindProductById(Guid productId)
        {
            var products = factories.Products;
            return products.FindById(productId);
        }

        public Category FindCategoryById(Guid categoryId)
        {
            var categories = factories.Categories;
            return categories.FindById(categoryId);
        }

        public List<Product> FindProductsByCategory(Guid categoryId)
        {
            var result = from product in factories.Products
                         where product.CategoryId == categoryId
                         select product;
            return result.ToList();
        }

        public Category FindCategoryFirstAlphabetically()
        {
            var categories = factories.Categories;
            var sortedCategories = categories.OrderBy(category => category.Name);
            return sortedCategories.FirstOrDefault();
        }
    }

    public static class FinderExtensions
    {
        public static T FindById<T>(this List<T> entities, Guid entityId) where T : EntityWithId
        {
            var result = from entity in entities
                         where entity.Id == entityId
                         select entity;
            return result.FirstOrDefault();
        }
    }
}
