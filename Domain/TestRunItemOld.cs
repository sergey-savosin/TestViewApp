using TestViewApp.UtilityClasses;

namespace TestViewApp.Domain
{
    public class TestRunItemOld : ObservableObject
    {
        private int _testId;
        private string _testName;
        private string? _stageName;

        public TestRunItemOld(int testId, string testName, string? stageName)
        {
            _testId = testId;
            _testName = testName;
            _stageName = stageName;
        }

        public int TestRunId
        {
            get { return _testId; }
            set
            {
                _testId = value;
                RaisePropertyChangedEvent(nameof(TestRunId));
            }
        }

        public string TestRunName
        {
            get { return _testName; }
            set
            {
                _testName = value;
                RaisePropertyChangedEvent(nameof(TestRunName));
            }
        }

        public string StageName
        {
            get { return _stageName ?? ""; }
            set
            {
                _stageName = value;
                RaisePropertyChangedEvent(nameof(StageName));
            }
        }
    }
}
