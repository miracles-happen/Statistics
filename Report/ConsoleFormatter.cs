using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Logging;

namespace GitlabStats.Report
{
    class ConsoleFormatter : IReportFormatter
    {
        public void Print(Report report) 
        {
            Console.WriteLine($"Report date: {DateTime.Now}, issues since: {report.SinceDate}");
            foreach (string key in report.Items.Keys) 
            {
                PrintMilestoneData(key, report.Items[key]);
            }
        }

        private void PrintMilestoneData(string milestone, IList<ReportItem> reportItems) 
        {
            Console.WriteLine("****************************************************************************");
            Console.WriteLine($"Milestone: {milestone}");

            foreach (var reportItem in reportItems) 
            {
                Console.WriteLine($"{reportItem.Id}: {reportItem.HumanEstimate}/{reportItem.HumanSpent} ({reportItem.HumanDiff}) - {reportItem.Status}");

            }
            Console.WriteLine("****************************************************************************");
        }
    }
}
