using System;
using System.Windows.Input;
using StockTweetsApp.ViewModel;

namespace StockTweetsApp.Command
{
    public class SearchInstrumentsClickedCommand : ICommand
    {
        public bool CanExecute(object parameter)
        {
            return true;
        }

        public async void Execute(object parameter)
        {
            var wi = (WatchedInstrumentsViewModel)parameter;
            await wi.Search();
        }

        public event EventHandler CanExecuteChanged;
    }
}
