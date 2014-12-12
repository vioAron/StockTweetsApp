using System;
using System.Collections.Generic;
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
        private readonly TwitterContext _twitterCtx;
        private readonly Dictionary<string, ulong> _sinceIds = new Dictionary<string, ulong>();
        public string SearchText { get; set; }

        private const int NumberOfPastDays = 1;

        public ICommand SearchInstrumentsClickedCommand
        {
            get { return new SearchInstrumentsClickedCommand(); }
        }

        public ICommand SearchManyTweetsClickedCommand
        {
            get { return new SearchManyTweetsClickedCommand(); }
        }

        public WatchedInstrumentsViewModel()
        {
            SearchText = "$AAPL";
            Tweets = new ObservableCollection<Tweet>();
            _twitterCtx = new TwitterContext(SharedState.Authorizer);
        }

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

        private static bool InPastDays(Status tweet)
        {
            if (tweet == null)
                throw new ArgumentNullException("tweet");

            return (DateTime.Now.Date - tweet.CreatedAt.Date).TotalDays < NumberOfPastDays;
        }

        public IEnumerable<Status> GetTweetsForInstrument(string instrumentId)
        {
            var statuses = new List<Status>();

            ulong maxId = 0;

            Search userStatusResponse;

            if (!_sinceIds.ContainsKey(instrumentId))
            {
                userStatusResponse = (
                                         from search in _twitterCtx.Search
                                         where search.Type == SearchType.Search &&
                                               search.Query == instrumentId &&
                                               search.Count == 100 &&
                                               search.ResultType == ResultType.Recent
                                         select search
                                     ).SingleOrDefault();

                if (userStatusResponse == null)
                    return Enumerable.Empty<Status>();

                var sinceId = userStatusResponse.Statuses.Max(search => search.StatusID);
                _sinceIds.Add(instrumentId, sinceId);
                maxId = userStatusResponse.Statuses.Min(search => search.StatusID) - 1;

                statuses.AddRange(userStatusResponse.Statuses);
            }

            do
            {
                userStatusResponse =
                    (from search in _twitterCtx.Search
                     where search.Type == SearchType.Search &&
                           search.Query == instrumentId &&
                           search.Count == 100
                           && search.MaxID == maxId
                     select search)
                        .SingleOrDefault();

                if (userStatusResponse == null)
                    break;
                if (userStatusResponse.Statuses.Count == 0)
                    break;
                if (!InPastDays(userStatusResponse.Statuses.First()))
                    break;

                maxId = userStatusResponse.Statuses.Min(status => status.StatusID) - 1;

                statuses.AddRange(userStatusResponse.Statuses);

            } while (userStatusResponse.Count >= 100);

            return statuses;
        }

        public void SearchMany()
        {
            var statuses = GetTweetsForInstrument(SearchText);

            Tweets.Clear();

            foreach (var status in statuses)
            {
                Tweets.Add(new Tweet
                    {
                        Id = status.StatusID,
                        CreatedAt = status.CreatedAt,
                        SinceId = status.SinceID,
                        Message = status.Text,
                        UserName = status.User.ScreenNameResponse,
                        RetweetedCount = status.RetweetCount
                    });
            }
        }
    }
}
