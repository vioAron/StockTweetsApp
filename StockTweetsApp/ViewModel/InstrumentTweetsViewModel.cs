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
        public InstrumentTweetsViewModel()
        {
            Tweets = new ObservableCollection<Tweet>();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public ICommand LoadInstrumentTweetsClickedCommand
        {
            get { return new LoadInstrumentTweetsClickedCommand(); }
        }

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

        public ObservableCollection<Tweet> Tweets { get; set; }
        public void Load()
        {
            BindInstruments();

            Observable.Timer(TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(10)).
                ObserveOn(new SynchronizationContextScheduler(SynchronizationContext.Current)).
                Subscribe(n => BindInstruments());
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        private void BindInstruments()
        {
            Tweets.Clear();

            var observable = TwitterFeedsService.Instance.GetTweetsObservable(SearchText);

            observable.Subscribe(t => Tweets.Add(t));
        }
    }
}
