using System;
using System.Collections.Generic;
using System.Text;

namespace GitlabStats
{
    interface IOptions
    {
        public string GitlabUrl { get; set; }

        public string GitlabAccessToken { get; set; }

        public string JiraUrl { get; set; }

        public string JiraBasicAuth { get; set; }
    }
}
