using System;
using System.Collections.Generic;
using System.Text;

namespace GitlabStats.MilestoneDiagram
{
    class IssueLink
    {
        public Issue RelatedIssue { get; set; }

        public LinkTypes LinkType { get; set; }
    }

    public enum LinkTypes
    {
        RelatesTo,
        Blocks,
        IsBlockedBy
    }
}
