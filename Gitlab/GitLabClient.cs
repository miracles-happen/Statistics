using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using GitlabStats.Gitlab;
using GitlabStats.MilestoneDiagram;
using GitlabStats.PrerequisiteCheck;
using Microsoft.Extensions.Logging;
using RestEase;

namespace GitlabStats.GitlabApi
{
    class GitLabClient : IIssueStore
    {
        private readonly string _url;
        private readonly IGitlabApi _gitlabApi;
        private readonly ILogger _logger;

        private int _currentPage;
        private int _totalItems;
        private List<IssueDto> _issuesDto;


        public GitLabClient(IOptions options, ILogger<GitLabClient> logger) 
        {
            _logger = logger;
            _url = options.GitlabUrl;
            _gitlabApi = RestClient.For<IGitlabApi>(_url);
            _gitlabApi.AccessToken = options.GitlabAccessToken;

            _currentPage = 1;
            _issuesDto = new List<IssueDto>();
        }

        public async Task<IEnumerable<MilestoneIssue>> FindTasksAsync(DateTime since) 
        {
            var sinceDate = ToIso8601DateString(since);
            await LoadTasksAsync(sinceDate);
            var issues = new List<MilestoneIssue>();

            if (_totalItems != _issuesDto.Count) 
            {
                throw new Exception($"Some error occurred: found items: {_totalItems}, read items: {_issuesDto.Count}");
            }

            foreach (var issueDto in _issuesDto) 
            {
                if (issueDto.CloseDate >= since)
                {
                    issues.Add(issueDto.MapToMilestoneIssue());
                }
                else 
                {
                    _logger.LogTrace($"Skip issue: {issueDto.Id} {issueDto.Title}, closeDate {issueDto.CloseDate}");
                }
            }
            _logger.LogInformation($"Found {_issuesDto.Count} items, skipped {_issuesDto.Count - issues.Count}");
            return issues;
        }

        public async Task<IEnumerable<MilestoneIssue>> FindTasksByMilestoneAsync(string milestone)
        {
            _logger.LogInformation($"Making call to Gitlab for page {_currentPage}.");

            try
            {
                var response = await _gitlabApi.GetIssuesByMilestoneAsync(milestone, _currentPage);

                if (response.ResponseMessage.StatusCode == HttpStatusCode.OK)
                {
                    _issuesDto.AddRange(response.GetContent());

                    var pagesCount = GetTotalPages(response.ResponseMessage.Headers);
                    _totalItems = GetTotalItems(response.ResponseMessage.Headers);

                    if (_currentPage < pagesCount)
                    {
                        _logger.LogWarning($"Milestone has more than 100 issues!! Total pagesCount: {pagesCount}");
                    }
                }
                else
                {
                    _logger.LogError("Request failed: " + response.StringContent);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred on retrieveing data from GitLab");
            }

            var issues = new List<MilestoneIssue>();

            foreach (var issueDto in _issuesDto)
            {
               issues.Add(issueDto.MapToMilestoneIssue());
            }

            _logger.LogInformation($"Found {_issuesDto.Count} items in milestone.");
            return issues;
        }

        public async Task<Issue> GetTaskAsync(int id)
        {
            _logger.LogInformation($"Making call to Gitlab for issue {id}.");

            Issue issue = null;
            try
            {
                var response = await _gitlabApi.GetIssueAsync(id);

                if (response.ResponseMessage.StatusCode == HttpStatusCode.OK)
                {
                    var issueDto = response.GetContent();
                    issue = new Issue(issueDto.Id, issueDto.Title, issueDto.TimeStats.HumanEstimate, issueDto.Milestone?.Title, issueDto.State);
                }
                else
                {
                    _logger.LogError("Request failed: " + response.StringContent);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred on retrieveing data from GitLab");
            }

            return issue;
        }

        public async Task<IEnumerable<IssueLink>> GetRelatedTasksAsync(int id)
        {
            _logger.LogInformation($"Making call to Gitlab for related issues {id}.");

            IEnumerable<IssueLinkDto> issueLinksDto = null;

            try
            {
                var response = await _gitlabApi.GetIssueLinksAsync(id);

                if (response.ResponseMessage.StatusCode == HttpStatusCode.OK)
                {
                    issueLinksDto = response.GetContent();
                }
                else
                {
                    _logger.LogError("Request failed: " + response.StringContent);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred on retrieveing data from GitLab");
            }

            var issueLinks = new List<IssueLink>();

            foreach (var issueLinkDto in issueLinksDto)
            {
                issueLinks.Add(issueLinkDto.MapToIssueLink());
            }

            _logger.LogInformation($"Found {issueLinks.Count} issues.");
            return issueLinks;
        }

        public async Task<IEnumerable<MergeRequest>> GetRelatedMergeRequestsAsync(int id) 
        {
            _logger.LogInformation($"Making call to Gitlab for related merge requests for issue {id}.");

            IEnumerable<MergeRequestDto> mergeRequestsDto = null;

            try
            {
                var response = await _gitlabApi.GetRelatedMergeRequestsForIssueAsync(id);

                if (response.ResponseMessage.StatusCode == HttpStatusCode.OK)
                {
                    mergeRequestsDto = response.GetContent();
                }
                else
                {
                    _logger.LogError("Request failed: " + response.StringContent);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred on retrieveing data from GitLab");
            }

            var mergeRequests = new List<MergeRequest>();

            foreach (var mergeRequestDto in mergeRequestsDto)
            {
                mergeRequests.Add(mergeRequestDto.MapToMergeRequest());
            }

            _logger.LogInformation($"Found {mergeRequests.Count} merge requests.");
            return mergeRequests;
        }

        private string ToIso8601DateString(DateTime dateTime)
        {
            return dateTime.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
        }

        private async Task LoadTasksAsync(string sinceDate) 
        {
            _logger.LogInformation($"Making call to Gitlab for page {_currentPage}.");

            try
            {
                var response = await _gitlabApi.GetIssuesAsync(sinceDate, _currentPage);

                if (response.ResponseMessage.StatusCode == HttpStatusCode.OK)
                {
                    _issuesDto.AddRange(response.GetContent());

                    var pagesCount = GetTotalPages(response.ResponseMessage.Headers);
                    _totalItems = GetTotalItems(response.ResponseMessage.Headers);

                    if (_currentPage < pagesCount)
                    {
                        _currentPage++;
                        await LoadTasksAsync(sinceDate);

                    }
                }
                else
                {
                    _logger.LogError("Request failed: " + response.StringContent);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred on retrieveing data from GitLab");
            }
        }

        private int GetTotalPages(HttpResponseHeaders headers)
        {
            return GetHttpHeaderValue(headers, "X-Total-Pages");
        }

        private int GetTotalItems(HttpResponseHeaders headers)
        {
            return GetHttpHeaderValue(headers, "X-Total");
        }

        private int GetHttpHeaderValue(HttpResponseHeaders headers, string headerName) 
        {
            IEnumerable<string> values;

            if (headers.TryGetValues(headerName, out values))
            {
                var value = values.First();
                return Int32.Parse(value);
            }

            return 0;
        }
    }
}
