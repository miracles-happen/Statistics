using System;
using System.Collections.Generic;
using System.Text;

namespace GitlabStats.JiraVersionComparing
{
    class JiraIssue
    {
        public int Id { get; set;}

        public string Key { get; set; }

        public IList<int> ExternalIssues { get; set; }

        public string Summary { get; set; }
    }
}
