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

        public override bool Equals(Object obj)
        {
            //Check for null and compare run-time types.
            if ((obj == null) || !this.GetType().Equals(obj.GetType()))
            {
                return false;
            }
            else
            {
                Issue issue = (Issue)obj;
                return Id == issue.Id;
            }
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public override string ToString()
        {
            return $"{Id} {Title}";
        }
    }

    class TimeStats 
    {
        public int Estimate { get; set; }

        public int Spent { get; set; }

        public string HumanEstimate { get; set; }

        public string HumanSpent { get; set; }
    }
}
