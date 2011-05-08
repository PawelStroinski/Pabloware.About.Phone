using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Dietphone.ViewModels;
using Dietphone.Models;
using Dietphone.BinarySerializers;
using Microsoft.Phone.Tasks;
using Telerik.Windows.Controls;

namespace Dietphone
{
    public partial class App : Application
    {
        public RadPhoneApplicationFrame RootFrame { get; private set; }
        private bool phoneApplicationInitialized = false;
        private static Factories factories = null;
        private static readonly object factoriesLock = new object();

        public static Factories Factories
        {
            get
            {
                lock (factoriesLock)
                {
                    if (factories == null)
                    {
                        CreateFactories();
                    }
                    return factories;
                }
            }
            set
            {
                if (value == null)
                {
                    throw new NullReferenceException("Factories");
                }
                factories = value;
            }
        }

        public App()
        {
            UnhandledException += Application_UnhandledException;
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // Display the current frame rate counters
                //Application.Current.Host.Settings.EnableFrameRateCounter = true;
                // Show the areas of the app that are being redrawn in each frame.
                //Application.Current.Host.Settings.EnableRedrawRegions = true;
                // Enable non-production analysis visualization mode, 
                // which shows areas of a page that are being GPU accelerated with a colored overlay.
                //Application.Current.Host.Settings.EnableCacheVisualization = true;
            }
            // Standard Silverlight initialization
            InitializeComponent();
            // Phone-specific initialization
            InitializePhoneApplication();
        }

        // Code to execute when the application is launching (eg, from Start)
        // This code will not execute when the application is reactivated
        private void Application_Launching(object sender, LaunchingEventArgs e)
        {
        }

        // Code to execute when the application is activated (brought to foreground)
        // This code will not execute when the application is first launched
        private void Application_Activated(object sender, ActivatedEventArgs e)
        {
        }

        // Code to execute when the application is deactivated (sent to background)
        // This code will not execute when the application is closing
        private void Application_Deactivated(object sender, DeactivatedEventArgs e)
        {
        }

        // Code to execute when the application is closing (eg, user hit Back)
        // This code will not execute when the application is deactivated
        private void Application_Closing(object sender, ClosingEventArgs e)
        {
        }

        private static void CreateFactories()
        {
            StorageCreator binary = new BinaryStorageCreator();
            factories = new FactoriesImpl(binary);
        }

        private void InitializePhoneApplication()
        {
            if (phoneApplicationInitialized)
                return;
            // Create the frame but don't set it as RootVisual yet; this allows the splash
            // screen to remain active until the application is ready to render.
            RootFrame = new RadPhoneApplicationFrame();
            RootFrame.Navigated += CompleteInitializePhoneApplication;
            RootFrame.NavigationFailed += RootFrame_NavigationFailed;
            phoneApplicationInitialized = true;
        }

        private void CompleteInitializePhoneApplication(object sender, NavigationEventArgs e)
        {
            if (RootVisual != RootFrame)
                RootVisual = RootFrame;
            RootFrame.Navigated -= CompleteInitializePhoneApplication;
        }

        private void RootFrame_NavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            if (System.Diagnostics.Debugger.IsAttached)
            {
                System.Diagnostics.Debugger.Break();
            }
            else
            {
                var exception = e.Exception;
                SendExceptionQuestion(exception.ToString());
                e.Handled = true;
            }
        }

        private void Application_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            if (System.Diagnostics.Debugger.IsAttached)
            {
                System.Diagnostics.Debugger.Break();
            }
            else
            {
                var exception = e.ExceptionObject;
                SendExceptionQuestion(exception.ToString());
                e.Handled = true;
            }
        }

        private void SendExceptionQuestion(string exception)
        {
            if (MessageBox.Show(exception, "A niech to, pluskwa w aplikacji! Zgłosić autorowi?",
                MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                SendException(exception);
            }
        }

        private void SendException(string exception)
        {
            EmailComposeTask task = new EmailComposeTask();
            task.To = "pol84@live.com";
            task.Body = String.Format("Zgłaszam poniższą pluskwę:\r\n\r\n{0}", exception);
            task.Subject = "Pluskwa!";
            task.Show();
        }
    }
}