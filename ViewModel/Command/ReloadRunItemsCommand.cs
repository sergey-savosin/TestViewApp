using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace TestViewApp.ViewModel.Command
{
    public class ReloadRunItemsCommand : ICommand
    {
        private readonly MainWindowViewModel _mainWindowViewModel;

        public ReloadRunItemsCommand(MainWindowViewModel mainWindowViewModel)
        {
            _mainWindowViewModel = mainWindowViewModel;
        }

        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public void Execute(object? parameter)
        {
            int deltaInHours = _mainWindowViewModel.DeltaInHours;
            _mainWindowViewModel.LoadRunItems(deltaInHours).ConfigureAwait(false);
        }
    }
}
