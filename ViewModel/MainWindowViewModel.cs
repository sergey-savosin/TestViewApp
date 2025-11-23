using System.Collections.ObjectModel;
using System.Windows;
using TestViewApp.Model;
using TestViewApp.UtilityClasses;

namespace TestViewApp.ViewModel
{
    public class MainWindowViewModel : ViewModelBase
    {
        private IListItem _selectedItem;
        public ObservableCollection<IListItem> Items { get; } = new ObservableCollection<IListItem>();
        public RelayCommand ShowBuildDefinitionTestListCommand { get; }
        public RelayCommand ShowTestDetailsCommand { get; }

        public IListItem SelectedItem
        {
            get => _selectedItem;
            set
            {
                _selectedItem = value;
                RaisePropertyChangedEvent(nameof(SelectedItem));
            }
        }

        public MainWindowViewModel()
        {
            ShowBuildDefinitionTestListCommand = new RelayCommand(_ => AddItem(new BuildDefinitionTestListViewModel { Name = "TestList" }));
            ShowTestDetailsCommand = new RelayCommand(_ => GetTest());
        }

        private void AddItem(IListItem item)
        {
            Items.Add(item);
            SelectedItem = item;
        }

        private void GetTest()
        {
            if (Items.Count == 0)
            {
                MessageBox.Show("Загрузите список BuildDefinition", "Просмотр теста", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK);
                return;
            }
            var buildDefsViewModel = (BuildDefinitionTestListViewModel)Items[0];
            var testCase = buildDefsViewModel.SelectedTestCaseResultItem;
            if (testCase == null)
            {
                MessageBox.Show("Сначала выберите тест (TestCase)", "Просмотр теста", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK);
                return;
            }
        }
    }
}
