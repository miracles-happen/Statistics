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

        [Get("api/v4/issues?pagination=keyset&per_page=100&scope=all")]
        Task<Response<IEnumerable<IssueDto>>> GetIssuesByMilestoneAsync([Query] string milestone, [Query] int page);

        [Get("api/v4/projects/169/issues/{id}")]
        Task<Response<IssueDto>> GetIssueAsync([Path] int id);

        [Get("api/v4/projects/169/issues/{id}/links")]
        Task<Response<IEnumerable<IssueLinkDto>>> GetIssueLinksAsync([Path] int id);

        [Get("api/v4/projects/169/issues/{id}/related_merge_requests")]
        Task<Response<IEnumerable<MergeRequestDto>>> GetRelatedMergeRequestsForIssueAsync([Path] int id);
    }


}
