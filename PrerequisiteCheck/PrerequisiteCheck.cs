using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace GitlabStats.PrerequisiteCheck
{
    interface IPrerequisiteCheck
    {
        public Task RunAsync();
    }

    class PrerequisiteCheck : IPrerequisiteCheck
    {
        private readonly IIssueStore _store;
        private readonly ILogger<PrerequisiteCheck> _logger;
        private readonly int _issueId;
        private readonly string _issueBranch;
        private readonly IEnumerable<string> _branchesForPorting;
        private IEnumerable<MergeRequest> _mergeRequests;

        public PrerequisiteCheck(IIssueStore store, ILogger<PrerequisiteCheck> logger)
        {
            _store = store;
            _logger = logger;

            _issueId = 3761;
            _issueBranch = "stable-3.17";
            _branchesForPorting = new List<string>() { "master" };
        }

        public async Task RunAsync()
        {
            try
            {
                // TODO: Внимание! В список related merge request для некоторого issue могут попасть merge request'ы другого issue (см. текущую задачу, к ней прикреплены MR других задач). Такие merge request следует пропускать при проверке.
                _logger.LogInformation($"PrerequisiteCheck starts for issue {_issueId}");

                _mergeRequests = await _store.GetRelatedMergeRequestsAsync(_issueId);

                Validate();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "some error ocurred:");
            }
            finally
            {
                _logger.LogInformation("PrerequisiteCheck finishes");
            }
            Console.ReadLine();
        }

        private void Validate() 
        {
            var mainMergeRequests = _mergeRequests.Where(mr => mr.TargetBranch.Equals(_issueBranch) && mr.State == MergeState.Merged);
            foreach (var mainMR in mainMergeRequests) 
            {
                ValidateMR(mainMR);
            }
        }

        private void ValidateMR(MergeRequest mainMR) 
        {
            if (mainMR.TargetBranch.Equals("master"))
                return;

            if (mainMR.Labels.Contains("skip porting"))
                return;

            foreach (var targetBranch in _branchesForPorting) 
            {
                ValidateMRPorting(mainMR, targetBranch);
            }
        }

        private void ValidateMRPorting(MergeRequest mainMR, string targetBranch)
        {
            if (ShouldSkip(mainMR, targetBranch))
                return;

            string originalMR = $"Original MR: !{mainMR.Id}";
            var portingMergeRequests = _mergeRequests.Where(mr => mr.TargetBranch.Equals(targetBranch) 
                                                            && mr.State == MergeState.Merged
                                                            && mr.Description.Contains(originalMR));

            if (!portingMergeRequests.Any())
            {
                _logger.LogError($"{mainMR.Author}, не были портированы изменения по задаче {_issueId} в ветку {targetBranch}. Merge request, который необходимо портировать: {mainMR.Reference}");
            }
            else if (portingMergeRequests.Count() > 1) 
            {
                _logger.LogWarning($"Найдено более 1го портирования Merge request {mainMR.Reference}. Возможно присутствует ошибка в логике поиска MR");
            }
        }

        private bool ShouldSkip(MergeRequest mainMR, string targetBranch) 
        {
            int startIndex = mainMR.Description.IndexOf("### Skip porting");

            if (startIndex < 0)
            {
                return false;    
            }

            int endIndex = mainMR.Description.IndexOf("# Review summary");

            string skipPorting = mainMR.Description.Substring(startIndex, endIndex - startIndex - 1);

            if (skipPorting.Contains("(e.g. \"stable-3.15, stable-3.16\")")) 
            {
                skipPorting = skipPorting.Replace("(e.g. \"stable-3.15, stable-3.16\")", "");
            }

            if (skipPorting.Contains(targetBranch)) 
            {
                _logger.LogInformation($" Porting MR {mainMR.Reference} to branch {targetBranch} was skipped.");
                return true;
            }

            return false;
        }
    }
}
