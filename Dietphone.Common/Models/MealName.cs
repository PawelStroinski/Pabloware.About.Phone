using Dietphone.Views;

namespace Dietphone.Models
{
    public sealed class MealName : EntityWithId
    {
        public MealNameKind Kind { get; set; }
        private string customName = string.Empty;

        // Please note that setting Name may change Kind.
        public string Name
        {
            get
            {
                return GetMealNameByKind();
            }
            set
            {
                if (Name != value)
                {
                    Kind = MealNameKind.Custom;
                    customName = value;
                }
            }
        }

        private string GetMealNameByKind()
        {
            switch (Kind)
            {
                case MealNameKind.Custom:
                    return customName;
                case MealNameKind.Breakfast:
                    return Translations.Breakfast;
                case MealNameKind.Lunch:
                    return Translations.Lunch;
                case MealNameKind.Dinner:
                    return Translations.Dinner;
                default:
                    return string.Empty;
            }
        }
    }

    public enum MealNameKind
    {
        Custom,
        Breakfast,
        Lunch,
        Dinner
    }
}