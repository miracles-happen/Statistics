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
            AccessToken = configurationRoot.GetValue<string>("Gitlab:AccessToken");
        }


        public string GitlabUrl { get; set; }

        public string AccessToken { get; set; }
    }
}
