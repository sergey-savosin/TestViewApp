using Microsoft.TeamFoundation.Build.WebApi;
using TestViewApp.Repository.Azure;

namespace TestViewApp.BusinessLogic
{
    public class BuildDefinitionListProcessor : IListProcessor<DefinitionReference>
    {
        private AzureBuildDefinitions _azureBuildDefinitions;

        public BuildDefinitionListProcessor(AzureBuildDefinitions azureBuildDefinitions)
        {
            _azureBuildDefinitions = azureBuildDefinitions;
        }

        public async Task<DefinitionReference[]> GetListAsync(string path)
        {
            var definitionList = await _azureBuildDefinitions.GetListData(path);

            return definitionList;
        }
    }
}
