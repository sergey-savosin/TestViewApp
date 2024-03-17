namespace TestViewApp.Domain
{
    public class TestRunItem
    {
        public int TestRunId { get; set; }
        
        public string TestRunName { get; set; }
        
        public string? StageName { get; set; }

        public DateTime StartedDate { get; set; }

        public DateTime? CompletedDate { get; set; }

        public string? BuildRequestedBy { get; set; }

        public string BuildRequestedByInfo => BuildRequestedBy?.Equals("Microsoft.TeamFoundation.System", StringComparison.InvariantCultureIgnoreCase) ?? true ? "-" : BuildRequestedBy;

        public string? BuildDefinitionName { get; set; }

        public int DurationInSeconds => (int)((CompletedDate ?? StartedDate) - StartedDate).TotalSeconds;

        public string? BuildReason { get; set; }

        public int TotalTests { get; set; }

        public int PassedTests { get; set; }

        public int UnanalyzedTests { get; set; }
    }
}
