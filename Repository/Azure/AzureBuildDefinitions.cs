using Microsoft.TeamFoundation.Build.WebApi;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using TestViewApp.Repository.Azure.DataModel;

namespace TestViewApp.Repository.Azure
{
    public class AzureBuildDefinitions
    {
        readonly string _baseUrl;
        const string _buildDefinitionsListTemplate = @"_apis/build/definitions?api-version=7.1";
        const string _buildDefinitionsListWithPathTemplate = @"_apis/build/definitions?path={0}&api-version=7.1";
        HttpClientHandler authHandler;
        HttpClient httpClient;


        public AzureBuildDefinitions(string baseUrl)
        {
            if (string.IsNullOrEmpty(baseUrl))
            {
                throw new ArgumentNullException(nameof(baseUrl));
            }

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

        public async Task<DefinitionReference[]> GetListData(string path)
        {
            string url = prepareBuildDefinitionsListUrl(path);

            HttpResponseMessage message = await httpClient.GetAsync(url);
            if (!message.IsSuccessStatusCode)
            {
                throw new Exception(message.ToString());
            }

            var responseStr = await message.Content.ReadAsStringAsync();

            var bs = JsonConvert.DeserializeObject<BuildDefinitionList>(responseStr);

            return bs?.value?.ToArray<DefinitionReference>() ?? Array.Empty<DefinitionReference>();
        }

        private string prepareBuildDefinitionsListUrl(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return string.Concat(_baseUrl, _buildDefinitionsListTemplate);
            }
            else
            {
                return string.Concat(_baseUrl, 
                    string.Format(_buildDefinitionsListWithPathTemplate, path));
            }
        }
    }
}
