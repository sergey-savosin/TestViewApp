using Microsoft.TeamFoundation.Build.WebApi;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using TestViewApp.Repository.Azure.DataModel;

namespace TestViewApp.Repository.Azure
{
    public class AzureBuilds
    {
        readonly string _baseUrl;
        const string _buildsUrlTemplate = @"_apis/build/builds/{0}?api-version=6.0";
        const string _buildsListUrlTemplate = @"_apis/build/builds?buildIds={0}&api-version=6.0";
        const string _buildsListByBuildDefinitionUrlTemplate = @"_apis/build/builds?definitions={0}&$top={1}&api-version=7.1";

        HttpClientHandler authHandler;
        HttpClient httpClient;

        public AzureBuilds(string baseUrl)
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

        public async Task<Build[]> GetListData(int[] buildIds)
        {
            string url = prepareBuildsListUrl(buildIds);

            HttpResponseMessage message = await httpClient.GetAsync(url);
            if (!message.IsSuccessStatusCode)
            {
                throw new Exception(message.ToString());
            }

            var responseStr = await message.Content.ReadAsStringAsync();

            var bs = JsonConvert.DeserializeObject<BuildList>(responseStr);

            return bs?.value?.ToArray<Build>() ?? Array.Empty<Build>();
        }

        public async Task<Build[]> GetListDataByBuildDefinition(int buildDefinitionId)
        {
            string url = prepareBuildsListByBuildDefinitionUrl(buildDefinitionId);

            HttpResponseMessage message = await httpClient.GetAsync(url);
            if (!message.IsSuccessStatusCode)
            {
                throw new Exception(message.ToString());
            }

            var responseStr = await message.Content.ReadAsStringAsync();

            var bs = JsonConvert.DeserializeObject<BuildList>(responseStr);

            return bs?.value?.ToArray<Build>() ?? Array.Empty<Build>();
        }

        private string prepareBuildsListUrl(int[] buildIds)
        {
            string strList = string.Join(",", buildIds);
            return string.Concat(_baseUrl, string.Format(_buildsListUrlTemplate, strList));
        }

        private string prepareBuildsListByBuildDefinitionUrl(int buildDefinitionId)
        {
            const int topN = 10;
            return string.Concat(_baseUrl, string.Format(_buildsListByBuildDefinitionUrlTemplate, buildDefinitionId, topN));
        }
    }
}
