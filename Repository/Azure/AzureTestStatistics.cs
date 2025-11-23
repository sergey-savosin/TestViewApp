using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestViewApp.Repository.Azure.DataModel;

namespace TestViewApp.Repository.Azure
{
    public class AzureTestStatistics
    {
        readonly string _baseUrl;

        public AzureTestStatistics(string baseUrl)
        {
            if (string.IsNullOrWhiteSpace(baseUrl))
                throw new ArgumentNullException(nameof(baseUrl));

            _baseUrl = baseUrl;
        }

        public async Task<CustomTestRunStatistic[]> GetListData(string testPath)
        {
            return null;
        }
    }
}
