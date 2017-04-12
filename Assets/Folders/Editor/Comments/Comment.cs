using System;

namespace BeardPhantom.Folders
{
    [Serializable]
    public class Comment : IComparable<Comment>
    {
        public string user;
        public string commentText;
        public long date;

        public int CompareTo(Comment other)
        {
            return date.CompareTo(other.date);
        }
    }
}