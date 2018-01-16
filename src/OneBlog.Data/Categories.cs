using System;
using System.Collections.Generic;
using OneBlog.Helpers;
using SS.Toolkit.Helpers;

namespace OneBlog.Data
{
    public class Categories
    {

        public Categories()
        {
            Id = GuidHelper.Gen();
        }

        public Guid Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public Guid ParentId { get; set; }

        public virtual IList<PostsInCategories> PostsInCategories { get; set; }
    }
}
