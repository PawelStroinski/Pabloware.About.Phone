using System;
using System.Windows.Threading;

namespace Dietphone.ViewModels
{
    public class OptionalDispatcher
    {
        private readonly Dispatcher dispatcher;

        public OptionalDispatcher()
        {
        }

        public OptionalDispatcher(Dispatcher dispatcher)
        {
            this.dispatcher = dispatcher;
        }

        public DispatcherOperation BeginInvoke(Action action)
        {
            if (dispatcher == null)
            {
                action.Invoke();
                return null;
            }
            else
            {
                return dispatcher.BeginInvoke(action);
            }
        }
    }
}
