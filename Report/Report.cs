using System;
using System.Collections.Generic;
using System.Text;

namespace GitlabStats.Report
{
    class Report
    {
        public IReadOnlyDictionary<string, IList<ReportItem>> Items { get; }
        public DateTime SinceDate { get; }

        public Report(IReadOnlyDictionary<string, IList<ReportItem>> data, DateTime since) 
        {
            Items = data;
            SinceDate = since;
        }
    }
}
