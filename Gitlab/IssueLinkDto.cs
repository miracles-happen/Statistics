using GitlabStats.MilestoneDiagram;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Text;

namespace GitlabStats.Gitlab
{
    public class IssueLinkDto
    {
        [JsonProperty("iid")]
        public int Id { get; set; }

        public string Title { get; set; }

        [JsonProperty("link_type")]
        [JsonConverter(typeof(StringEnumConverter))]
        public LinkTypes LinkType { get; set; }

        [JsonProperty("time_stats")]
        public TimeStatsDto TimeStats { get; set; }

        internal IssueLink MapToIssueLink()
        {
            var issueLink = new IssueLink()
            {
                RelatedIssue = new Issue(Id, Title, TimeStats.HumanEstimate)
            };

            switch (LinkType) 
            {
                case LinkTypes.Relates_To:
                    issueLink.LinkType = MilestoneDiagram.LinkTypes.RelatesTo;
                    break;
                case LinkTypes.Blocks:
                    issueLink.LinkType = MilestoneDiagram.LinkTypes.Blocks;
                    break;
                case LinkTypes.Is_blocked_by:
                    issueLink.LinkType = MilestoneDiagram.LinkTypes.IsBlockedBy;
                    break;
                default:
                    throw new ArgumentException($"Unknown issue link type: {LinkType}");
            }

            return issueLink;
        }
    }

    public enum LinkTypes
    {
        [JsonProperty("relates_to")]
        Relates_To, 
        
        Blocks, 
        
        Is_blocked_by
    }
}
