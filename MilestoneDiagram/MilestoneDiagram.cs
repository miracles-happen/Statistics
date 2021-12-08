using GitlabStats.MilestoneDiagram;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace GitlabStats.MilestoneDiagram
{
    interface IMilestoneDiagram
    {
        public Task RunAsync();
    }

    internal class MilestoneDiagram : IMilestoneDiagram
    {
        private readonly IIssueStore _store;
        private readonly ILogger<MilestoneDiagram> _logger;
        private readonly string _issuesListFilePath;
        private readonly List<Issue> _sourceIssues;
        private readonly List<Issue> _processedIssues;
        private readonly IssueRelations _relations;
        private readonly string _outputPath;

        public MilestoneDiagram(IIssueStore store, ILogger<MilestoneDiagram> logger)
        {
            _store = store;
            _logger = logger;
            _issuesListFilePath = "..\\..\\..\\Source\\IssueDiagram.txt";
            _sourceIssues = new List<Issue>();
            _processedIssues = new List<Issue>();
            _relations = new IssueRelations();
            _outputPath = "Diagram.md";
        }

        public async Task RunAsync()
        {
            try
            {
                _logger.LogInformation("Diagram starts");
                await LoadSourceIssues();
                await ProcessIssuesAsync();
                PrintResults();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "some error ocurred:");
            }
            finally
            {
                _logger.LogInformation("Diagram finishes");
            }
            Console.ReadLine();

        }

        private async Task LoadSourceIssues()
        {
            string line;

            StreamReader file = new StreamReader(_issuesListFilePath);
            while ((line = await file.ReadLineAsync()) != null)
            {
                await ProcessLineAsync(line);
            }

            file.Close();
            _logger.LogInformation($"Found {_sourceIssues.Count} in googleDoc.");
        }

        private async Task ProcessLineAsync(string line)
        {
            int spaceIndex = line.IndexOf(' ');

            int id = Int32.Parse(line.Substring(0, spaceIndex));

            var issue = await _store.GetTaskAsync(id);
           _sourceIssues.Add(issue);
        }

        private async Task ProcessIssuesAsync() 
        {
            foreach (var issue in _sourceIssues) 
            {
                await ProcessIssueAsync(issue);
            }
        }

        private async Task ProcessIssueAsync(Issue issue) 
        {
            if (_processedIssues.Contains(issue))
                return;

            _processedIssues.Add(issue);

            var issueLinks = await _store.GetRelatedTasksAsync(issue.Id);

            foreach (var issueLink in issueLinks) 
            {
                bool relationAdded = false;

                switch (issueLink.LinkType) 
                {
                    case LinkTypes.Blocks:
                        relationAdded = _relations.AddRelation(issueLink.RelatedIssue, issue);
                        break;
                    case LinkTypes.IsBlockedBy:
                        relationAdded = _relations.AddRelation(issue, issueLink.RelatedIssue);
                        break;
                }

                if(relationAdded)
                    await ProcessIssueAsync(issueLink.RelatedIssue);
            }
        }

        private void PrintResults()
        {
            StringBuilder sb = new StringBuilder();
            string result = "";

            BeginDiagram(sb);
            BuildDiagram(sb);
            EndDiagram(sb);

            result = sb.ToString();

            using (StreamWriter sw = new StreamWriter(_outputPath))
            {
                sw.Write(result);
            }

            Console.WriteLine(result);
        }

        private void BeginDiagram(StringBuilder sb) 
        {
            sb.AppendLine("```plantuml");
            sb.AppendLine("@startuml");
            sb.AppendLine();
            sb.AppendLine("digraph world {");
            sb.AppendLine();
            sb.AppendLine("size=\"15, 15\";");
        }

        private void BuildDiagram(StringBuilder sb)
        {
            foreach (var issue in _sourceIssues)
            {
              if (!_relations.HasIssue(issue))
              {
                  sb.AppendLine(issue.ToString());
               }
            }

            foreach (var relation in _relations)
            {
                sb.AppendLine(relation.ToString());
            }
        }

        private void EndDiagram(StringBuilder sb)
        {
            sb.AppendLine("}");
            sb.AppendLine();
            sb.AppendLine("@enduml");
            sb.AppendLine("```");
            sb.AppendLine();
        }
    }
}
