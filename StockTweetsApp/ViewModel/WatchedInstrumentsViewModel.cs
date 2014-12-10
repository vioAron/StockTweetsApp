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
            SearchText = "$AAPL";
            Tweets = new ObservableCollection<Tweet>();
        }
        private readonly TwitterContext _twitterCtx = new TwitterContext(SharedState.Authorizer);
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
                    Tweets.Add(new Tweet { Id = tweet.StatusID, CreatedAt = tweet.CreatedAt, SinceId = tweet.SinceID, Message = tweet.Text, UserName = tweet.User.ScreenNameResponse, RetweetedCount = tweet.RetweetCount }));
        }

        public void SearchMany()
        {
            var maxId = ulong.MaxValue;
            var sinceId = (ulong)341350918903701507;

            var searchResult =
            (
                from search in _twitterCtx.Search
                where search.Type == SearchType.Search &&
                      search.Query == "$AAPL" &&
                      search.Count == 100 &&
                      search.ResultType == ResultType.Recent
                select search
            ).SingleOrDefault<Search>();

            var resultList = searchResult.Statuses;

            sinceId = resultList.Last().StatusID - 1;

            var userStatusResponse =
                (from search in _twitterCtx.Search
                    where search.Type == SearchType.Search &&
                          search.Query == "$AAPL" &&
                          search.SinceID == sinceId &&
                          search.Count == 100
                    select search)
                    .ToList();

            //resultList.AddRange(userStatusResponse.ForEach());

            // first tweet processed on current query
            //maxId = userStatusResponse.Min(
            //    status => ulong.Parse(status.StatusID)) - 1;
        }
    }
}
