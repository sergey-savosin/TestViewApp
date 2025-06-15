using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using TestViewApp.Repository.Azure.DataModel;

namespace TestViewApp.Repository.Azure
{
    public class AzureTestCaseResults
    {
        readonly string _baseUrl;
        const string _testRunsUrlTemplateByUri = @"_apis/test/runs/{0}/results?$skip={1}&$top={2}&outcomes={3}&api-version=7.1";

        readonly HttpClientHandler authHandler;
        readonly HttpClient httpClient;

        public AzureTestCaseResults(string baseUrl)
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

        public async Task<CustomTestCaseResult[]> GetListDataByRunId(int runId, int skip, int top, string outcomes)
        {
            string url = prepareTestCaseResultsListByRunId(runId, skip, top, outcomes);

            HttpResponseMessage message = await httpClient.GetAsync(url);
            if (!message.IsSuccessStatusCode)
            {
                throw new Exception(message.ToString());
            }

            try
            {
                var responseStr = await message.Content.ReadAsStringAsync();
                var bs = JsonConvert.DeserializeObject<TestCaseResultList>(responseStr);
                return bs?.value?.ToArray<CustomTestCaseResult>() ?? Array.Empty<CustomTestCaseResult>();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        private string prepareTestCaseResultsListByRunId(int runId, int skip, int top, string outcomes)
        {
            return string.Concat(_baseUrl, string.Format(_testRunsUrlTemplateByUri, runId, skip, top, outcomes));
        }
    }
}
