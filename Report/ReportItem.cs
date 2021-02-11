using System;
using System.Collections.Generic;
using System.Text;

namespace GitlabStats.Report
{
    class ReportItem
    {
        private readonly Issue _issue;

        public int Id { get => _issue.Id; }
        public string Title { get => _issue.Title; }

        public string Milestone { get => _issue.Milestone; }

        public double Estimate { get => _issue.TimeStats?.Estimate/3600.00 ?? 0;  }

        public double Spent { get => _issue.TimeStats?.Spent/3600.00 ?? 0; }

        public string HumanEstimate { get => _issue.TimeStats?.HumanEstimate;  }

        public string HumanSpent { get => _issue.TimeStats?.HumanSpent; }

        public int Diff { get; }

        public string HumanDiff { get; }
        public IssueStatus Status { get; }

        public ReportItem(Issue issue) 
        {
            _issue = issue;

            if (issue.TimeStats != null && issue.TimeStats.Estimate != 0)
            {
                Diff = issue.TimeStats.Spent - issue.TimeStats.Estimate;
                HumanDiff = String.Format($"{Diff / 3600}h");

                var percent = Diff * 100 / issue.TimeStats.Estimate;

                if (percent > 40)
                    Status = IssueStatus.RED;
                else if (percent > 20)
                    Status = IssueStatus.ORANGE;
                else if (percent > -40)
                    Status = IssueStatus.GREEN;
                else
                    Status = IssueStatus.YELLOW;  //underestimated tasks
            }
            else
                Status = IssueStatus.NO;
        }
    }

    enum IssueStatus 
    {
        GREEN,
        YELLOW,
        ORANGE,
        RED,
        NO
    }
}
