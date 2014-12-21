using System;
using System.Collections.ObjectModel;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading;
using System.Windows.Input;
using StockTweetsApp.Command;
using StockTweetsApp.Model;
using StockTweetsApp.Repository;

namespace StockTweetsApp.ViewModel
{
    public class InstrumentTweetsViewModel
    {
        public ObservableCollection<Tweet> Tweets { get; set; }

        public string SearchText { get; set; }

        public InstrumentTweetsViewModel()
        {
            SearchText = "$AAPL";
            Tweets = new ObservableCollection<Tweet>();
        }

        public ICommand LoadInstrumentTweetsClickedCommand
        {
            get { return new LoadInstrumentTweetsClickedCommand(); }
        }

        public void Load()
        {
            BindInstruments();

            Observable.Timer(TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(10)).
                ObserveOn(new SynchronizationContextScheduler(SynchronizationContext.Current)).
                Subscribe(n => BindInstruments());
        }

        private void BindInstruments()
        {
            Tweets.Clear();

            TwitterFeedsService.Instance.GetTweets(SearchText).Subscribe(t => Tweets.Add(t));
        }
    }
}
