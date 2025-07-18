using Microsoft.Web.WebView2.Core;
using System;
using System.Windows;
using System.Threading.Tasks;

namespace SplErpDC
{
    public partial class MainWindow : Window
    {
        private Uri? homeUri;

        public MainWindow()
        {
            InitializeComponent();
            this.StateChanged += MainWindow_StateChanged;
            this.Loaded += MainWindow_Loaded;
        }

        private void MainWindow_StateChanged(object sender, EventArgs e)
        {
            if (this.WindowState == WindowState.Normal)
            {
                CenterWindowOnScreen();
            }
        }

        private void CenterWindowOnScreen()
        {
            var screenWidth = SystemParameters.PrimaryScreenWidth;
            var screenHeight = SystemParameters.PrimaryScreenHeight;
            this.Left = (screenWidth - this.Width) / 2;
            this.Top = (screenHeight - this.Height) / 2;
        }

        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            await Task.Delay(1000);

            string savedUrl = Settings.Default.ErpServerUrl;
            await InitializeWebViewWithUrl(savedUrl);
        }

        private async Task InitializeWebViewWithUrl(string? initialUrl)
        {
            string url = initialUrl ?? "";

            while (!IsValidUrl(url))
            {
                var prompt = new UrlPromptWindow();
                if (prompt.ShowDialog() == true)
                {
                    url = prompt.EnteredUrl;
                }
                else
                {
                    return;
                }
            }

            try
            {
                homeUri = new Uri(url);
                Settings.Default.ErpServerUrl = url;
                Settings.Default.Save();

                await webView.EnsureCoreWebView2Async();

                webView.CoreWebView2.Settings.AreDefaultContextMenusEnabled = false;

                webView.CoreWebView2.NavigationStarting -= CoreWebView2_NavigationStarting;
                webView.CoreWebView2.NavigationStarting += CoreWebView2_NavigationStarting;

                webView.CoreWebView2.NavigationCompleted -= CoreWebView2_NavigationCompleted;
                webView.CoreWebView2.NavigationCompleted += CoreWebView2_NavigationCompleted;

                webView.CoreWebView2.NewWindowRequested -= CoreWebView2_NewWindowRequested;
                webView.CoreWebView2.NewWindowRequested += CoreWebView2_NewWindowRequested;

                webView.Source = homeUri;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error initializing WebView2: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Application.Current.Shutdown();
            }
        }


        private bool IsValidUrl(string url)
        {
            return Uri.TryCreate(url, UriKind.Absolute, out Uri? result) &&
                   (result.Scheme == Uri.UriSchemeHttp || result.Scheme == Uri.UriSchemeHttps);
        }

        private async void CoreWebView2_NavigationStarting(object? sender, CoreWebView2NavigationStartingEventArgs e)
        {
            PageLoadingProgress.Visibility = Visibility.Visible;
        }

        private async void CoreWebView2_NavigationCompleted(object? sender, CoreWebView2NavigationCompletedEventArgs e)
        {
            PageLoadingProgress.Visibility = Visibility.Collapsed;
            if (LoadingScreen.Visibility == Visibility.Visible)
                LoadingScreen.Visibility = Visibility.Collapsed;

            if (!e.IsSuccess)
            {
                MessageBox.Show("Failed to load the page.\nPlease check the URL or your internet connection.", "Navigation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void CoreWebView2_NewWindowRequested(object? sender, CoreWebView2NewWindowRequestedEventArgs e)
        {
            e.Handled = true;
            var popup = new PopupWindow(new Uri(e.Uri));
            popup.Show();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (webView.CanGoBack) webView.GoBack();
        }

        private void ForwardButton_Click(object sender, RoutedEventArgs e)
        {
            if (webView.CanGoForward) webView.GoForward();
        }

        private void ReloadButton_Click(object sender, RoutedEventArgs e)
        {
            webView.Reload();
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            webView.Stop();
        }

        private void HomeButton_Click(object sender, RoutedEventArgs e)
        {
            if (homeUri != null)
                webView.Source = homeUri;
        }

        private void ZoomIn_Click(object sender, RoutedEventArgs e)
        {
            webView.ZoomFactor += 0.1;
        }

        private void ZoomOut_Click(object sender, RoutedEventArgs e)
        {
            webView.ZoomFactor -= 0.1;
        }

        private void ResetZoom_Click(object sender, RoutedEventArgs e)
        {
            webView.ZoomFactor = 1.0;
        }

        private void Print_Click(object sender, RoutedEventArgs e)
        {
            webView.CoreWebView2?.ExecuteScriptAsync("window.print();");
        }

        private void AboutButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show($"SPL ERP DC\n" +
                $"Version 1.0.0\n" +
                $"(Desktop application for SPL ERP by WEB SPL.)\n\n" +
                $"Developed by Al Shimbil Khan\n" +
                $"Mobile: +88 01516711976\n" +
                $"Email: shimbilmax@gmail.com", "About", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private async void ChangeUrlButton_Click(object sender, RoutedEventArgs e)
        {
            await InitializeWebViewWithUrl(null);
        }
    }
}
