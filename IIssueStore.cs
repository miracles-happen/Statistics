using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GitlabStats
{
    interface IIssueStore
    {
        Task<IEnumerable<Issue>> FindTasksAsync(DateTime since);

        Task<IEnumerable<Issue>> FindTasksByMilestoneAsync(string milestone);
    }
}
