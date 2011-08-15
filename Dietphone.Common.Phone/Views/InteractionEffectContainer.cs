using System.Windows.Controls;
using Telerik.Windows.Controls;

namespace Dietphone.Views
{
    public class InteractionEffectContainer : ContentControl
    {
        static InteractionEffectContainer()
        {
            InteractionEffectManager.AllowedTypes.Add(typeof(InteractionEffectContainer));
        }

        public InteractionEffectContainer()
        {
            DefaultStyleKey = typeof(InteractionEffectContainer);
        }
    }
}
