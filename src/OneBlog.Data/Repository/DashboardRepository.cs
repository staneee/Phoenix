using OneBlog.Data.Contracts;
using OneBlog.Data.Models.ManageViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace OneBlog.Data.Repository
{
    public class DashboardRepository : IDashboardRepository
    {
        private AppDbContext _ctx;

        public DashboardRepository(AppDbContext ctx)
        {
            _ctx = ctx;
        }

        /// <summary>
        /// Get all dashboard items
        /// </summary>
        /// <returns>Dashboard view model</returns>
        public DashboardViewModel Get()
        {
            return new DashboardViewModel(_ctx);
        }
    }
}
