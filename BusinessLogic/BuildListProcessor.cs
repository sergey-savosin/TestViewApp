using Microsoft.TeamFoundation.Build.WebApi;
using TestViewApp.Repository.Azure;

namespace TestViewApp.BusinessLogic
{
    public class BuildListProcessor
    {
        private AzureBuilds _azureBuilds;

        public BuildListProcessor(AzureBuilds azureBuilds)
        {
            _azureBuilds = azureBuilds;
        }

        public async Task<Build[]> GetListAsync(int buildDefinitionId)
        {
            var res = await _azureBuilds.GetListDataByBuildDefinition(buildDefinitionId);
            return res;
        }
    }
}
