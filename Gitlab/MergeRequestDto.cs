using System;
using System.Collections.Generic;
using System.Text;
using GitlabStats.PrerequisiteCheck;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace GitlabStats.Gitlab
{
    public class MergeRequestDto
    {
        public AuthorDto Author { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        [JsonProperty("iid")]
        public int Id { get; set; }

        public IEnumerable<string> Labels { get; set; }

        public string Reference { get; set; }

        [JsonProperty("source_branch")]
        public string SourceBranch { get; set; }

        [JsonProperty("target_branch")]
        public string TargetBranch { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public MergeState State { get; set; }

        internal MergeRequest MapToMergeRequest()
        {
            var mergeRequest = new MergeRequest()
            {
                Id = Id,
                Title = Title,
                Description = Description,
                Author = Author.UserName,
                Labels = Labels,
                Reference = Reference,
                SourceBranch = SourceBranch,
                TargetBranch = TargetBranch,
                State = State
            };

            //switch (LinkType)
            //{
            //    case LinkTypes.Relates_To:
            //        issueLink.LinkType = MilestoneDiagram.LinkTypes.RelatesTo;
            //        break;
            //    case LinkTypes.Blocks:
            //        issueLink.LinkType = MilestoneDiagram.LinkTypes.Blocks;
            //        break;
            //    case LinkTypes.Is_blocked_by:
            //        issueLink.LinkType = MilestoneDiagram.LinkTypes.IsBlockedBy;
            //        break;
            //    default:
            //        throw new ArgumentException($"Unknown issue link type: {LinkType}");
            //}

            return mergeRequest;
        }

    }

    public enum MergeStateDto
    {
        Closed,
        Merged,
        Open
    }

    public class AuthorDto
    {
        public string UserName { get; set; }
    }
}
