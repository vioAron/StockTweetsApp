using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using LinqToTwitter;
using StockTweetsApp.View;

namespace StockTweetsApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();


        }

        private async void ConnectButton_OnClick(object sender, RoutedEventArgs e)
        {
            //var twitterCtx = new TwitterContext();

            //var searchResponse =
            //    await
            //    (from search in twitterCtx.Search
            //     where search.Type == SearchType.Search &&
            //           search.Query == "\"LINQ to Twitter\""
            //     select search)
            //    .SingleOrDefaultAsync();

            //if (searchResponse != null && searchResponse.Statuses != null)
            //    searchResponse.Statuses.ForEach(tweet =>
            //        Console.WriteLine(
            //            "User: {0}, Tweet: {1}", 
            //            tweet.User.ScreenNameResponse,
            //            tweet.Text));
        }

        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            if (SharedState.Authorizer == null)
                new OAuth().Show();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            FollowingWindow window = new FollowingWindow();
            window.Show();
        }
    }
}
