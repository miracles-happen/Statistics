using System;
using System.Collections.Generic;
using System.Text;

namespace GitlabStats
{
    class Issue
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Milestone { get; set; }

        public TimeStats TimeStats { get; set; }
    }

    class TimeStats 
    {
        public int Estimate { get; set; }

        public int Spent { get; set; }

        public string HumanEstimate { get; set; }

        public string HumanSpent { get; set; }
    }
}
