using System.Windows.Input;

namespace TestViewApp.ViewModel.Command
{
    public class DeleteTestRunItemCommand : ICommand
    {
        private readonly MainWindowViewModel _mainWindowViewModel;

        public DeleteTestRunItemCommand(MainWindowViewModel mainWindowViewModel)
        {
            _mainWindowViewModel = mainWindowViewModel;
        }

        public bool CanExecute(object? parameter)
        {
            return true;
            //return (_mainWindowViewModel.SelectedTestRunItem != null);
        }

        public event EventHandler? CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public void Execute(object? parameter)
        {
            //var selectedItem = _mainWindowViewModel.SelectedTestRunItem;
            //_mainWindowViewModel.TestRunList.Remove(selectedItem);
        }
    }
}
