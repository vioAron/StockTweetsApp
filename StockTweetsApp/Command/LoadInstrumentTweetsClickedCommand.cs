using System;
using System.Windows.Input;
using StockTweetsApp.ViewModel;

namespace StockTweetsApp.Command
{
    public class LoadInstrumentTweetsClickedCommand : ICommand
    {
        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            ((InstrumentTweetsViewModel) parameter).Load();
        }

        public event EventHandler CanExecuteChanged;
    }
}
