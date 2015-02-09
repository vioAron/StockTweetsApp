using System;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading;
using System.Windows.Controls;

namespace StockTweetsApp.View
{
    /// <summary>
    /// Interaction logic for InstrumentTweetsWindow.xaml
    /// </summary>
    public partial class InstrumentTweetsWindow
    {
        private readonly SynchronizationContextScheduler _uiScheduler;

        public InstrumentTweetsWindow()
        {
            InitializeComponent();

            _uiScheduler = new SynchronizationContextScheduler(SynchronizationContext.Current);

            var textChangedObservable = Observable.FromEventPattern<TextChangedEventHandler, TextChangedEventArgs>(
                ev => txtSearch.TextChanged += ev, ev => txtSearch.TextChanged -= ev);

            textChangedObservable.Select(ep => ((TextBox)ep.Sender).Text).Throttle(TimeSpan.FromSeconds(2)).DistinctUntilChanged().ObserveOnDispatcher().Subscribe(
                args =>
                {
                    Model.Tweets.Clear();
                    var resultObservable = Repository.TwitterFeedsService.Instance.GetTweetsObservable(args);
                    resultObservable.TakeUntil(textChangedObservable).ObserveOn(_uiScheduler).Subscribe(result => Model.Tweets.Add(result));
                });

            Model.SearchText = "$AAPL";
        }
    }
}
