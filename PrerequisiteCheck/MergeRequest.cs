using System;
using System.Collections.Generic;
using System.Text;

namespace GitlabStats.PrerequisiteCheck
{
    class MergeRequest
    {
        public string Author { get; set; }

        public string Title { get;  set; }

        public string Description { get; set; }

        public int Id { get; set; }

        public IEnumerable<string> Labels { get; set; }

        public string Reference { get; set; }

        public string SourceBranch { get; set; }

        public string TargetBranch { get; set; }

        public MergeState State { get; set; }

    }

    public enum MergeState 
    {
        Closed,
        Merged,
        Open
    }
}
