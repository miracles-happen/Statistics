using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Logging;

namespace GitlabStats.Report
{
    interface IReportBuilder 
    {
        void Build(IEnumerable<Issue> issues);
    }

    class ReportBuilder : IReportBuilder
    {
        private readonly SortedList<string, IList<ReportItem>> _reportData;
        private readonly IReportFormatter _reportFormatter;
        private readonly ILogger<StatisticBuilder> _logger;


        public ReportBuilder(IReportFormatter formatter, ILogger<StatisticBuilder> logger) 
        {
            _reportData = new SortedList<string, IList<ReportItem>>();
            _reportFormatter = formatter;
            _logger = logger;
        }
        public void Build(IEnumerable<Issue> issues) 
        {
            _logger.LogInformation("Build report starts.");

            PrepareReportData(issues);
            _reportFormatter.Print(_reportData);

            _logger.LogInformation("Build report finishes");
        }

        private void PrepareReportData(IEnumerable<Issue> issues) 
        {
            foreach (var issue in issues)
            {
                IList<ReportItem> reportItems;

                if (!_reportData.ContainsKey(issue.Milestone))
                {
                    reportItems = new List<ReportItem>();
                    _reportData.Add(issue.Milestone, reportItems);
                }
                else
                {
                    reportItems = _reportData[issue.Milestone];
                }

                var reportItem = new ReportItem(issue);
                reportItems.Add(reportItem);
            }
        }
    }
}
