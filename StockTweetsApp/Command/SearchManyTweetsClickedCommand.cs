using System;
using System.Windows.Input;
using StockTweetsApp.ViewModel;

namespace StockTweetsApp.Command
{
    public class SearchManyTweetsClickedCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            var wi = (WatchedInstrumentsViewModel)parameter;
            wi.SearchMany();
        }
    }
}
