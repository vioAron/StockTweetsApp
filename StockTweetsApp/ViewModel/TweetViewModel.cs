using System.Collections.ObjectModel;

namespace StockTweetsApp.ViewModel
{
    public class TweetViewModel
    {
        public ObservableCollection<Tweet> Tweets { get; set; }

        public TweetViewModel()
        {
            Tweets = new ObservableCollection<Tweet>();
        }        
    }
}
