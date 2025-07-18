using System.Windows;

namespace SplErpDC
{
    public partial class UrlPromptWindow : Window
    {
        public string EnteredUrl { get; private set; } = "";

        public UrlPromptWindow()
        {
            InitializeComponent();
            urlTextBox.Focus();
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            EnteredUrl = urlTextBox.Text.Trim();
            if (!string.IsNullOrWhiteSpace(EnteredUrl) && IsValidUrl(EnteredUrl))
            {
                this.DialogResult = true;
            }
            else
            {
                MessageBox.Show("Please enter a valid URL.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private bool IsValidUrl(string url)
        {
            return Uri.TryCreate(url, UriKind.Absolute, out Uri? uriResult)
                && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
        }
    }
}
