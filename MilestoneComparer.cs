using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace GitlabStats
{
    interface IMilestoneComparer
    {
        public Task RunAsync(string milestone);
    }

    class MilestoneComparer : IMilestoneComparer
    {
        // TODO: refactor   
        private readonly IIssueStore _store;
        private readonly ILogger<MilestoneComparer> _logger;
        private readonly string _googleDocFilePath;
        private readonly List<MilestoneIssue> _googleDocIssues;
        private IEnumerable<MilestoneIssue> _gitlabIssues;


        public MilestoneComparer(IIssueStore store, ILogger<MilestoneComparer> logger)
        {
            _store = store;
            _logger = logger;
            _googleDocFilePath = "..\\..\\..\\Source\\GoogleDocMilestone.txt";
            _googleDocIssues = new List<MilestoneIssue>();
        }

        public async Task RunAsync(string milestone)
        {
            try
            {
                _logger.LogInformation($"Comparer starts for milestone {milestone}");

                _gitlabIssues = await _store.FindTasksByMilestoneAsync(milestone);
                await LoadGoogleDocIssues();

                ListIssuesNotFoundInGitLab();
                ListIssuesNotFoundInGoogleDoc();
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

        private async Task LoadGoogleDocIssues() 
        {
            string line;

            StreamReader file = new StreamReader(_googleDocFilePath);
            while ((line = await file.ReadLineAsync()) != null)
            {
                ProcessLine(line);
            }

            file.Close();
            _logger.LogInformation($"Found {_googleDocIssues.Count} in googleDoc.");
        }

        private void ProcessLine(string line)
        {
            int spaceIndex = line.IndexOf(' ');

            int id = Int32.Parse(line.Substring(0, spaceIndex));
            string title = line.Substring(spaceIndex + 1);

            _googleDocIssues.Add(new MilestoneIssue() { Id = id, Title = title });
        }

        private void ListIssuesNotFoundInGitLab() 
        {
            var sb = new StringBuilder();
            sb.AppendLine("****************************************************");
            sb.AppendLine("Issues not found in GitLab milestone:");
            foreach (var issue in _googleDocIssues) 
            {
                if (!_gitlabIssues.Contains(issue)) 
                {
                    sb.AppendLine($"   - {issue}");
                }
            }

            sb.AppendLine("****************************************************");
            _logger.LogInformation(sb.ToString());
        }

        private void ListIssuesNotFoundInGoogleDoc()
        {
            var sb = new StringBuilder();
            sb.AppendLine("****************************************************");
            sb.AppendLine("Issues not found in Googledoc milestone: ");
            foreach (var issue in _gitlabIssues)
            {
                if (!_googleDocIssues.Contains(issue))
                {
                    sb.AppendLine($"   - {issue}");
                }
            }

            sb.AppendLine("****************************************************");
            _logger.LogInformation(sb.ToString());
        }
    }
}
