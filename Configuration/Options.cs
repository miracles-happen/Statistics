using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace GitlabStats
{
    class Options : IOptions
    {
        public Options(IConfigurationRoot configurationRoot)
        {
            GitlabUrl = configurationRoot.GetValue<string>("Gitlab:Url");
            GitlabAccessToken = configurationRoot.GetValue<string>("Gitlab:AccessToken");
            JiraUrl = configurationRoot.GetValue<string>("Jira:Url");
            JiraBasicAuth = configurationRoot.GetValue<string>("Jira:BasicAuth");
        }

        public string GitlabUrl { get; set; }

        public string GitlabAccessToken { get; set; }

        public string JiraUrl { get; set; }

        public string JiraBasicAuth { get; set; }
    }
}