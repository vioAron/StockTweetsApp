using System.Collections.ObjectModel;
using StockTweetsApp.Model;

namespace StockTweetsApp.ViewModel
{
    public class TweetsViewModel
    {
        public ObservableCollection<Tweet> Tweets { get; set; }

        public TweetsViewModel()
        {
            Tweets = new ObservableCollection<Tweet>();
        }        
    }
}
