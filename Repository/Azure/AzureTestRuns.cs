using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using TestViewApp.Repository.Azure.DataModel;

namespace TestViewApp.Repository.Azure
{
    public class AzureTestRuns
    {
        const string msContinuationHeader = "x-ms-continuationtoken";
        readonly string _baseUrl;
        const string _testRunsUrlTemplate = @"_apis/test/runs?minLastUpdatedDate={0}&maxLastUpdatedDate={1}&api-version=6.0";
        const string _testRunsUrlTemplateWithContinuationToken = @"_apis/test/runs?minLastUpdatedDate={0}&maxLastUpdatedDate={1}&continuationToken={2}&api-version=6.0";

        HttpClientHandler authHandler;
        HttpClient httpClient;
        string _continuationToken;
        bool canReadMoreData = true; // при первом запуске всегда можем читать данные

        public AzureTestRuns(string baseUrl)
        {
            _baseUrl = baseUrl;

            authHandler = new HttpClientHandler()
            {
                UseDefaultCredentials = true,
            };
            httpClient = new HttpClient(authHandler);
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            // httpClient.DefaultRequestHeaders.Add("User-Agent", ".NET Foundation Repository Reporter");

            _continuationToken = string.Empty;
        }

        public void Reset()
        {
            canReadMoreData = true;
            _continuationToken = string.Empty;
        }

        public async Task<TestRunList> GetNextData(DateTime dateStart, DateTime dateStop)
        {
            if (!canReadMoreData)
                return new TestRunList();

            string url = prepareTestRunsUrl(dateStart, dateStop, _continuationToken);
            Console.WriteLine(url);

            HttpResponseMessage message = await httpClient.GetAsync(url);
            if (!message.IsSuccessStatusCode)
            {
                throw new Exception(message.ToString());
            }
            var responseStr = await message.Content.ReadAsStringAsync();

            if (message.Headers.Contains(msContinuationHeader))
            {
                _continuationToken = message.Headers.GetValues(msContinuationHeader).First();
                Console.WriteLine(_continuationToken);
                canReadMoreData = true;
            }
            else
            {
                _continuationToken = string.Empty;
                canReadMoreData = false;
            }

            TestRunList? testRuns = JsonConvert.DeserializeObject<TestRunList>(responseStr);

            return testRuns ?? new TestRunList();
        }

        private string prepareTestRunsUrl(DateTime dateStart, DateTime dateStop, string _continuationToken)
        {
            string sDateStart = dateStart.ToString("yyyy-MM-ddTHH:mm:ss");
            string sDateEnd = dateStop.ToString("yyyy-MM-ddTHH:mm:ss");

            if (string.IsNullOrEmpty(_continuationToken))
            {
                return string.Concat(_baseUrl, string.Format(_testRunsUrlTemplate, sDateStart, sDateEnd));
            }

            return string.Concat(_baseUrl, string.Format(_testRunsUrlTemplateWithContinuationToken, sDateStart, sDateEnd, _continuationToken));
        }
    }
}
