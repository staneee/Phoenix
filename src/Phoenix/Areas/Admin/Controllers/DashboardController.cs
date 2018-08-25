using Microsoft.AspNetCore.Mvc;
using Phoenix.Data.Contracts;
using Phoenix.Data.Models.ManageViewModels;

namespace Phoenix.Areas.Admin.Controllers
{
    [Route("api/[controller]")]
    public class DashboardController : Controller
    {
        readonly IDashboardRepository repository;

        public DashboardController(IDashboardRepository repository)
        {
            this.repository = repository;
        }

        public DashboardViewModel Get()
        {
            return repository.Get();
        }
    }
}
