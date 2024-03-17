using System.Windows.Input;

namespace TestViewApp.ViewModel.Command
{
    public class FilterTestRunsByBuildCommand : ICommand
    {
        private readonly MainWindowViewModel _mainWindowViewModel;

        public FilterTestRunsByBuildCommand()
        {
            _mainWindowViewModel = new MainWindowViewModel();
        }

        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter)
        {
            throw new NotImplementedException();
        }

        public void Execute(object? parameter)
        {
            var selectedBuild = _mainWindowViewModel.SelectedBuildDefinitionItem;
            //_mainWindowViewModel.TestRunList;
        }
    }
}
