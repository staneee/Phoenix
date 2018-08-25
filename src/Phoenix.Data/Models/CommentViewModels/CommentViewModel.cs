﻿using Phoenix.Data.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Phoenix.Models.CommentViewModels
{
    public class CommentViewModel
    {

        [Display(Name = "用户名")]
        public string UserName { get; set; }

        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        public string Content { get; set; }

        [Required]
        public string Captcha { get; set; }

        [Required]
        public string PostId { get; set; }

        public List<CommentItem> Comments
        {
            get; set;
        }

    }


}
