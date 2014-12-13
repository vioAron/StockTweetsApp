using System.Collections.Generic;

namespace StockTweetsApp.Model
{
    public class InstrumentTweets
    {
        public string InstrumentId { get; set; }
        public IEnumerable<Tweet> Tweets { get; set; }
    }
}
