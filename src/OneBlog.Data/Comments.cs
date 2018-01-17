using SS.Toolkit.Helpers;
using System;

namespace OneBlog.Data
{
    /// <summary>
    /// 评论
    /// </summary>
    public class Comments
    {
        public Comments()
        {
            Id = GuidHelper.Gen();
        }

        public Guid Id { get; set; }

        /// <summary>
        /// 父Id
        /// </summary>
        public Guid ParentId { get; set; }

        /// <summary>
        /// 内容
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// Ip地址
        /// </summary>
        public string Ip { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the Comment is approved.
        /// </summary>
        public bool IsApproved { get; set; }

        /// <summary>
        /// 是否是作弊
        /// </summary>
        public bool IsSpam { get; set; }

        /// <summary>
        /// 评论时间
        /// </summary>
        public DateTime CreateDate { get; set; }

        public virtual Posts Posts { get; set; }

        public virtual ApplicationUser Author { get; set; }



    }
}
