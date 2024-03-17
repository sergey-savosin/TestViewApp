using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using TestViewApp.Domain;
using TestViewApp.Repository.Azure.DataModel;

namespace TestViewApp.Repository.Azure
{
    public class AzureTestInformation
    {
        readonly string _urlBase;

        readonly AzureBuilds _azureBuilds;

        public AzureTestInformation(string urlBase)
        {
            _urlBase = urlBase;
            _azureBuilds = new AzureBuilds(urlBase);
        }

        public async Task<List<TestRunItem>> GetTestList(int deltaInHours)
        {
            DateTime dt = DateTime.Now;
            DateTime dateStart = dt.AddHours(-deltaInHours);
            DateTime dateStop = dt.AddDays(1);

            var azureTestRuns = new AzureTestRuns(_urlBase);
            return await ProcessRepositoriesAsync(azureTestRuns, dateStart, dateStop);
        }

        private async Task<List<TestRunItem>> ProcessRepositoriesAsync(AzureTestRuns azureTestRuns, DateTime dateStart, DateTime dateStop)
        {
            var testRunList = new List<TestRun>();
            const int deltaHours = 24;
            DateTime dt1, dt2;

            dt1 = dateStart;
            dt2 = dt1.AddHours(deltaHours).AddSeconds(-1);

            while (dt2 < dateStop)
            {
                Console.WriteLine($"dt: {dt1} - {dt2}");

                var testRuns = await ProcessTestRunsAsync(azureTestRuns, dt1, dt2);
                testRunList.AddRange(testRuns);

                dt1 = dt1.AddHours(deltaHours);
                dt2 = dt2.AddHours(deltaHours);
            }

            HashSet<int> buildIdList = GetBuildIdList(testRunList);
            var testDataList = await AddBuildToTestData(testRunList, buildIdList);

            var res = testDataList.Select(t =>
                new TestRunItem()
                {
                    TestRunId = t.TestRun.Id,
                    TestRunName = t.TestRun.Name,
                    StageName = t.TestRun.PipelineReference?.StageReference?.StageName,
                    BuildReason = t.Build?.Reason.ToString(),
                    BuildDefinitionName = t.Build?.Definition?.Name,
                    BuildRequestedBy = t.Build?.RequestedBy?.DisplayName,
                    StartedDate = t.TestRun.StartedDate,
                    CompletedDate = t.TestRun.CompletedDate,
                    TotalTests = t.TestRun.TotalTests,
                    PassedTests = t.TestRun.PassedTests,
                    UnanalyzedTests = t.TestRun.UnanalyzedTests
                });
            return res.ToList();
        }

        private async Task<List<TestRun>> ProcessTestRunsAsync(AzureTestRuns azureTestRuns, DateTime dateStart, DateTime dateStop)
        {
            var testRunList = new List<TestRun>();

            azureTestRuns.Reset();

            do
            {
                var testRuns = await azureTestRuns.GetNextData(dateStart, dateStop);
                Console.WriteLine($"{testRuns?.value?.Count() ?? 0}.");
                if (testRuns == null || testRuns.value == null)
                    break;

                testRunList.AddRange(testRuns.value);
            } while (1 == 1);

            return testRunList;
        }

        private HashSet<int> GetBuildIdList(List<TestRun> testRunList)
        {
            HashSet<int> result = new HashSet<int>();

            foreach (var testRun in testRunList)
            {
                var buildId = testRun.BuildConfiguration?.Id ?? 0;

                result.Add(buildId);
            }

            return result;
        }

        private async Task<List<TestInformationItem>> AddBuildToTestData(List<TestRun> testRunList, HashSet<int> buildIdList)
        {
            int page = 1;
            IEnumerable<int> list;
            List<Build> buildList = new List<Build>();

            while (1 == 1)
            {
                list = ReturnList(buildIdList, page);
                if (!list.Any())
                    break;

                var builds = await _azureBuilds.GetListData(list.ToArray());
                buildList.AddRange(builds);
                page++;
            }

            var res = testRunList.Join(
                buildList,
                t => t.BuildConfiguration?.Id,
                b => b?.Id,
                (testRun, build) => new TestInformationItem { TestRun = testRun, Build = build }
                );

            return res.ToList();
        }

        private IEnumerable<int> ReturnList(HashSet<int> mylist, int page)
        {
            const int N = 10;
            return mylist.Skip(N * (page - 1)).Take(N);
        }

        private void ShowTestRuns(List<TestInformationItem> testDataList)
        {
            int cnt = testDataList.Count;
            int lineNumber = 0;
            Console.WriteLine($"--- records found: {cnt} ---");

            foreach (var testData in testDataList)
            {
                var testRun = testData.TestRun;
                var build = testData.Build;

                PipelineReference? pr = testRun?.PipelineReference;
                if (testRun?.PipelineReference?.StageReference?.StageName == "__default")
                    continue;
                lineNumber++;

                var start = testRun?.StartedDate;
                var stop = testRun?.CompletedDate;
                var duration = stop - start;
                var requestedBy = build?.RequestedBy?.DisplayName;
                if (requestedBy == "Microsoft.TeamFoundation.System")
                    requestedBy = "-";

                Console.Write($"{lineNumber} [{testRun?.StartedDate}]");
                Console.Write($"DN: {build?.Definition?.Name} Nm: {testRun?.Name} ");

                Console.Write($" PSN: {testRun?.PipelineReference?.StageReference?.StageName}");
                Console.Write(" => dur: {0:0}m ({1:0}s)", duration?.TotalMinutes, duration?.TotalSeconds);
                Console.Write($" RB: {requestedBy} RSN: {build?.Reason}");

                Console.WriteLine($" res: {testRun?.TotalTests} -> {testRun?.PassedTests}, {testRun?.UnanalyzedTests}");
            }
        }
    }
}
