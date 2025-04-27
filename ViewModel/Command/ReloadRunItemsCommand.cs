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
            _mainWindowViewModel.LoadBuildDefinitions().ConfigureAwait(false);
        }
    }
}
