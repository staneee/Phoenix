﻿using Microsoft.AspNetCore.Mvc;
using Phoenix.Data.Contracts;
using Phoenix.Data.Models;
using Phoenix.Models.CommentViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Phoenix.ViewComponents
{
    public class CommentViewComponent : ViewComponent
    {
        private readonly IPostsRepository _postRepository;
        private readonly ICommentsRepository _commentsRepository;

        public CommentViewComponent(IPostsRepository postRepository, ICommentsRepository commentsRepository)
        {
            _postRepository = postRepository;
            _commentsRepository = commentsRepository;
        }


        public IViewComponentResult Invoke(string id)
        {
            var model = new CommentViewModel();
            model.PostId = id;
            model.Comments = _commentsRepository.FindByPostId(id);
            return View(model);
        }
    }
}
