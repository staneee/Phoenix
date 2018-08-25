using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Phoenix.Configuration
{
    public class DataSettings
    {
        public string ConnectionString { get; set; }

        public DbProvider DbProvider { get; set; }
    }
}
