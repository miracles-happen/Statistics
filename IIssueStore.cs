using GitlabStats.MilestoneDiagram;
using GitlabStats.PrerequisiteCheck;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GitlabStats
{
    interface IIssueStore
    {
        Task<IEnumerable<MilestoneIssue>> FindTasksAsync(DateTime since);

        Task<IEnumerable<MilestoneIssue>> FindTasksByMilestoneAsync(string milestone);

        Task<Issue> GetTaskAsync(int id);
        Task<IEnumerable<IssueLink>> GetRelatedTasksAsync(int id);

        Task<IEnumerable<MergeRequest>> GetRelatedMergeRequestsAsync(int id);

    }
}
