namespace TestViewApp.Repository.Azure.DataModel
{
    public class CustomTestCaseResult
    {
        public double DurationInMs { get; set; }
        public string Outcome { get; set; }
        public string State { get; set; }
        public string ComputerName { get; set; }
        public string Url { get; set; }
        public string TestCaseTitle { get; set; }
        public int TestCaseReferenceId { get; set; }
        public string AutomatedTestName { get; set; }
    }
}
