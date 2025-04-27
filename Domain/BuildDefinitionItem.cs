namespace TestViewApp.Domain
{
    public class BuildDefinitionItem
    {
        public BuildDefinitionItem()
        {
            Children = [];
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public string Path { get; set; }

        public IList<BuildDefinitionItem> Children { get; set; }
    }
}
