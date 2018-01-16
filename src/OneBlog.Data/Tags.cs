using System;
using OneBlog.Helpers;
using System.Collections.Generic;
using SS.Toolkit.Helpers;

namespace OneBlog.Data
{
    public class Tags
    {
        public Tags()
        {
            Id = GuidHelper.Gen();
        }

        public Guid Id { get; set; }

        public string TagName { get; set; }

        public virtual IList<TagsInPosts> TagsInPosts { get; set; }
    }
}
