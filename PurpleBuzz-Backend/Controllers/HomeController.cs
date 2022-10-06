using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PurpleBuzz_Backend.DAL;
using PurpleBuzz_Backend.Models;
using PurpleBuzz_Backend.ViewModels.Home;

namespace PurpleBuzz_Backend.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _appDbContext;
        public HomeController(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }   
        public async Task<IActionResult> Index()
        {
            var recentWorkComponents = await _appDbContext.RecentWorkComponents.ToListAsync();
            var model = new HomeIndexViewModel
            {
                RecentWorkComponents = recentWorkComponents
            };
            return View(model);
        }
        
    }
}
