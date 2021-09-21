using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace GitlabStats.Gitlab
{
    public class IssueDto
    {
        [JsonProperty("iid")]
        public int Id { get; set; }

        public string Title { get; set; }

        public MilestoneDto Milestone { get; set; }

        [JsonProperty("time_stats")]
        public TimeStatsDto TimeStats { get; set; }

        [JsonProperty("closed_at")]
        public DateTime? CloseDate { get; set; }

        public override string ToString() 
        {
            return String.Format($"[{Id}] {Title}\n{Milestone}\nTimeStats:{TimeStats}\n\n\n");
        }

        internal MilestoneIssue MapToMilestoneIssue() 
        {
            var issue = new MilestoneIssue()
            {
                Id = Id,
                Title = Title,
                Milestone = Milestone?.Title ?? "<No milestone>",
                TimeStats = new TimeStats()
                {
                    Spent = TimeStats.Spent,
                    Estimate = TimeStats.Estimate,
                    HumanEstimate = TimeStats.HumanEstimate,
                    HumanSpent = TimeStats.HumanSpent
                }
            };

            return issue;
        }
    }

    public class MilestoneDto 
    {
        public string Title { get; set; }

        public override string ToString() 
        {
            return String.Format($"Milestone: {Title}");
        }
    }
    public class TimeStatsDto
    {
        [JsonProperty("time_estimate")]
        public int Estimate { get; set; }

        [JsonProperty("total_time_spent")]
        public int Spent { get; set; }

        [JsonProperty("human_time_estimate")]
        public string HumanEstimate { get; set; }

        [JsonProperty("human_total_time_spent")]
        public string HumanSpent { get; set; }

        public override string ToString()
        {
            return String.Format($"Estimate: {HumanEstimate}, Spent: {HumanSpent}");
        }
    }
}
