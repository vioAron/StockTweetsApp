using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace StockTweetsApp.Model
{
    public class InstrumentTweets
    {
        private readonly ReplaySubject<Tweet> _subjectTweets = new ReplaySubject<Tweet>();

        public string InstrumentId { get; set; }

        public IObservable<Tweet> TweetsObservable
        {
            get
            {
                return _subjectTweets;
            }
        }

        public void Add(Tweet newTweet)
        {
            _subjectTweets.OnNext(newTweet);
        }

        public void AddRange(IEnumerable<Tweet> tweets)
        {
            foreach (var tweet in tweets)
            {
                _subjectTweets.OnNext(tweet);
            }
        }
    }
}
