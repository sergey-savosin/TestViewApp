using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using Microsoft.VisualStudio.Services.Common;
using System.Collections.ObjectModel;
using System.Configuration;
using TestViewApp.Domain;
using TestViewApp.Model;
using TestViewApp.Repository.Azure;
using TestViewApp.Repository.Azure.DataModel;
using TestViewApp.UtilityClasses;

namespace TestViewApp.ViewModel
{
    public class BuildDefinitionTestListViewModel : ViewModelBase, IListItem
    {
        private ObservableCollection<BuildDefinitionItem> p_BuildDefinitionList = null!;
        private ObservableCollection<Build> p_BuildList = null!;
        private ObservableCollection<TestRun> p_TestRunList = null!;
        private ObservableCollection<CustomTestCaseResult> p_TestCaseResultList = null!;
        private bool p_LoadInProgress;
        private string p_Name;
        private bool p_ShowOnlyFailedTestCases;
        private int p_TestCasesCount = 0;
        private BuildDefinitionItem selectedBuildDefinitionItem = null!;
        private Build selectedBuildItem = null!;
        private TestRun selectedTestRunItem = null!;
        private CustomTestCaseResult selectedTestCaseResultItem = null!;

        private const string constKormBDeploysPath = @"\Korm B\Tests\**\**";

        public BuildDefinitionTestListViewModel()
        {
            InitializeAsync().ConfigureAwait(false);
        }

        public ObservableCollection<BuildDefinitionItem> BuildDefinitionList
        {
            get { return p_BuildDefinitionList; }
            set
            {
                p_BuildDefinitionList = value;
            }
        }

        public BuildDefinitionItem SelectedBuildDefinitionItem
        {
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
                OnSelectedTestRunItemChanged().ConfigureAwait(false);
            }
        }

        public ObservableCollection<CustomTestCaseResult> TestCaseResultList
        {
            get { return p_TestCaseResultList; }
            set { p_TestCaseResultList = value; }
        }

        public CustomTestCaseResult SelectedTestCaseResultItem
        {
            get { return selectedTestCaseResultItem; }
            set
            {
                selectedTestCaseResultItem = value;
                OnSelectedTestCaseResultItemChanged();
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

        public bool ShowOnlyFailedTestCases
        {
            get { return p_ShowOnlyFailedTestCases; }
            set
            {
                p_ShowOnlyFailedTestCases = value;
                OnSelectedTestRunItemChanged().ConfigureAwait(false);
            }
        }

        public int TestCasesCount
        {
            get { return p_TestCasesCount; }
            set
            {
                p_TestCasesCount = value;
                RaisePropertyChangedEvent(nameof(TestCasesCount));
            }
        }

        public string Name
        {
            get { return p_Name; }
            set
            {
                p_Name = value;
                RaisePropertyChangedEvent(nameof(Name));
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

        private async Task OnSelectedTestRunItemChanged()
        {
            var testRun = SelectedTestRunItem;
            if (testRun != null)
            {
                string outcomesFilter = ShowOnlyFailedTestCases == true ? "failed" : "";
                await LoadTestCaseResultList(testRun.Id, outcomesFilter).ConfigureAwait(false);
                TestCasesCount = TestCaseResultList.Count;
            }
        }

        private void OnSelectedTestCaseResultItemChanged()
        {
            throw new NotImplementedException();
        }

        private async Task InitializeAsync()
        {
            p_BuildDefinitionList = new ObservableCollection<BuildDefinitionItem>();
            p_BuildList = new ObservableCollection<Build>();
            p_TestRunList = new ObservableCollection<TestRun>();
            p_TestCaseResultList = new ObservableCollection<CustomTestCaseResult>();

            await LoadBuildDefinitions();
        }

        public async Task LoadBuildDefinitions()
        {
            LoadInProgress = true;

            var azureBuildDefinition = new AzureBuildDefinitions(GetAzureBaseUrl());
            var definitionArray = await azureBuildDefinition.GetListData(constKormBDeploysPath);
            IEnumerable<BuildDefinitionItem> buildDefUIArray = MakeBuildDefTree(definitionArray);
            BuildDefinitionList.Clear();
            BuildDefinitionList.AddRange(buildDefUIArray);

            LoadInProgress = false;
        }

        private async Task LoadBuildList(int buildDefinitionId)
        {
            LoadInProgress = true;

            var azureBuild = new AzureBuilds(GetAzureBaseUrl());
            var buildArray = await azureBuild.GetListDataByBuildDefinition(buildDefinitionId);
            BuildList.Clear();
            BuildList.AddRange(buildArray);

            LoadInProgress = false;
        }

        private async Task LoadTestRunList(Uri uri)
        {
            LoadInProgress = true;
            var azureTestRuns = new AzureTestRuns(GetAzureBaseUrl());
            var res = await azureTestRuns.GetListDataByBuildUri(uri.ToString());
            TestRunList.Clear();
            TestRunList.AddRange(res);

            LoadInProgress = false;
        }

        private async Task LoadTestCaseResultList(int runId, string outcomesFilter)
        {
            LoadInProgress = true;

            var azureTestCaseResults = new AzureTestCaseResults(GetAzureBaseUrl());
            var res = await azureTestCaseResults.GetListDataByRunId(runId, 0, 100, outcomesFilter);
            TestCaseResultList.Clear();
            TestCaseResultList.AddRange(res);

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

        private static string GetAzureBaseUrl()
        {
            return ConfigurationManager.AppSettings["AzureUrlBase"] ?? throw new Exception("AzureUrlBase is not found in AppSettings");
        }
    }
}
