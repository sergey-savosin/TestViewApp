using Microsoft.VisualStudio.Services.Common;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Configuration;
using System.Windows.Data;
using System.Windows.Input;
using TestViewApp.Domain;
using TestViewApp.Repository.Azure;
using TestViewApp.UtilityClasses;
using TestViewApp.ViewModel.Command;

namespace TestViewApp.ViewModel
{
    public class MainWindowViewModel : ViewModelBase
    {
        private ObservableCollection<TestRunItem> p_TestRunList;
        private int p_ItemCount;
        private ObservableCollection<BuildDefinitionItem> p_BuildDefinitionList;
        private int p_DeltaInHours;
        private bool p_LoadInProgress;
        private BuildDefinitionItem selectedBuildDefinitionItem;

        public MainWindowViewModel()
        {
            Initialize();
        }

        public ICommand DeleteTestRunItem { get; set; }

        public ICommand ReloadRunItems { get; set; }

        public ObservableCollection<TestRunItem> TestRunList
        {
            get { return p_TestRunList; }
            set
            {
                p_TestRunList = value;
                RaisePropertyChangedEvent(nameof(TestRunList));
                OnTestItemListChanged();
            }
        }

        public ICollectionView TestRunFilteredList
        {
            get
            {
                var source = CollectionViewSource.GetDefaultView(TestRunList);
                source.Filter = p => Filter((TestRunItem)p);
                return source;
            }
        }

        private bool Filter(TestRunItem p)
        {
            if (p == null)
                return false;

            var selectedBuildName = SelectedBuildDefinitionItem?.Name ?? string.Empty;
            if (selectedBuildName == string.Empty || selectedBuildName == "(all)")
                return true;
            
            if (p.BuildDefinitionName == selectedBuildName)
                return true;
            
            return false;
        }

        public ObservableCollection<BuildDefinitionItem> BuildDefinitionList
        {
            get { return p_BuildDefinitionList; }
            set
            {
                p_BuildDefinitionList = value;
            }
        }

        public TestRunItem SelectedTestRunItem { get; set; }

        public BuildDefinitionItem SelectedBuildDefinitionItem {
            get { return selectedBuildDefinitionItem; }
            set
            {
                selectedBuildDefinitionItem = value;
                // RaisePropertyChangedEvent("SelectedBuildDefinitionItem");
                OnSelectedBuildDefinitionItemChanged();
            }
        }

        public int DeltaInHours
        {
            get { return p_DeltaInHours; }
            set
            {
                p_DeltaInHours = value;
                RaisePropertyChangedEvent(nameof(DeltaInHours));
            }
        }

        public bool LoadInProgress
        {
            get { return p_LoadInProgress; }
            set
            {
                p_LoadInProgress = value;
                RaisePropertyChangedEvent(nameof(LoadInProgress));
            }
        }

        public int TestRunItemCount
        {
            get { return p_ItemCount; }
            set
            {
                p_ItemCount = value;
                RaisePropertyChangedEvent(nameof(TestRunItemCount));
            }
        }

        private void OnTestItemListChanged()
        {
            TestRunItemCount = TestRunList.Count;

            List<BuildDefinitionItem> bd = TestRunList
                .Select(t => new BuildDefinitionItem() { Name = t.BuildDefinitionName ?? "-" })
                .DistinctBy(t => t.Name)
                .ToList();
            BuildDefinitionList.Clear();
            BuildDefinitionList.Add(new BuildDefinitionItem() { Name = "(all)" });
            BuildDefinitionList.AddRange(bd);
        }

        private void OnSelectedBuildDefinitionItemChanged()
        {
            TestRunFilteredList.Refresh();
        }

        private async void Initialize()
        {
            DeleteTestRunItem = new DeleteTestRunItemCommand(this);
            ReloadRunItems = new ReloadRunItemsCommand(this);

            p_BuildDefinitionList = new ObservableCollection<BuildDefinitionItem>();
            p_TestRunList = new ObservableCollection<TestRunItem>();
            p_TestRunList.CollectionChanged += OnTestRunList_CollectionChanged;
            DeltaInHours = 12;

            await LoadRunItems(DeltaInHours);
        }

        private void OnTestRunList_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            OnTestItemListChanged();
        }

        public async Task LoadRunItems(int deltaInHours)
        {
            LoadInProgress = true;
            string azureUrlBase = ConfigurationManager.AppSettings["AzureUrlBase"] ?? throw new Exception("AzureUrlBase is not found in AppSettings");
            var atr = new AzureTestInformation(azureUrlBase);

            var testInfoList = await atr.GetTestList(deltaInHours);
            var res = new ObservableCollection<TestRunItem>(testInfoList.Where(t => !t.StageName?.StartsWith("__default") ?? true).ToArray());

            TestRunList.Clear();
            TestRunList.AddRange(res);

            LoadInProgress = false;
        }
    }
}
