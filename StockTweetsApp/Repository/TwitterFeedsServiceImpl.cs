using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using LinqToTwitter;
using StockTweetsApp;
using StockTweetsApp.ViewModel;

namespace ulbuzz.core.services
{
    public class TwitterFeedsServiceImpl : TwitterFeedsService
    {
        private readonly DeskLog _logger = Framework.Services.Get<LogService>().Logger<TwitterFeedsServiceImpl>();
        private readonly TwitterContext _twitterCtx;
        private readonly Dictionary<string, ulong> _sinceIds = new Dictionary<string, ulong>();
        public IEnumerable<InstrumentTweets> InstrumentTweetsCache = new List<InstrumentTweets>();

        public IEnumerable<string> DefaultIntruments = new List<string> { "$BNP", "$VOD", "$AAPL", "$MSFT" };

        public TwitterFeedsServiceImpl()
        {
            TwitterAuthorizer.TwitterAuth();

            _twitterCtx = new TwitterContext(SharedState.Authorizer);

            _logger.Info("Instrument tweets cache started!");

            LoadCache();

            _logger.Info("Instrument tweets cache ended!");
        }

        public override IEnumerable<Tweet> GetTweets(string instrumentId, int numberOfDays = NumberOfDays)
        {
            instrumentId = "$" + instrumentId.Replace("$", "");
            return InstrumentTweetsCache.Where(i => i.InstrumentId == instrumentId).
                SelectMany(i => i.Tweets.Where(t => InThePastDays(t, numberOfDays)));
        }

        public override IEnumerable<InstrumentTweets> GetInstrumentFeeds(IEnumerable<string> instrumentIds, int numberOfDays = NumberOfDays)
        {
            return InstrumentTweetsCache.Where(i => instrumentIds.Contains(i.InstrumentId)).Select(i => new InstrumentTweets
            {
                InstrumentId = i.InstrumentId,
                Tweets = i.Tweets.Where(t => InThePastDays(t, numberOfDays))
            });
        }

        private IEnumerable<InstrumentTweets> GetInstrumentFeedsCore(IEnumerable<string> instrumentIds, int numberOfDays = NumberOfDays)
        {
            var list = new List<InstrumentTweets>();

            foreach (var instrumentId in instrumentIds)
            {
                var it = new InstrumentTweets();

                it.InstrumentId = instrumentId;
                it.Tweets = GetTweetsCore(instrumentId, numberOfDays);

                list.Add(it);
            }

            return list;
        }

        private void LoadCache(IEnumerable<string> instrumentIds = null, bool useDefaultInstruments = true, int numberOfDays = NumberOfDays)
        {
            if (!useDefaultInstruments && instrumentIds == null)
                throw new ArgumentNullException("instrumentIds");

            if (useDefaultInstruments)
            {
                InstrumentTweetsCache = GetInstrumentFeedsCore(DefaultIntruments, numberOfDays);
            }
            else
            {
                InstrumentTweetsCache = GetInstrumentFeedsCore(instrumentIds, numberOfDays);
            }
        }

        private IEnumerable<Tweet> GetTweetsCore(string instrumentId, int numberOfDays = NumberOfDays)
        {
            var statuses = new List<Status>();

            ulong maxId = 0;

            List<Tweet> tweets = null;
            try
            {
                Search userStatusResponse;
                if (!_sinceIds.ContainsKey(instrumentId))
                {
                    userStatusResponse = (from search in _twitterCtx.Search
                                          where search.Type == SearchType.Search &&
                                                search.Query == instrumentId &&
                                                search.Count == 100 &&
                                                search.ResultType == ResultType.Recent
                                          select search
                                         ).SingleOrDefault();

                    if (userStatusResponse == null)
                        return Enumerable.Empty<Tweet>();

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
                    if (!InThePastDays(userStatusResponse.Statuses.First(), numberOfDays))
                        break;

                    maxId = userStatusResponse.Statuses.Min(status => status.StatusID) - 1;

                    statuses.AddRange(userStatusResponse.Statuses);

                } while (userStatusResponse.Count >= 100);

                tweets = statuses.Select(status => new Tweet
                {
                    Id = status.StatusID,
                    CreatedAt = status.CreatedAt,
                    Message = status.Text,
                    UserName = status.User.ScreenNameResponse,
                    RetweetedCount = status.RetweetCount
                }).ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            return tweets;
        }

        private static bool InThePastDays(Status tweet, int numberOfDays)
        {
            if (tweet == null)
                throw new ArgumentNullException("tweet");

            return (DateTime.Now.Date - tweet.CreatedAt.Date).TotalDays < numberOfDays;
        }

        private static bool InThePastDays(Tweet tweet, int numberOfDays)
        {
            if (tweet == null)
                throw new ArgumentNullException("tweet");

            return (DateTime.Now.Date - tweet.CreatedAt.Date).TotalDays < numberOfDays;
        }
    }
}
