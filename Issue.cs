using System;
using System.Collections.Generic;
using System.Text;

namespace GitlabStats
{
    public class Issue
    {
        public int Id { get; private set; }

        public string Title { get; private set; }

        public string Estimation { get; private set; }

        public Issue(int id, string title) : this (id, title, string.Empty)
        {
        }

        public Issue(int id, string title, string estimation) 
        {
            Id = id;
            Title = CleanTitle(title);
            Estimation = estimation;
        }

        public override string ToString() 
        {
            if(!string.IsNullOrEmpty(Estimation))
                return $"\"{Id} {Title}\\n({Estimation})\"";
            else
                return $"\"{Id} {Title}\"";
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
                Issue issue = (Issue)obj;
                return Id == issue.Id;
            }
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        private string CleanTitle(string title) 
        {
            int startIndex = title.IndexOf("[");
            int endIndex = title.IndexOf("]");

            if(endIndex > startIndex && startIndex >= 0) 
            {
                title = title.Substring(endIndex + 2);
            }

            return title.Replace("\"", "\'");
        }
    }
}
