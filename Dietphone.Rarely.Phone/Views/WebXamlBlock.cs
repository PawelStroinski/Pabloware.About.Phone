// Na podstawie http://www.jeff.wilcox.name/2011/07/my-app-about-page/
using System.Windows.Controls;
using System;
using System.Windows;
using System.Net;
using System.Windows.Data;
using System.Windows.Markup;

namespace Dietphone.Views
{
    public class WebXamlBlock : ContentControl
    {
        private bool haveTriedDownloading;

        public WebXamlBlock()
        {
        }

        public Uri XamlUri
        {
            get
            {
                return GetValue(XamlUriProperty) as Uri;
            }
            set
            {
                SetValue(XamlUriProperty, value);
            }
        }

        public object FallbackContent
        {
            get
            {
                return GetValue(FallbackContentProperty);
            }
            set
            {
                SetValue(FallbackContentProperty, value);
            }
        }

        public static readonly DependencyProperty XamlUriProperty =
            DependencyProperty.Register(
                "XamlUri",
                typeof(Uri),
                typeof(WebXamlBlock),
                new PropertyMetadata(null, OnXamlUriPropertyChanged));

        public static readonly DependencyProperty FallbackContentProperty =
            DependencyProperty.Register(
                "FallbackContent",
                typeof(object),
                typeof(WebXamlBlock),
                new PropertyMetadata(null));

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            TryDownloading();
        }

        private static void OnXamlUriPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            WebXamlBlock source = d as WebXamlBlock;
            source.TryDownloading();
        }

        private void TryDownloading()
        {
            if (haveTriedDownloading)
            {
                return;
            }
            haveTriedDownloading = true;
            if (XamlUri != null)
            {
                var webClient = new WebClient();
                webClient.DownloadStringCompleted += OnDownloadStringCompleted;
                webClient.DownloadStringAsync(XamlUri);
            }
        }

        private void OnDownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            if (e.Error != null || e.Cancelled)
            {
                Dispatcher.BeginInvoke(() =>
                {
                    OnError();
                });
            }
            else
            {
                string xaml = e.Result;
                Dispatcher.BeginInvoke(() =>
                {
                    CatchedLoadXaml(xaml);
                });
            }
        }

        private void OnError()
        {
            var b = new Binding("FallbackContent")
            {
                Source = this
            };
            SetBinding(ContentProperty, b);
        }

        private void CatchedLoadXaml(string xaml)
        {
            try
            {
                LoadXaml(xaml);
            }
            catch
            {
                OnError();
            }
        }

        private void LoadXaml(string xaml)
        {
            var o = XamlReader.Load(xaml);
            if (o != null)
            {
                Content = o;
            }
        }
    }
}