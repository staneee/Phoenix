﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Phoenix.Data;
using Phoenix.Data.Contracts;
using Phoenix.Data.Models;

namespace Phoenix.Controllers
{
    [Route("[controller]")]
    public class SearchController : Controller
    {
        private IPostsRepository _repo;

        public SearchController(IPostsRepository repo)
        {
            _repo = repo;
        }

        [HttpGet("")]
        public IActionResult Index()
        {
            ViewBag.Term = "";
            return View(new PostsResult());
        }

        [HttpGet("{term}/{page:int?}")]
        public IActionResult Pager(string term, int page = 1)
        {
            ViewBag.ControllerName = "search";
            ViewBag.Term = term;
            var results = _repo.GetPostsByTerm(term, 10, page);
            return View("Index", results);
        }
    }
}
