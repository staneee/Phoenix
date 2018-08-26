using Microsoft.AspNetCore.Mvc;
using Phoenix.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Phoenix.Helpers
{
    public static class BusinessExtensions
    {
        /// <summary>
        /// 详情页Link
        /// </summary>
        /// <param name="url"></param>
        /// <param name="id"></param>
        /// <param name="slug"></param>
        /// <returns></returns>
        public static string Link_Post(this IUrlHelper url, string id, string slug)
        {
            if (!string.IsNullOrEmpty(slug))
            {
                return url.Action("post", "root", new { slug = slug });
            }
            return url.Action("post", "root", new { slug = id });
        }
    }
}
