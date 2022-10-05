using Microsoft.AspNetCore.Mvc;
using PurpleBuzz_Backend.Models;
using PurpleBuzz_Backend.ViewModels.Home;

namespace PurpleBuzz_Backend.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            var projectComponents = new List<ProjectComponent>
            {
                new ProjectComponent{Title = "UX/UI design", Text = "UI/UX design text", FilePath = "/assets/img/services-01.jpg"},
                new ProjectComponent{Title = "Social Media", Text = "Social Media text", FilePath = "/assets/img/services-02.jpg"},
                new ProjectComponent{Title = "Marketing", Text = "Marketing text", FilePath = "/assets/img/services-03.jpg"},
                new ProjectComponent{Title = "Graphic", Text = "Graphic text", FilePath = "/assets/img/services-04.jpg"}
            };
            var recentWorkComponents = new List<RecentWorkComponent>
            {
                new RecentWorkComponent{Title="Social Media", Text="Social media text", FilePath="/assets/img/recent-work-01.jpg"},
                new RecentWorkComponent{Title="Web Marketing", Text="Web marketing text", FilePath="/assets/img/recent-work-02.jpg"},
                new RecentWorkComponent{Title="R & D", Text="R & D text", FilePath="/assets/img/recent-work-03.jpg"}
            };
            var model = new HomeIndexViewModel
            {
                ProjectComponents = projectComponents,
                RecentWorkComponents = recentWorkComponents
            };
            return View(model);
        }
    }
}
