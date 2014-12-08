using System.Linq;
using System.Windows;
using LinqToTwitter;
using StockTweetsApp.ViewModel;

namespace StockTweetsApp.View
{
    /// <summary>
    /// Interaction logic for FollowingWindow.xaml
    /// </summary>
    public partial class FollowingWindow
    {
        public FollowingWindow()
        {
            InitializeComponent();
        }

        private async void FollowingWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            var twitterCtx = new TwitterContext(SharedState.Authorizer);

            var tweets =
                await
                (from tweet in twitterCtx.Status
                 where tweet.Type == StatusType.Home
                 select new Tweet
                 {
                     ImageSource = tweet.User.ProfileImageUrl,
                     UserName = tweet.User.ScreenNameResponse,
                     Message = tweet.Text
                 })
                .ToListAsync();

            var tweetCollection = ((TweetsViewModel) DataContext).Tweets;
            tweetCollection.Clear();
            tweets.ForEach(tweetCollection.Add);
        }
    }
}
