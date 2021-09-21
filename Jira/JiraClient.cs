using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using GitlabStats.JiraVersionComparing;
using Microsoft.Extensions.Logging;
using RestEase;

namespace GitlabStats.Jira
{
    interface IJiraStore 
    {
        Task<IEnumerable<JiraIssue>> FindTasksByVersionAsync(string version);
    }

    class JiraClient : IJiraStore
    {
        private readonly string _url;
        private readonly IJiraApi _jiraApi;
        private readonly ILogger _logger;

        public JiraClient(IOptions options, ILogger<JiraClient> logger)
        {
            _logger = logger;
            _url = options.JiraUrl;


            var httpClientHandler = new HttpClientHandler();
            httpClientHandler.ServerCertificateCustomValidationCallback = (message, cert, chain, sslPolicyErrors) =>
            {
                return true;
            };

            var httpClient = new HttpClient(httpClientHandler) { BaseAddress = new Uri(_url) };

            _jiraApi = RestClient.For<IJiraApi>(httpClient);
            var value = Convert.ToBase64String(Encoding.ASCII.GetBytes(options.JiraBasicAuth));
            _jiraApi.Authorization = new AuthenticationHeaderValue("Basic", value);
        }

        public async Task<IEnumerable<JiraIssue>> FindTasksByVersionAsync(string version)
        {
            _logger.LogInformation($"Making call to JIRA for version {version}.");

            var issues = new List<JiraIssue>();

            try
            {
                var response = await _jiraApi.GetIssuesByVersionAsync(version);

                if (response.ResponseMessage.StatusCode == HttpStatusCode.OK)
                {
                    var result = response.GetContent();
                    foreach (var issueDto in result.Issues)
                    {
                        issues.Add(issueDto.MapToJiraIssue());
                    }
                }
                else
                {
                    _logger.LogError("Request failed: " + response.StringContent);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred on retrieveing data from JIRA");
            }

            _logger.LogInformation($"Found {issues.Count} items.");
            return issues;
        }

    }
}
