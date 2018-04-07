using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace OneBlog.Configuration
{
    public class DataSettings
    {
        public string ConnectionString { get; set; }

        public Microsoft.EntityFrameworkCore.DbType DbProvider { get; set; }
    }
}
