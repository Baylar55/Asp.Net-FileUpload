using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PurpleBuzz_Backend.DAL;
using PurpleBuzz_Backend.ViewModels.About;

namespace PurpleBuzz_Backend.Controllers
{
    public class AboutController : Controller
    {
        private readonly AppDbContext _appDbContext;

        public AboutController(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }
        public async Task<IActionResult> Index()
        {
            //var model = new AboutIndexViewModel
            //{
            //    objectiveComponents = await _appDbContext.ObjectiveComponents.ToListAsync()
            //};
            //return View(model);
            return View();
        }
    }
}
