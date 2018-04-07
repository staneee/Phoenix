using Microsoft.AspNetCore.Identity;
using OneBlog.Data;
using OneBlog.Data.Contracts;
using OneBlog.Helpers;
using OneBlog.Services;
using Qiniu.IO;
using SS.MetaWeblog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace OneBlog.MetaWeblog
{
    public class WeblogProvider : IMetaWeblogProvider
    {
        private IPostsRepository _repo;
        private UserManager<AppUser> _userMgr;
        private QiniuService _qiniuService;

        public WeblogProvider(UserManager<AppUser> userMgr, IPostsRepository repo, QiniuService qiniuService)
        {
            _repo = repo;
            _userMgr = userMgr;
            _qiniuService = qiniuService;
        }


        public string AddPost(string blogid, string username, string password, MetaPost post, bool publish)
        {
            EnsureUser(username, password).Wait();

            var newStory = new Data.Post();
            try
            {
                newStory.Title = post.title;
                newStory.Content = post.description;
                newStory.DatePublished = post.dateCreated == DateTime.MinValue ? DateTime.UtcNow : post.dateCreated;
                newStory.Tags = string.Join(",", post.categories);
                newStory.IsPublished = publish;
                newStory.Slug = newStory.GetStoryUrl();

                _repo.AddPost(newStory);
                _repo.SaveAll();
            }
            catch (Exception)
            {
                throw new MetaWeblogException("Failed to save the post.");
            }
            return newStory.Id.ToString();

        }

        public bool EditPost(string postid, string username, string password, MetaPost post, bool publish)
        {
            EnsureUser(username, password).Wait();

            try
            {
                var story = _repo.GetPost(postid);

                story.Title = post.title;
                story.Content = post.description;
                story.DatePublished = post.dateCreated == DateTime.MinValue ? DateTime.UtcNow : post.dateCreated;
                story.Tags = string.Join(",", post.categories);
                story.IsPublished = publish;
                story.Slug = story.GetStoryUrl();

                _repo.SaveAll();

                return true;
            }
            catch (Exception)
            {
                throw new MetaWeblogException("Failed to save the post.");
            }
        }



        private string GetFileName(string ext = "")
        {
            var random = new Random(Guid.NewGuid().GetHashCode());

            return string.Format("{0}{1}{2}", DateTime.Now.ToString("yyyy/MM/dd/HHmmss"), random.Next(100000, 1000000), ext);
        }

        public MetaMediaObjectInfo NewMediaObject(string blogid, string username, string password, MetaMediaObject mediaObject)
        {
            EnsureUser(username, password).Wait();

            var filenameonly = mediaObject.name.Substring(mediaObject.name.LastIndexOf('/') + 1);
            var ext = Path.GetExtension(filenameonly);

            var target = new IOClient();

            var key = GetFileName(ext);
            var objectInfo = new MetaMediaObjectInfo();

            var bits = Convert.FromBase64String(mediaObject.bits);

            objectInfo.url = _qiniuService.Upload(key, bits).Result;
            return objectInfo;
        }

        public MetaCategoryInfo[] GetCategories(string blogid, string username, string password)
        {
            EnsureUser(username, password).Wait();

            return _repo.GetCategories()
              .Select(c => new MetaCategoryInfo()
              {
                  categoryid = c,
                  title = c,
                  description = c,
                  htmlUrl = string.Concat("/tags/", c),
                  rssUrl = ""
              }).ToArray();

        }

        public bool DeletePost(string key, string postid, string username, string password, bool publish)
        {
            EnsureUser(username, password).Wait();

            try
            {
                var result = _repo.DeletePost(postid);
                _repo.SaveAll();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }



        private async Task<AppUser> EnsureUser(string username, string password)
        {
            var user = await _userMgr.FindByNameAsync(username);
            if (user != null)
            {
                if (await _userMgr.CheckPasswordAsync(user, password))
                {
                    return user;
                }
            }
            throw new MetaWeblogException("Authentication failed.");
        }

        private void EnsureDirectory(DirectoryInfo dir)
        {
            if (dir.Parent != null)
            {
                EnsureDirectory(dir.Parent);
            }

            if (!dir.Exists)
            {
                dir.Create();
            }
        }

        public int AddCategory(string key, string username, string password, MetaNewCategory category)
        {
            EnsureUser(username, password).Wait();

            // We don't store these, just query them from the list of stories so don't do anything
            return 1;
        }

        public async Task<MetaUserInfo> GetUserInfoAsync(string key, string username, string password)
        {
            var user = await EnsureUser(username, password);
            return new MetaUserInfo()
            {
                email = user.Email,
                nickname = user.DisplayName,
                userid = user.Id,
                url = "/"
            };
        }

        public async Task<IList<MetaBlogInfo>> GetUsersBlogsAsync(string key, string username, string password)
        {
            var user = await EnsureUser(username, password);
            var blog = new MetaBlogInfo()
            {
                blogid = user.Id,
                blogName = user.DisplayName,
                url = "/"
            };

            return new MetaBlogInfo[] { blog };
        }

        public async Task<MetaPost> GetPostAsync(string postid, string username, string password)
        {
            var user = await EnsureUser(username, password);
            try
            {
                var story = _repo.GetPost(postid);
                var newPost = new MetaPost()
                {
                    title = story.Title,
                    description = story.Content,
                    dateCreated = story.DatePublished,
                    categories = story.Tags.Split(','),
                    postid = story.Id,
                    userid = "test",
                    wp_slug = story.GetStoryUrl()
                };

                return newPost;
            }
            catch (Exception)
            {
                throw new MetaWeblogException("Failed to get the post.");
            }
        }

        public async Task<IList<MetaPost>> GetRecentPostsAsync(string blogid, string username, string password, int numberOfPosts)
        {
            var user = await EnsureUser(username, password);

            return _repo.GetPosts(numberOfPosts).Posts.Select(s => new MetaPost()
            {
                title = s.Title,
                description = s.Content,
                categories = s.Tags.Select(m => m.TagName).ToArray(),
                dateCreated = s.DatePublished,
                postid = s.Id,
                permalink = s.Slug,
                wp_slug = s.Slug
            }).ToArray();
        }
    }
}