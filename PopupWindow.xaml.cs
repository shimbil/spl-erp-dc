using Microsoft.Web.WebView2.Core;
using System;
using System.Windows;

namespace SplErpDC
{
    public partial class PopupWindow : Window
    {
        public PopupWindow(Uri uri)
        {
            InitializeComponent();
            this.Loaded += async (s, e) =>
            {
                try
                {
                    await popupWebView.EnsureCoreWebView2Async();
                    popupWebView.Source = uri;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error loading popup: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    this.Close();
                }
            };
        }
    }
}
