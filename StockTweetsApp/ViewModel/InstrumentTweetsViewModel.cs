using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows.Input;
using StockTweetsApp.Annotations;
using StockTweetsApp.Command;
using StockTweetsApp.Model;
using StockTweetsApp.Repository;

namespace StockTweetsApp.ViewModel
{
    public class InstrumentTweetsViewModel : INotifyPropertyChanged
    {
        private string _searchText;
        public ObservableCollection<Tweet> Tweets { get; set; }

        public string SearchText
        {
            get { return _searchText; }
            set
            {
                if (value == _searchText) return;
                _searchText = value;
                OnPropertyChanged();
            }
        }

        public InstrumentTweetsViewModel()
        {
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

            var observable = TwitterFeedsService.Instance.GetTweets(SearchText).Replay();
            
            observable.Subscribe(t => Tweets.Add(t));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
