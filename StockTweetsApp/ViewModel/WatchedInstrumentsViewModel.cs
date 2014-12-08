using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using LinqToTwitter;
using StockTweetsApp.Command;

namespace StockTweetsApp.ViewModel
{
    public class WatchedInstrumentsViewModel
    {
        public ObservableCollection<Tweet> Tweets { get; set; }

        public string SearchText { get; set; }

        public ICommand SearchInstrumentsClickedCommand
        {
            get { return new SearchInstrumentsClickedCommand(); }
        }

        public WatchedInstrumentsViewModel()
        {
            Tweets = new ObservableCollection<Tweet>();
        }

        public async Task Search()
        {
            var twitterCtx = new TwitterContext(SharedState.Authorizer);

            var searchResponse =
                await
                (from s in twitterCtx.Search
                 where s.Type == SearchType.Search && s.Query == SearchText && s.Count == 10000 && s.ResultType == ResultType.Recent
                 select s)
                    .SingleOrDefaultAsync();
            
            Tweets.Clear();
            
            if (searchResponse != null && searchResponse.Statuses != null)
                searchResponse.Statuses.OrderByDescending(t => t.RetweetCount).ToList().ForEach(tweet =>
                    Tweets.Add(new Tweet { Message = tweet.Text, UserName = tweet.User.ScreenNameResponse, RetweetedCount = tweet.RetweetCount }));
        }
    }
}
