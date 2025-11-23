using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using TestViewApp.Model;
using TestViewApp.Repository.Azure;
using TestViewApp.Repository.Azure.DataModel;
using TestViewApp.UtilityClasses;

namespace TestViewApp.ViewModel
{
    public class TestStatisticsViewModel : ViewModelBase, IListItem
    {
        private ObservableCollection<CustomTestRunStatistic> p_TestRunStatisticList = null!;
        private string p_Name;
        private const string constKormTestPath = @"\Korm B\Tests\**\**";

        public TestStatisticsViewModel()
        {
            InitializeAsync().ConfigureAwait(false);
        }

        public string Name
        {
            get { return p_Name; }
            set
            {
                p_Name = value;
                RaisePropertyChangedEvent(nameof(Name));
            }
        }

        public bool LoadInProgress { get; private set; }

        public ObservableCollection<CustomTestRunStatistic> TestRunStatisticList
        {
            get { return p_TestRunStatisticList; }
            set
            {
                p_TestRunStatisticList = value;
            }
        }

        private async Task InitializeAsync()
        {

        }

        private async Task LoadTestRunStatistics()
        {
            LoadInProgress = true;

            var azureTestRunStatistics = new AzureTestStatistics(GetAzureBaseUrl());
            var res = await azureTestRunStatistics.GetListData(constKormTestPath);

            TestRunStatisticList.Clear();
            TestRunStatisticList.AddRange(res);

            LoadInProgress = false;
        }

        private static string GetAzureBaseUrl()
        {
            return ConfigurationManager.AppSettings["AzureUrlBase"] ?? throw new Exception("AzureUrlBase is not found in AppSettings");
        }
    }
}
