using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Phoenix.Configuration;
using Phoenix.Data;
using Phoenix.Data.Contracts;
using Phoenix.Data.Models;
using System;

namespace Phoenix.Controllers
{

    [Route("category")]
    public class CategoryController : Controller
    {
        private IPostsRepository _postsRepository;
        private IMemoryCache _memoryCache;
        private IOptions<AppSettings> _appSettings;
        public CategoryController(IPostsRepository postsRepository, IOptions<AppSettings> appSettings, IMemoryCache memoryCache)
        {
            _postsRepository = postsRepository;
            _memoryCache = memoryCache;
            _appSettings = appSettings;
        }

        [HttpGet("{id}")]
        public IActionResult Index(string id)
        {
            return Pager(id, 1);
        }

        [HttpGet("{id}/{page}")]
        public IActionResult Pager(string id, int page)
        {

            var cacheKey = $"Categor_Index_{id.ToString()}_{page}";
            string cached;
            PostsResult result = null;
            if (!_memoryCache.TryGetValue(cacheKey, out cached))
            {
                result = _postsRepository.GetPostsByCategory(id, _appSettings.Value.PostPerPage, page);
                if (result != null)
                {
                    cached = JsonConvert.SerializeObject(result);
                    _memoryCache.Set(cacheKey, cached, new MemoryCacheEntryOptions() { SlidingExpiration = TimeSpan.FromMinutes(5) });
                }
            }
            else
            {
                try
                {
                    result = JsonConvert.DeserializeObject<PostsResult>(cached);
                }
                catch
                {
                    result = _postsRepository.GetPostsByCategory(id, _appSettings.Value.PostPerPage, page);
                }
            }
            ViewBag.ControllerName = "category";
            ViewBag.Id = id.ToString();
            ViewBag.Title = $"{result.Category}";
            ViewBag.Category = result.Category;
            return View("Index", result);
        }
    }
}