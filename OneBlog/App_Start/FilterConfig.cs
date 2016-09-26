﻿using System.Web;
using System.Web.Mvc;

namespace OneBlog
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new GlobalHandleErrorAttribute());
            filters.Add(new RedirectAttribute());
        }
    }
}
