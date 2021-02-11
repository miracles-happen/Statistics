using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Logging;

namespace GitlabStats.Report
{
    class ConsoleFormatter : IReportFormatter
    {
        public void Print(IReadOnlyDictionary<string, IList<ReportItem>> data) 
        {
            foreach (string key in data.Keys) 
            {
                PrintMilestoneData(key, data[key]);
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
