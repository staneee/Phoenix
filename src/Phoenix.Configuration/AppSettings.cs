﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Phoenix.Configuration
{
    public class AppSettings
    {
        public string Title { get; set; }

        public string Description { get; set; }

        public int PostPerPage { get; set; }

        public string Theme { get; set; }

        public bool TestData { get; set; }
    }
}
