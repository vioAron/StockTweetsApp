using System.Configuration;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Threading.Tasks;
using System.Windows;
using LinqToTwitter;
using StockTweetsApp.Repository;
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
            SharedState.Authorizer = auth;

            await Task.Run(() => TwitterFeedsService.Instance.LoadCache());

            IsEnabled = true;
        }

        private void WatchedInstrumentsButton_OnClick(object sender, RoutedEventArgs e)
        {
            var window = new WatchedInstruments();
            window.Show();
        }
    }
}
