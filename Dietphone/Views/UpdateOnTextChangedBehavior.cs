// Kod zaczerpnięty z http://zoltanarvai.com/2009/07/22/binding-update-on-textbox-textchanged-event-using-behaviors/

using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Interactivity;

namespace Dietphone.Views
{
    public class UpdateOnTextChangedBehavior : Behavior<TextBox>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.TextChanged += new TextChangedEventHandler(AssociatedObject_TextChanged);
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.TextChanged -= new TextChangedEventHandler(AssociatedObject_TextChanged);
        }

        private void AssociatedObject_TextChanged(object sender, TextChangedEventArgs e)
        {
            BindingExpression binding = AssociatedObject.GetBindingExpression(TextBox.TextProperty);
            if (binding != null)
            {
                binding.UpdateSource();
            }
        }
    }
}