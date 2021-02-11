using System;
using System.Collections.Generic;
using System.Text;

namespace GitlabStats.Report
{
    interface IReportFormatter
    {
        public void Print(IReadOnlyDictionary<string, IList<ReportItem>> data);
    }
}
