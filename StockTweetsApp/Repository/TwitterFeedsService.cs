﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows;
using LinqToTwitter;
using StockTweetsApp.Model;

namespace StockTweetsApp.Repository
{
    public class TwitterFeedsService
    {
        private static readonly Lazy<TwitterFeedsService> _lazyInstance = new Lazy<TwitterFeedsService>();
        private readonly TwitterContext _twitterCtx;
        private readonly Dictionary<string, ulong> _sinceIds = new Dictionary<string, ulong>();
        public IEnumerable<InstrumentTweets> InstrumentTweetsCache = new List<InstrumentTweets>();

        public readonly IEnumerable<string> DefaultIntruments = new List<string> { "$BNP", "$VOD", "$AAPL", "$MSFT" };

        private const int NumberOfDays = 2;

        public TwitterFeedsService()
        {
            _twitterCtx = new TwitterContext(SharedState.Authorizer);
        }

        public static TwitterFeedsService Instance
        {
            get { return _lazyInstance.Value; }
        }

        public IEnumerable<Tweet> GetTweets(string instrumentId, int numberOfDays = NumberOfDays)
        {
            instrumentId = "$" + instrumentId.Replace("$", "");
            return InstrumentTweetsCache.Where(i => i.InstrumentId == instrumentId).
                SelectMany(i => i.Tweets.Where(t => InThePastDays(t, numberOfDays)));
        }

        public IEnumerable<InstrumentTweets> GetInstrumentFeeds(IEnumerable<string> instrumentIds, int numberOfDays = NumberOfDays)
        {
            return InstrumentTweetsCache.Where(i => instrumentIds.Contains(i.InstrumentId)).Select(i => new InstrumentTweets
            {
                InstrumentId = i.InstrumentId,
                Tweets = i.Tweets.Where(t => InThePastDays(t, numberOfDays))
            });
        }

        private IEnumerable<InstrumentTweets> GetInstrumentFeedsCore(IEnumerable<string> instrumentIds, CancellationToken token, int numberOfDays = NumberOfDays)
        {
            var its = new List<InstrumentTweets>();

            foreach (var instrumentId in instrumentIds)
            {
                if (token.IsCancellationRequested)
                {
                    token.ThrowIfCancellationRequested();
                }

                var it = new InstrumentTweets
                {
                    InstrumentId = instrumentId,
                    Tweets = GetTweetsCore(instrumentId, numberOfDays)
                };
                its.Add(it);
            }

            return its;
        }

        public void LoadCache(CancellationToken token, IEnumerable<string> instrumentIds = null, bool useDefaultInstruments = true, int numberOfDays = NumberOfDays)
        {
            if (!useDefaultInstruments && instrumentIds == null)
                throw new ArgumentNullException("instrumentIds");

            if (token.IsCancellationRequested)
            {
                token.ThrowIfCancellationRequested();
            }

            if (useDefaultInstruments)
            {
                InstrumentTweetsCache = GetInstrumentFeedsCore(DefaultIntruments, token, numberOfDays);
            }
            else
            {
                InstrumentTweetsCache = GetInstrumentFeedsCore(instrumentIds, token, numberOfDays);
            }
        }

        private IEnumerable<Tweet> GetTweetsCore(string instrumentId, int numberOfDays = NumberOfDays)
        {
            var statuses = new List<Status>();

            ulong maxId = 0;

            List<Tweet> tweets;
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
                throw;
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
