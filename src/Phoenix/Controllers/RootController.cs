using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.NodeServices;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Phoenix.Configuration;
using Phoenix.Data;
using Phoenix.Data.Contracts;
using Phoenix.Data.Models;
using Phoenix.Helpers;
using Phoenix.Models;
using Phoenix.Models.CommentViewModels;
using Phoenix.RssSyndication;
using Phoenix.Services;
using System;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Phoenix.Controllers
{
    [Route("")]
    public class RootController : Controller
    {

        private IMailService _mailService;
        private IPostsRepository _postsRepository;
        private IMemoryCache _memoryCache;
        private ILogger<RootController> _logger;
        private ICommentsRepository _commentsRepository;
        private IViewRenderService _viewRenderService;
        private readonly UserManager<AppUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IOptions<AppSettings> _appSettings;
        private INodeServices _nodeService;
        private ISession _session => _httpContextAccessor.HttpContext.Session;

        public RootController(IMailService mailService, UserManager<AppUser> userManager,
                              IPostsRepository repo, ICommentsRepository commentsRepository,
                              IHttpContextAccessor httpContextAccessor,
                              IMemoryCache memoryCache,
                              IViewRenderService viewRenderService,
                              IOptions<AppSettings> appSettings,
                              INodeServices nodeService,
                              ILogger<RootController> logger)
        {
            _nodeService = nodeService;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
            _viewRenderService = viewRenderService;
            _mailService = mailService;
            _postsRepository = repo;
            _commentsRepository = commentsRepository;
            _memoryCache = memoryCache;
            _appSettings = appSettings;
            _logger = logger;

        }


        [ResponseCache(VaryByHeader = "Accept-Encoding", Location = ResponseCacheLocation.Any, Duration = 10)]
        [HttpGet("")]
        [HttpPost("")]
        public IActionResult Index()
        {
            return Pager(1);
        }

        [HttpGet("{page:int?}")]
        public IActionResult Pager(int page)
        {
            ViewBag.ControllerName = "Root";
            var cacheKey = $"Root_Pager_{page}";
            string cached;
            PostsResult result = null;
            //if (_memoryCache.TryGetValue(cacheKey, out cached))
            //{
            //    result = JsonConvert.DeserializeObject<PostsResult>(cached);
            //}
            if (result == null)
            {
                result = _postsRepository.GetPosts(_appSettings.Value.PostPerPage, page);
                if (result != null)
                {
                    cached = JsonConvert.SerializeObject(result);
                    _memoryCache.Set(cacheKey, cached, new MemoryCacheEntryOptions() { SlidingExpiration = TimeSpan.FromMinutes(5) });
                }
            }
            return View("_List", result);
        }

        [HttpGet("captcha")]
        public ActionResult Captcha()
        {
            string code = ValidateHelper.CreateValidateCode(5);
            _session.SetString("ValidateCode", code);
            byte[] bytes = ValidateHelper.CreateValidateGraphic(code);
            return File(bytes, @"image/jpeg");
        }

        /// <summary>
        /// 统计接口
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost("postcount/{id?}")]
        public IActionResult PostCount(string id)
        {
            try
            {
                var count = _postsRepository.AddPostCount(id);
                return Json(new { Count = count });
            }
            catch
            {
                return Json(new { Count = 0 });
            }
        }



        /// <summary>
        /// 文章详情页面
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("post/{slug}")]
        public async Task<IActionResult> Post(string slug)
        {
            try
            {
                PostDetail post = null;
                if (Guid.TryParse(slug, out Guid id))
                {
                    post = _postsRepository.FindById(id);
                }
                else
                {
                    post = _postsRepository.GetPostBySlug(slug);
                }
                if (post != null)
                {
                    post.Content = _postsRepository.FixContent(post.Content);
                    try
                    {
                        ///转化Markdown
                        post.Content = await _nodeService.InvokeAsync<string>("./Node/Parser", post.Content);
                    }
                    catch (Exception ex)
                    {

                    }
                }
                return View(post);
            }
            catch
            {
                _logger.LogWarning($"Couldn't find the ${slug} post");
            }
            return Redirect("/");
        }


        /// <summary>
        /// 关于页面
        /// </summary>
        /// <returns></returns>
        [HttpGet("about")]
        public IActionResult About()
        {
            return View();
        }

        /// <summary>
        /// 联系我们页面
        /// </summary>
        /// <returns></returns>
        [HttpGet("contact")]
        public IActionResult Contact()
        {
            return View();
        }

        [HttpPost("contact")]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Contact([FromBody]ContactModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var spamState = VerifyNoSpam(model);
                    if (!spamState.Success)
                    {
                        return BadRequest(new { Reason = spamState.Reason });
                    }

                    await _mailService.SendMail("ContactTemplate.txt", model.Name, model.Email, model.Subject, model.Msg);

                    return Ok(new { Success = true, Message = "Message Sent" });
                }
                else
                {
                    return BadRequest(new { Reason = "Failed to send email..." });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to send email from contact page", ex);
                return BadRequest(new { Reason = "Error Occurred" });
            }

        }

        // Brute Force getting rid of my worst emails
        private SpamState VerifyNoSpam(ContactModel model)
        {
            var tests = new string[]
            {
        "improve your seo",
        "improved seo",
        "generate leads",
        "viagra",
        "your team",
        "PHP Developers",
        "working remotely",
        "google search results"
            };

            if (tests.Any(t =>
            {
                return new Regex(t, RegexOptions.IgnoreCase).Match(model.Msg).Success;
            }))
            {
                return new SpamState() { Reason = "Spam Email Detected. Sorry." };
            }
            return new SpamState() { Success = true };
        }


        [HttpGet("Error/{code:int}")]
        public IActionResult Error(int errorCode)
        {
            if (Response.StatusCode == (int)HttpStatusCode.NotFound ||
                errorCode == (int)HttpStatusCode.NotFound ||
                Request.Path.Value.EndsWith("404"))
            {
                return View("NotFound");
            }

            return View();
        }

        [HttpGet("Exception")]
        public IActionResult Exception()
        {
            var exception = HttpContext.Features.Get<IExceptionHandlerFeature>();
            var request = HttpContext.Features.Get<IHttpRequestFeature>();

            if (exception != null && request != null)
            {
                var message = $@"RequestUrl: ${request.Path} Exception: ${exception.Error}";

                ViewBag.Error = message;
                //_mailService.SendMail("logmessage.txt", "Shawn Wildermuth", "shawn@wildermuth.com", "[One Exception]", message);
            }
            return View();
        }

        [HttpGet("feed")]
        public IActionResult Feed()
        {
            var feed = new RssFeed()
            {
                Title = _appSettings.Value.Title,
                Description = _appSettings.Value.Description,
                Link = new Uri("http://www.huafenfei.com/feed"),
                Copyright = "© 2016-2017 huafenfei.com"
            };

            var entries = _postsRepository.GetPosts(_appSettings.Value.PostPerPage);

            foreach (var entry in entries.Posts)
            {
                var item = new RssItem()
                {
                    Title = entry.Title,
                    Body = string.Concat(entry.Content),
                    Link = new Uri(new Uri(Request.GetEncodedUrl()), "post/" + entry.Id.ToString()),
                    Permalink = entry.Slug,
                    PublishDate = entry.DatePublished,
                    Author = new RssAuthor() { Name = entry.Author.Name, Email = entry.Author.Email }
                };

                foreach (var cat in entry.Tags)
                {
                    item.Categories.Add(cat.TagName);
                }
                feed.Items.Add(item);
            }

            return File(Encoding.UTF8.GetBytes(feed.Serialize()), "text/xml");

        }

        [HttpGet("calendar")]
        public IActionResult Calendar()
        {
            return View();
        }


        /// <summary>
        /// 评论
        /// </summary>
        /// <param name="model"></param>
        /// <param name="collection"></param>
        /// <returns></returns>
        [HttpPost("comment")]
        public async Task<IActionResult> Comment(CommentViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new
                {
                    Error = "提交的信息有误,请检查后再试"
                });
            }
            var validateCode = _session.GetString("ValidateCode");
            if (string.IsNullOrEmpty(validateCode))
            {
                return Json(new
                {
                    Error = "验证码过期，请刷新重试！",
                });
            }
            _session.Remove("ValidateCode");
            if (!string.Equals(validateCode, model.Captcha, StringComparison.OrdinalIgnoreCase))
            {
                return Json(new
                {
                    Error = "提交的验证码错误！",
                });
            }
            var replyToCommentId = Request.Form["hiddenReplyTo"].ToString();
            var post = _postsRepository.GetPost(model.PostId);
            var commentDetail = new CommentDetail() { PostId = model.PostId, Author = await GetCurrentUserAsync(), Content = model.Content };
            if (!string.IsNullOrEmpty(replyToCommentId))
            {
                commentDetail.ParentId = replyToCommentId;
            }
            var comment = _commentsRepository.Add(commentDetail);
            var result = await _viewRenderService.RenderToStringAsync(this, "_Comment", comment);
            return Json(new
            {
                Error = "",
                CommentId = comment.Id,
                CommentCount = (post.Comments.Count + 1),
                Result = result,
                Content = model.Content
            });
        }

        private Task<AppUser> GetCurrentUserAsync()
        {
            return _userManager.GetUserAsync(HttpContext.User);
        }

    }
}
