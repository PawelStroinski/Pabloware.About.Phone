using Microsoft.Phone.Controls;
using System.Windows.Navigation;
using Pabloware.About.Tools;

namespace Pabloware.About.Views
{
    public class StateProviderPage : PhoneApplicationPage, StateProvider
    {
        public bool IsOpened { get; private set; }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            IsOpened = true;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            IsOpened = false;
        }
    }
}