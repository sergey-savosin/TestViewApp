using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using Microsoft.VisualStudio.Services.Common;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Windows.Input;
using TestViewApp.Domain;
using TestViewApp.Repository.Azure;
using TestViewApp.UtilityClasses;
using TestViewApp.ViewModel.Command;

namespace TestViewApp.ViewModel
{
    public class MainWindowViewModel : ViewModelBase
    {
        private ObservableCollection<BuildDefinitionItem> p_BuildDefinitionList = null!;
        private ObservableCollection<Build> p_BuildList = null!;
        private ObservableCollection<TestRun> p_TestRunList = null!;
        private bool p_LoadInProgress;
        private BuildDefinitionItem selectedBuildDefinitionItem = null!;
        private Build selectedBuildItem = null!;
        private TestRun selectedTestRunItem = null!;

        //private const string constKormBDeploysPath = @"\Korm B\Deploy\SaleListManagement";
        private const string constKormBDeploysPath = @"\Korm B\Tests\*";
        //private const string constKormBDeploysPath = @"\Korm B\Deploy\*";

        public MainWindowViewModel()
        {
            InitializeAsync().ConfigureAwait(false);
        }

        public ICommand DeleteTestRunItem { get; set; }

        public ICommand ReloadRunItems { get; set; }

        public ObservableCollection<BuildDefinitionItem> BuildDefinitionList
        {
            get { return p_BuildDefinitionList; }
            set
            {
                p_BuildDefinitionList = value;
            }
        }

        public BuildDefinitionItem SelectedBuildDefinitionItem {
            get { return selectedBuildDefinitionItem; }
            set
            {
                selectedBuildDefinitionItem = value;
                // RaisePropertyChangedEvent("SelectedBuildDefinitionItem");
                OnSelectedBuildDefinitionItemChanged().ConfigureAwait(false);
            }
        }

        public ObservableCollection<Build> BuildList
        {
            get { return p_BuildList; }
            set { p_BuildList = value; }
        }

        public Build SelectedBuildItem
        {
            get { return selectedBuildItem; }
            set
            {
                selectedBuildItem = value;
                OnSelectedBuildItemChanged().ConfigureAwait(false);
            }
        }

        public ObservableCollection<TestRun> TestRunList
        {
            get { return p_TestRunList; }
            set { p_TestRunList = value; }
        }

        public TestRun SelectedTestRunItem
        {
            get { return selectedTestRunItem; }
            set
            {
                selectedTestRunItem = value;
                OnSelectedTestRunItemChanged();
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

        private async Task OnSelectedBuildDefinitionItemChanged()
        {
            var buildDefinition = SelectedBuildDefinitionItem;
            if (buildDefinition != null)
            {
                await LoadBuildList(buildDefinition.Id);
            }
        }

        private async Task OnSelectedBuildItemChanged()
        {
            var build = SelectedBuildItem;
            if (build != null)
            {
                await LoadTestRunList(build.Uri);
            }
        }

        private void OnSelectedTestRunItemChanged()
        {
            //ToDo
            throw new NotImplementedException();
        }

        private async Task InitializeAsync()
        {
            DeleteTestRunItem = new DeleteTestRunItemCommand(this);
            ReloadRunItems = new ReloadRunItemsCommand(this);

            p_BuildDefinitionList = new ObservableCollection<BuildDefinitionItem>();
            p_BuildList = new ObservableCollection<Build>();
            p_TestRunList = new ObservableCollection<TestRun>();

            await LoadBuildDefinitions();
        }

        public async Task LoadBuildDefinitions()
        {
            LoadInProgress = true;

            string azureUrlBase = ConfigurationManager.AppSettings["AzureUrlBase"] ?? throw new Exception("AzureUrlBase is not found in AppSettings");
            var azureBuildDefinition = new AzureBuildDefinitions(azureUrlBase);
            var definitionArray = await azureBuildDefinition.GetListData(constKormBDeploysPath);
            IEnumerable<BuildDefinitionItem> buildDefUIArray = MakeBuildDefTree(definitionArray);
            BuildDefinitionList.Clear();
            BuildDefinitionList.AddRange(buildDefUIArray);

            LoadInProgress = false;
        }

        private async Task LoadBuildList(int buildDefinitionId)
        {
            LoadInProgress = true;

            string azureUrlBase = ConfigurationManager.AppSettings["AzureUrlBase"] ?? throw new Exception("AzureUrlBase is not found in AppSettings");
            var azureBuild = new AzureBuilds(azureUrlBase);
            var buildArray = await azureBuild.GetListDataByBuildDefinition(buildDefinitionId);
            BuildList.Clear();
            BuildList.AddRange(buildArray);

            LoadInProgress = false;
        }

        private async Task LoadTestRunList(Uri uri)
        {
            LoadInProgress = true;

            string azureUrlBase = ConfigurationManager.AppSettings["AzureUrlBase"] ?? throw new Exception("AzureUrlBase is not found in AppSettings");
            var azureTestRuns = new AzureTestRuns(azureUrlBase);
            var res = await azureTestRuns.GetListDataByBuildUri(uri.ToString());
            TestRunList.Clear();
            TestRunList.AddRange(res);

            LoadInProgress = false;
        }


        private static IEnumerable<BuildDefinitionItem> MakeBuildDefTree(DefinitionReference[] buildDefArray)
        {
            return buildDefArray
                .Select(t => new BuildDefinitionItem
                {
                    Id = t.Id,
                    Name = t.Name,
                    Path = t.Path,
                })
                .OrderBy(t => t.Path)
                .ThenBy(t => t.Name);
        }
    }
}
