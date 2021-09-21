using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GitlabStats.JiraVersionComparing;
using Newtonsoft.Json;

namespace GitlabStats.Jira
{
    public class JiraIssues 
    {
        public IEnumerable<JiraIssueDto> Issues { get; set; }
    }
    public class JiraIssueDto
    {
        public int Id { get; set; }

        public string Key { get; set; }

        public JiraFieldsDto Fields { get; set; }

        internal JiraIssue MapToJiraIssue()
        {
            var issue = new JiraIssue()
            {
                Id = Id,
                Key = Key,
                Summary = Fields.Summary,
                ExternalIssues = Fields.ExternalIssues.Split(',').Select(Int32.Parse).ToList(),
            };

            return issue;
        }
    }

    public class JiraFieldsDto 
    {
        [JsonProperty("customfield_11798")]
        public string ExternalIssues { get; set; }

        public string Summary { get; set; }
    }
}
