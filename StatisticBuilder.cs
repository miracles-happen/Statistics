using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using GitlabStats.Report;
using Microsoft.Extensions.Logging;

namespace GitlabStats
{
    interface IStatisticBuilder 
    {
        public Task RunAsync(DateTime since);
    }
    class StatisticBuilder : IStatisticBuilder
    {
        private readonly IIssueStore _store;
        private readonly ILogger<StatisticBuilder> _logger;
        private readonly IReportBuilder _reportBuilder;

        public StatisticBuilder(IIssueStore store, IReportBuilder reportBuilder, ILogger<StatisticBuilder> logger) 
        {
            _store = store;
            _logger = logger;
            _reportBuilder = reportBuilder;
        }

        public async Task RunAsync(DateTime since) 
        {
            try
            {
                _logger.LogInformation($"Builder starts since date {since}");

                var issues = await _store.FindTasksAsync(since);
                _reportBuilder.Build(issues, since);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "some error ocurred:");
            }
            finally 
            {
                _logger.LogInformation("Builder finishes");

            }
        }
    }
}
