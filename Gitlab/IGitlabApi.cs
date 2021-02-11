using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using RestEase;

namespace GitlabStats.Gitlab
{
    [AllowAnyStatusCode]
    public interface IGitlabApi
    {
        [Header("PRIVATE-TOKEN")]
        string AccessToken { get; set; }

        [Get("api/v4/issues?pagination=keyset&per_page=100&scope=all&state=closed&labels=CMS&order_by=updated_at")]
        Task<Response<IEnumerable<IssueDto>>> GetIssuesAsync([Query] string updated_after, [Query] int page);
    }


}
