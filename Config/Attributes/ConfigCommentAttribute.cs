using System;

namespace SALT.Config.Attributes
{
    public class ConfigCommentAttribute : Attribute
    {
        internal string Comment;

        public ConfigCommentAttribute(string comment) => this.Comment = comment;
    }
}
