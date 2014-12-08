using System.Collections.ObjectModel;

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
