using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using GitlabStats.GitlabApi;
using GitlabStats.Report;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace GitlabStats
{
    class Program
    {
        // TODO access token remove
        static readonly DateTime _since = new DateTime(2021, 01, 20);  // TODO: move to console args

        static void Main()
        {
            CancellationTokenSource cts = new CancellationTokenSource();

            Console.CancelKeyPress += (s, e) =>
            {
                e.Cancel = true;
                cts.Cancel();
            };

            try
            {
                var task = MainAsync();
                task.Wait(cts.Token);
            }
            catch (OperationCanceledException operationCanceledException)
            {
                Console.WriteLine("OperationCanceledException:" + operationCanceledException.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR:" + ex);
            }
        }

        private static async Task MainAsync()
        {

            var services = ConfigureServices();
            var serviceProvider = services.BuildServiceProvider();

            var statisticBuilder = serviceProvider.GetService<IStatisticBuilder>();
            await statisticBuilder.RunAsync(_since);
        }

        public static IServiceCollection ConfigureServices()
        {
            var configuration = GetConfiguration();

            var options = new Options(configuration);

            var services = new ServiceCollection()
                .AddSingleton<IOptions, Options>(provider => options)
                .AddLogging(logging =>
                {
                    logging.AddConsole();
                    logging.SetMinimumLevel(LogLevel.Trace);
                })
                .AddScoped<IStatisticBuilder, StatisticBuilder>()
                .AddScoped<IIssueStore, GitLabClient>()
                //.AddScoped<IReportFormatter, ConsoleFormatter>()
                .AddScoped<IReportFormatter, CsvFormatter>()
                .AddScoped<IReportBuilder, ReportBuilder>();

            return services;
        }

        private static IConfigurationRoot GetConfiguration()
        {
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true);

            return builder.Build();
        }
    }
}
