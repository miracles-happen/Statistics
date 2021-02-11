using System;
using System.Collections.Generic;
using System.Text;

namespace GitlabStats
{
    interface IOptions
    {
        public string GitlabUrl { get; set; }

        public string AccessToken { get; set; }
    }
}
