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
            CreateTime = DateTime.Now;
        }

        public Guid Id { get; set; }

        public string TagName { get; set; }

        public DateTime CreateTime { get; set; }

        public virtual IList<TagsInPosts> TagsInPosts { get; set; }
    }
}
