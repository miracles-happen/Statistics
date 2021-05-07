using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using GitlabStats.Gitlab;
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
            _gitlabApi.AccessToken = options.AccessToken;

            _currentPage = 1;
            _issuesDto = new List<IssueDto>();
        }

        public async Task<IEnumerable<Issue>> FindTasksAsync(DateTime since) 
        {
            var sinceDate = ToIso8601DateString(since);
            await LoadTasksAsync(sinceDate);
            var issues = new List<Issue>();

            if (_totalItems != _issuesDto.Count) 
            {
                throw new Exception($"Some error occurred: found items: {_totalItems}, read items: {_issuesDto.Count}");
            }

            foreach (var issueDto in _issuesDto) 
            {
                if (issueDto.CloseDate >= since)
                {
                    issues.Add(issueDto.MapToIssue());
                }
                else 
                {
                    _logger.LogTrace($"Skip issue: {issueDto.Id} {issueDto.Title}, closeDate {issueDto.CloseDate}");
                }
            }
            _logger.LogInformation($"Found {_issuesDto.Count} items, skipped {_issuesDto.Count - issues.Count}");
            return issues;
        }

        public async Task<IEnumerable<Issue>> FindTasksByMilestoneAsync(string milestone)
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

            var issues = new List<Issue>();

            foreach (var issueDto in _issuesDto)
            {
               issues.Add(issueDto.MapToIssue());
            }

            _logger.LogInformation($"Found {_issuesDto.Count} items in milestone.");
            return issues;
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
