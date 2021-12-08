using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GitlabStats.Jira;
using Microsoft.Extensions.Logging;

namespace GitlabStats.JiraVersionComparing
{
    interface IJiraVersionComparer
    {
        public Task RunAsync(string milestone);
    }

    class JiraVersionComparer :IJiraVersionComparer
    {
        private readonly IJiraStore _jiraStore;
        private readonly IIssueStore _gitLabStore;
        private readonly ILogger<JiraVersionComparer> _logger;
        private readonly string _gitLabPrefix;
        private readonly string _jiraLink;

        private IEnumerable<JiraIssue> _jiraIssues;
        private IEnumerable<MilestoneIssue> _gitlabIssues;


        public JiraVersionComparer(IJiraStore jiraStore, IIssueStore gitLabStore, ILogger<JiraVersionComparer> logger)
        {
            _jiraStore = jiraStore;
            _gitLabStore = gitLabStore;
            _logger = logger;

            _gitLabPrefix = "ПМП-";
            _jiraLink = "https://jira.nspk.ru/browse/";
        }

        public async Task RunAsync(string milestone)
        {
            try
            {
                _logger.LogInformation($"Comparer starts for milestone {milestone}");

                _jiraIssues = await _jiraStore.FindTasksByVersionAsync(milestone);
                _gitlabIssues = await _gitLabStore.FindTasksByMilestoneAsync(_gitLabPrefix + milestone);

                ListIssuesNotFoundInJira();
                await ListIssuesNotFoundInGitLabAsync(milestone);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "some error ocurred:");
            }
            finally
            {
                _logger.LogInformation("Comparer finishes");
            }
            Console.ReadLine();

        }

        private void ListIssuesNotFoundInJira()
        {
            var sb = new StringBuilder();
            sb.AppendLine("****************************************************");
            sb.AppendLine("Issues not found in Jira milestone: ");

            foreach (var gitLabIssue in _gitlabIssues)
            {
                if (HasLink2NSPK(gitLabIssue))
                {
                    var jiraLink = GetLink2NSPK(gitLabIssue);

                    if (!JiraListHasIssue(jiraLink))
                    {
                        sb.AppendLine($"   - {jiraLink} from GitLab GitLab Issue \"{gitLabIssue}\"");
                    }
                }
            }

            sb.AppendLine("****************************************************");
            _logger.LogInformation(sb.ToString());
        }

        private bool JiraListHasIssue(string jiraLink)
        {
            return _jiraIssues.Where(jiraIssue => jiraIssue.Key.Equals(jiraLink)).Any();
        }

        private bool HasLink2NSPK(MilestoneIssue gitLabIssue) 
        {
            return gitLabIssue.Description.Contains(_jiraLink);
        }

        private string GetLink2NSPK(MilestoneIssue gitLabIssue)
        {
            int startIndex = gitLabIssue.Description.IndexOf(_jiraLink);
            int endIndex = gitLabIssue.Description.IndexOf("\n", startIndex);

            if (endIndex < 0)
                throw new InvalidOperationException("Invalid endIndex");

            var jiraLink = gitLabIssue.Description.Substring(startIndex + _jiraLink.Length, endIndex - startIndex - _jiraLink.Length);

            if (jiraLink.EndsWith(")"))
                jiraLink = jiraLink.Substring(0, jiraLink.Length - 1);

            var references = jiraLink.Split(' ');

            return references[0];
        }

        private async Task ListIssuesNotFoundInGitLabAsync(string milestone)
        {
            var sb = new StringBuilder();
            sb.AppendLine("****************************************************");
            sb.AppendLine("Issues not found in GitLab milestone:");
            foreach (var jiraIssue in _jiraIssues)
            {
                foreach (var externalRef in jiraIssue.ExternalIssues) 
                {
                    if (!GitLabListHasIssue(externalRef))
                    {
                        var detailedGitLabIssue = await _gitLabStore.GetTaskAsync(externalRef);

                        if (!IsAnalyticsIssue(detailedGitLabIssue)) 
                        {
                            sb.AppendLine($"   - {jiraIssue.Key}: {externalRef} not found in {milestone} milestone. {externalRef} with state {detailedGitLabIssue.State} actually present in milestone {detailedGitLabIssue.Milestone}");
                        }
                    }
                }
            }

            sb.AppendLine("****************************************************");
            _logger.LogInformation(sb.ToString());
        }

        private bool GitLabListHasIssue(int id) 
        {
            return _gitlabIssues.Where(gitLabIssue => gitLabIssue.Id.Equals(id)).Any();
        }

        private bool IsAnalyticsIssue(Issue issue) 
        {
            if (issue.Title.EndsWith("(analytics)") || issue.Title.EndsWith("(analysis)"))
                return true;
            else
                return false;
        }
    }
}
