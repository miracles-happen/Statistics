using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.Extensions.Logging;

namespace GitlabStats.Report
{
    class CsvFormatter : IReportFormatter
    {
        private readonly ILogger<CsvFormatter> _logger;
        private int _totalItems;
        public CsvFormatter(ILogger<CsvFormatter> logger) 
        {
            _logger = logger;
            _totalItems = 0;
        }
        public void Print(Report report)
        {
            try
            {
                StreamWriter sw = new StreamWriter("Result.csv");
                sw.WriteLine($"Report date: {DateTime.Now.ToShortDateString()}, issues since: {report.SinceDate.ToShortDateString()}");
                foreach (string key in report.Items.Keys)
                {
                    PrintMilestoneData(sw, key, report.Items[key]);
                }

                sw.WriteLine($"Total items: {_totalItems}");
                sw.Close();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception on writing CSV: ");
            }
        }
        
        private void PrintMilestoneData(StreamWriter sw, string milestone, IList<ReportItem> reportItems)
        {
            sw.WriteLine();
            sw.WriteLine($"Milestone: {milestone};");

            foreach (var reportItem in reportItems)
            {
                string report = $"{reportItem.Id};{reportItem.Title};{reportItem.HumanEstimate};{reportItem.HumanSpent};{reportItem.HumanDiff};{reportItem.Status};"; // TODO duplication
                if (reportItem.Estimate > 0) 
                {
                    report += $"{reportItem.Estimate.ToString("F2")};{reportItem.Spent.ToString("F2")};";
                }
                sw.WriteLine(report);
                _totalItems++;
            }
            sw.WriteLine();
        }
    }
}
