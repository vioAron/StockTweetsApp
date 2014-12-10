using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LinqToTwitter;

namespace StockTweetsApp.ViewModel
{
    public class PopularInstrumentsViewModel
    {
        public async void GeneratePopularInstruments()
        {
            var twitterCtx = new TwitterContext(SharedState.Authorizer);

            var searchResponse =
                await
                (from s in twitterCtx.Search
                 where s.Type == SearchType.Search && s.Query == "" && s.Count == 1000 && s.ResultType == ResultType.Recent
                 select s)
                    .SingleOrDefaultAsync();
        }
    }
}
