using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.TeamFoundation.TestManagement.WebApi;

namespace TestViewApp.Repository.Azure.DataModel
{
    public class TestInformationItem
    {
        public TestRun TestRun { get; set; }

        public Build? Build { get; set; }

    }
}
