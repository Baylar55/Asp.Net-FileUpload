using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PurpleBuzz_Backend.Areas.Admin.ViewModels;
using PurpleBuzz_Backend.DAL;
using PurpleBuzz_Backend.Models;

namespace PurpleBuzz_Backend.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class TeamMemberController : Controller
    {
        private readonly AppDbContext _appDbContext;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public TeamMemberController(AppDbContext appDbContext, IWebHostEnvironment webHostEnvironment)
        {
            _appDbContext = appDbContext;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<IActionResult> Index()
        {
            var model = new TeamMemberIndexViewModel
            {
                TeamMembers = await _appDbContext.TeamMembers.ToListAsync()
            };
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Create(TeamMember teamMember)
        {
            if (!ModelState.IsValid) return View(teamMember);

            if (!teamMember.Photo.ContentType.Contains("image/"))
            {
                ModelState.AddModelError("Photo", "Uploaded file should be in image format");
                return View(teamMember);
            }

            if (teamMember.Photo.Length / 1024 > 60)
            {
                ModelState.AddModelError("Photo", "Photo size is greater than 60kB");
                return View(teamMember);
            }

            var fileName = $"{Guid.NewGuid()}_{teamMember.Photo.FileName}";
            var path = Path.Combine(_webHostEnvironment.WebRootPath, "assets/img", fileName);

            using (FileStream fileStream = new FileStream(path, FileMode.Create, FileAccess.ReadWrite))
            {
                await teamMember.Photo.CopyToAsync(fileStream);
            }

            teamMember.PhotoPath = fileName;
            await _appDbContext.TeamMembers.AddAsync(teamMember);
            await _appDbContext.SaveChangesAsync();
            return RedirectToAction("index");
        }


        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            var dbTeamMember = await _appDbContext.TeamMembers.FindAsync(id);
            if (dbTeamMember == null) return NotFound();
            return View(dbTeamMember);
        }   


        [HttpPost]
        public async Task<IActionResult> Update(int id, TeamMember teamMember)
        {
            if (!ModelState.IsValid) return View(teamMember);

            var dbTeamMember = await _appDbContext.TeamMembers.FindAsync(id);
            if (dbTeamMember == null) return NotFound();

            if (id != teamMember.id) return BadRequest();
            string fname = Path.Combine(_webHostEnvironment.WebRootPath, "assets/img", dbTeamMember.PhotoPath);
            FileInfo file = new FileInfo(fname);
            if (file.Exists)
            {
                System.IO.File.Delete(fname);
                file.Delete();
            }

            if (!teamMember.Photo.ContentType.Contains("image/"))
            {
                ModelState.AddModelError("Photo", "Uploaded file should be in image format");
                return View(teamMember);
            }

            if (teamMember.Photo.Length / 1024 > 60)
            {
                ModelState.AddModelError("Photo", "Photo size is greater than 60kB");
                return View(teamMember);
            }
            var fileName = $"{Guid.NewGuid()}'_'{teamMember.Photo.FileName}";
            var path = Path.Combine(_webHostEnvironment.WebRootPath, "assets/img", fileName);

            using (FileStream fileStream = new FileStream(path, FileMode.Create, FileAccess.ReadWrite))
            {
                await teamMember.Photo.CopyToAsync(fileStream);
            }

            dbTeamMember.Name = teamMember.Name;
            dbTeamMember.Surname = teamMember.Surname;
            dbTeamMember.Position = teamMember.Position;
            dbTeamMember.PhotoPath = fileName;
            await _appDbContext.SaveChangesAsync();
            return RedirectToAction("index");
        }

        public async Task<IActionResult> Details(int id)
        {
            var dbTeamMember = await _appDbContext.TeamMembers.FindAsync(id);
            if (dbTeamMember == null) return NotFound();
            return View(dbTeamMember);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var dbTeamMember = await _appDbContext.TeamMembers.FindAsync(id);
            if (dbTeamMember == null) return NotFound();
            return View(dbTeamMember);
        }

        public async Task<IActionResult> DeleteMember(int id)
        {
            var dbTeamMember = await _appDbContext.TeamMembers.FindAsync(id);
            if (dbTeamMember == null) return NotFound();

            string fname = Path.Combine(_webHostEnvironment.WebRootPath, "assets/img", dbTeamMember.PhotoPath);
            FileInfo file = new FileInfo(fname);
            if (file.Exists)
            {
                System.IO.File.Delete(fname);
                file.Delete();
            }

            _appDbContext.TeamMembers.Remove(dbTeamMember);
            return RedirectToAction("index");
        }
    }
}
