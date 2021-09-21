using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using RestEase;

namespace GitlabStats.Jira
{
    [AllowAnyStatusCode]
    public interface IJiraApi
    {
        [Header("Authorization")]
        AuthenticationHeaderValue Authorization { get; set; }

        [Get("rest/api/2/search?jql=project%20%3D%20PMP%20AND%20fixVersion%20%3D%20{version}&maxResults=100&fields=customfield_11798,summary")]
        Task<Response<JiraIssues>> GetIssuesByVersionAsync([Path] string version);
    }
}
