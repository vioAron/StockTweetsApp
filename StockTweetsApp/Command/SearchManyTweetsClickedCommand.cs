using System;
using System.Windows.Input;
using StockTweetsApp.ViewModel;

namespace StockTweetsApp.Command
{
    public class SearchManyTweetsClickedCommand : ICommand
    {
        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            var wi = (WatchedInstrumentsViewModel)parameter;
            wi.SearchMany();
        }

        public event EventHandler CanExecuteChanged;
    }
}
