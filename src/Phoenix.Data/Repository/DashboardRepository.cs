using Phoenix.Data.Contracts;
using Phoenix.Data.Models.ManageViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace Phoenix.Data.Repository
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
