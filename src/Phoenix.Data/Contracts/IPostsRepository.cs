using Phoenix.Data.Models;
using System;
using System.Collections.Generic;

namespace Phoenix.Data.Contracts
{
    public interface IPostsRepository
    {
        Pager<PostItem> Find(int take = 10, int skip = 0);
        /// <summary>
        /// 获取Post
        /// </summary>
        /// <param name="pageSize"></param>
        /// <param name="page"></param>
        /// <param name="authorId"></param>
        /// <returns></returns>
        PostsResult GetPosts(int pageSize = 10, int page = 1, string authorId = null);
        PostsResult GetPostsByTerm(string term, int pageSize, int page);
        PostsResult GetPostsByTag(string tag, int pageSize, int page);

        PostsResult GetPostsByCategory(string categoryId, int pageSize, int page);

        Post GetPost(string id);

        long AddPostCount(string id);

        bool CheckIsOnly(string title, string authorId);

        PostDetail FindById(Guid id);

        PostDetail GetPostBySlug(string slug);

        PostDetail Add(PostDetail post);

        PostDetail Update(PostDetail post);

        void AddPost(Post story);

        void SaveAll();
        bool DeletePost(string postid);

        IEnumerable<string> GetCategories();

        string FixContent(string content);
    }
}