using System;
using System.Collections.Generic;
using System.Text;

namespace GitlabStats.Report
{
    interface IReportFormatter
    {
        public void Print(Report report);
    }
}
