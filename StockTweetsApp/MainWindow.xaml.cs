using System;
using System.Configuration;
using System.Reactive.Linq;
using System.Threading;
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
        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private readonly object _cancellationTokenSourceGate = new object();

        public MainWindow()
        {
            InitializeComponent();
        }

        private async void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            WatchedInstrumentsButton.IsEnabled = false;
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
            var token = _cancellationTokenSource.Token;

            try
            {
                await Task.Factory.StartNew(() => TwitterFeedsService.Instance.LoadCache(token),
                       TaskCreationOptions.LongRunning);
                Observable.Timer(TimeSpan.FromSeconds(30), TimeSpan.FromSeconds(30)).Subscribe(time =>
                {
                    TwitterFeedsService.Instance.SyncCache();
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                WatchedInstrumentsButton.IsEnabled = true;
            }
        }

        private void WatchedInstrumentsButton_OnClick(object sender, RoutedEventArgs e)
        {
            var window = new WatchedInstruments();
            window.Show();
        }

        private void CancelCacheButton_OnClick(object sender, RoutedEventArgs e)
        {
            _cancellationTokenSource.Cancel();
        }

        protected override void OnClosed(EventArgs e)
        {
            lock (_cancellationTokenSourceGate)
            {
                _cancellationTokenSource.Dispose();
            }

            base.OnClosed(e);
        }
    }
}
