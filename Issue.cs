using System;
using System.Collections.Generic;
using System.Text;

namespace GitlabStats
{
    public class Issue
    {
        public int Id { get; private set; }

        public string Title { get; private set; }

        public string ShortTitle { get; private set; }

        public string State { get; private set; }

        public string Milestone { get; private set; }

        public string Estimation { get; private set; }

        public Issue(int id, string title) : this (id, title, string.Empty)
        {
        }

        public Issue(int id, string title, string estimation) 
        {
            Id = id;
            Title = CleanTitle(title);
            ShortTitle = CreateShortTitle();
            Estimation = estimation;
        }

        public Issue(int id, string title, string estimation, string milestone, string state)
        {
            Id = id;
            Title = CleanTitle(title);
            ShortTitle = CreateShortTitle();
            Estimation = estimation;
            Milestone = milestone ?? "<No milestone>";
            State = state;
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

        private string CreateShortTitle() 
        {
            //if (this.Title.Length <= 20)
            return this.Title;

            ////return Title.Replace(/[\s\S]{ 1,20} (? !\S)/ g, '$&\n')


            ////StringBuilder sb = new StringBuilder();

            ////for (int counter = 30; counter < Title.Length; counter += 30) 
            ////{
            ////    if (Title.Substring(counter).Length > 15)
            ////    {
            ////        Title.IndexOf("")
            ////        sb.Append(Title.Substring(counter - 30, 30));
            ////        sb.Append("\n");
            ////    }
            ////    else 
            ////    {
            ////        return sb.ToString();
            ////    }
            ////}
        }
    }
}
