// Kod zaczerpnięty z http://www.japf.fr/2009/02/very-simple-mvvm-demo-application/ i dostosowany do SL oraz dodano obsługę powiadomień o wszystkich properites
// The following code is inspired by the work of Josh Smith
// http://joshsmithonwpf.wordpress.com/
using System.ComponentModel;
using System.Diagnostics;

namespace Pabloware.About.ViewModels
{
    /// <summary>
    /// Base class for all ViewModel classes in the application. Provides support for 
    /// property changes notification.
    /// </summary>
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        /// <summary>
        /// Raised when a property on this object has a new value.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Warns the developer if this object does not have a public property with
        /// the specified name. This method does not exist in a Release build.
        /// </summary>
        [Conditional("DEBUG")]
        public void VerifyPropertyName(string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName))
            {
                return;
            }
            // verify that the property name matches a real,  
            // public, instance property on this object.
            if (GetType().GetProperty(propertyName) == null)
            {
                Debug.Assert(false, "Invalid property name: " + propertyName);
            }
        }

        /// <summary>
        /// Raises this object's PropertyChanged event.
        /// </summary>
        /// <param name="propertyName">The name of the property that has a new value.</param>
        protected virtual void OnPropertyChanged(string propertyName)
        {
            VerifyPropertyName(propertyName);

            if (PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
