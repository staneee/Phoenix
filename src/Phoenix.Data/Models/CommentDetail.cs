namespace Phoenix.Data.Models
{
    /// <summary>
    /// Blog comments
    /// </summary>
    public class CommentDetail
    {
        /// <summary>
        ///Gets or sets the Comment Id
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        ///Parent comment Id
        /// </summary>
        public string ParentId { get; set; }
        /// <summary>
        ///Comment post ID
        /// </summary>
        public string PostId { get; set; }
        /// <summary>
        /// Comment title
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// Gets or sets the author's website
        /// </summary>
        public string Website { get; set; }
        /// <summary>
        /// User Name
        /// </summary>
        public string DislpayName { get; set; }

        public string Email { get; set; }
        /// <summary>
        ///     Gets or sets the ip
        /// </summary>
        public string Ip { get; set; }
        /// <summary>
        ///Comment content
        /// </summary>
        public string Content { get; set; }
    }
}
