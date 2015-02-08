using System;
using System.Windows.Input;
using StockTweetsApp.ViewModel;

namespace StockTweetsApp.Command
{
    public class SearchInstrumentsClickedCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public async void Execute(object parameter)
        {
            var wi = (WatchedInstrumentsViewModel)parameter;
            await wi.Search();
        }
    }
}
