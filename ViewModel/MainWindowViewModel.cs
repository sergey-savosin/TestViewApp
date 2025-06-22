using System.Collections.ObjectModel;
using TestViewApp.Model;
using TestViewApp.UtilityClasses;

namespace TestViewApp.ViewModel
{
    public class MainWindowViewModel : ViewModelBase
    {
        private IListItem _selectedItem;
        public ObservableCollection<IListItem> Items { get; } = new ObservableCollection<IListItem>();
        public RelayCommand ShowBuildDefinitionTestListCommand { get; }

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
            ShowBuildDefinitionTestListCommand = new RelayCommand(_ => AddItem(new BuildDefinitionTestListViewModel { Name = "Show BuildDefinition TestList" }));
        }

        private void AddItem(IListItem item)
        {
            Items.Add(item);
            SelectedItem = item;
        }
    }
}
