using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StockTweetsApp.ViewModel;

namespace StockTweetsApp.Model
{
    public class InstrumentTweets
    {
        public string InstrumentId { get; set; }
        public IEnumerable<Tweet> Tweets { get; set; }
    }
}
