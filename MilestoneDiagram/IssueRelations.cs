using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace GitlabStats.MilestoneDiagram
{
    internal class IssueRelations : IEnumerable<Relation>
    {
        private readonly Dictionary<RelationKey, Relation> _relations;

        internal IssueRelations() 
        {
            _relations = new Dictionary<RelationKey, Relation>();
        }

        public bool AddRelation(Issue mainIssue, Issue blockedByIssue) 
        {
            var key = new RelationKey(mainIssue.Id, blockedByIssue.Id);

            if (!_relations.ContainsKey(key)) 
            {
                _relations.Add(key, new Relation(mainIssue, blockedByIssue));
                return true;
            }

            return false;
        }

        public bool HasIssue(Issue issue) 
        {
            foreach (var relationKey in _relations.Keys) 
            {
                if (relationKey.IssueId == issue.Id || relationKey.BlockeByIssueId == issue.Id)
                    return true;
            }

            return false;
        }

        public IEnumerator<Relation> GetEnumerator()
        {
            return _relations.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _relations.Values.GetEnumerator();
        }
    }

    class RelationKey 
    {
        internal int IssueId { get; private set; }
        internal int BlockeByIssueId { get; private set; }

        internal RelationKey(int issueId, int blockedByIssueId) 
        {
            IssueId = issueId;
            BlockeByIssueId = blockedByIssueId;
        }

        public override string ToString()
        {
            return $"{BlockeByIssueId}_{IssueId}";
        }

        public override bool Equals(Object obj)
        {
            //Check for null and compare run-time types.
            if ((obj == null) || !this.GetType().Equals(obj.GetType()))
            {
                return false;
            }
            else
            {
                RelationKey key = (RelationKey)obj;
                return IssueId == key.IssueId && BlockeByIssueId == key.BlockeByIssueId;
            }
        }

        public override int GetHashCode()
        {
            return ShiftAndWrap(IssueId.GetHashCode(), 2) ^ BlockeByIssueId.GetHashCode();
        }


        private int ShiftAndWrap(int value, int positions)
        {
            positions = positions & 0x1F;

            // Save the existing bit pattern, but interpret it as an unsigned integer.
            uint number = BitConverter.ToUInt32(BitConverter.GetBytes(value), 0);
            // Preserve the bits to be discarded.
            uint wrapped = number >> (32 - positions);
            // Shift and wrap the discarded bits.
            return BitConverter.ToInt32(BitConverter.GetBytes((number << positions) | wrapped), 0);
        }
    }

    class Relation 
    {
        internal Issue MainIssue { get; private set; }

        internal Issue BlockedByIssue { get; private set; }

        internal Relation(Issue mainIssue, Issue blockedByIssue) 
        {
            MainIssue = mainIssue;
            BlockedByIssue = blockedByIssue;
        }

        public override string ToString()
        {
            return $"{BlockedByIssue} -> {MainIssue}";
        }
    }


}
