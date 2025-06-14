using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using TestViewApp.Repository.Azure.DataModel;

namespace TestViewApp.Repository.Azure
{
    public class AzureTestRuns
    {
        readonly string _baseUrl;
        const string _testRunsUrlTemplateByUri = @"_apis/test/runs?buildUri={0}&$top={1}&includeRunDetails=true&api-version=7.1";

        readonly HttpClientHandler authHandler;
        readonly HttpClient httpClient;

        public AzureTestRuns(string baseUrl)
        {
            if (string.IsNullOrWhiteSpace(baseUrl))
                throw new ArgumentNullException(nameof(baseUrl));

            _baseUrl = baseUrl;

            authHandler = new HttpClientHandler()
            {
                UseDefaultCredentials = true,
            };
            httpClient = new HttpClient(authHandler);
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.Add("User-Agent", ".NET Foundation Repository Reporter");
        }

        public async Task<TestRun[]> GetListDataByBuildUri(string buildUri)
        {
            string url = prepareTestRunsListByBuildUri(buildUri);

            HttpResponseMessage message = await httpClient.GetAsync(url);
            if (!message.IsSuccessStatusCode)
            {
                throw new Exception(message.ToString());
            }

            var responseStr = await message.Content.ReadAsStringAsync();

            var bs = JsonConvert.DeserializeObject<TestRunList>(responseStr);

            return bs?.value?.ToArray<TestRun>() ?? Array.Empty<TestRun>();
        }

        private string prepareTestRunsListByBuildUri(string buildUri)
        {
            const int topN = 100;
            return string.Concat(_baseUrl, string.Format(_testRunsUrlTemplateByUri, buildUri, topN));
        }

    }
}
