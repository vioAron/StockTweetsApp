using System.Configuration;
using System.Windows;
using LinqToTwitter;
using StockTweetsApp.View;

namespace StockTweetsApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            IsEnabled = false;
            var auth = new ApplicationOnlyAuthorizer
            {
                CredentialStore = new InMemoryCredentialStore
                {
                    ConsumerKey = ConfigurationManager.AppSettings["consumerKey"],
                    ConsumerSecret = ConfigurationManager.AppSettings["consumerSecret"]
                },
            };

            await auth.AuthorizeAsync();
            IsEnabled = true;
            SharedState.Authorizer = auth;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var window = new FollowingWindow();
            window.Show();
        }

        private void WatchedInstrumentsButton_OnClick(object sender, RoutedEventArgs e)
        {
            var window = new WatchedInstruments();
            window.Show();
        }
    }
}
