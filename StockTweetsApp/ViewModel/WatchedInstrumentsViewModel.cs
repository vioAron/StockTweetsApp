using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using LinqToTwitter;
using StockTweetsApp.Command;
using StockTweetsApp.Model;
using StockTweetsApp.Repository;

namespace StockTweetsApp.ViewModel
{
    public class WatchedInstrumentsViewModel
    {
        private readonly TwitterContext _twitterCtx;

        public WatchedInstrumentsViewModel()
        {
            SearchText = "$AAPL";
            Tweets = new ObservableCollection<Tweet>();
            _twitterCtx = new TwitterContext(SharedState.Authorizer);
        }

        public ICommand SearchInstrumentsClickedCommand
        {
            get { return new SearchInstrumentsClickedCommand(); }
        }

        public ICommand SearchManyTweetsClickedCommand
        {
            get { return new SearchManyTweetsClickedCommand(); }
        }

        public string SearchText { get; set; }

        public ObservableCollection<Tweet> Tweets { get; set; }

        public async Task Search()
        {
            var searchResponse =
                await
                (from s in _twitterCtx.Search
                 where s.Type == SearchType.Search && s.Query == SearchText && s.ResultType == ResultType.Mixed && s.Count == 999
                 select s)
                    .SingleOrDefaultAsync<Search>();

            Tweets.Clear();

            if (searchResponse != null && searchResponse.Statuses != null)
                searchResponse.Statuses.OrderByDescending(t => t.CreatedAt).ToList().ForEach(tweet =>
                    Tweets.Add(new Tweet { Id = tweet.StatusID, CreatedAt = tweet.CreatedAt, Message = tweet.Text, UserName = tweet.User.ScreenNameResponse, RetweetedCount = tweet.RetweetCount }));
        }

        public void SearchMany()
        {
            Tweets.Clear();

            var observable = TwitterFeedsService.Instance.GetTweets(SearchText);

            observable.Connect();
            observable.ObserveOnDispatcher().Subscribe(t => Tweets.Add(t));
        }
    }
}
