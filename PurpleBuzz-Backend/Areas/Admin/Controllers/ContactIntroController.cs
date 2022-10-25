using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PurpleBuzz_Backend.Areas.Admin.ViewModels;
using PurpleBuzz_Backend.Areas.Admin.ViewModels.ContactIntro;
using PurpleBuzz_Backend.DAL;
using PurpleBuzz_Backend.Helpers;
using PurpleBuzz_Backend.Models;

namespace PurpleBuzz_Backend.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ContactIntroController : Controller
    {
        private readonly AppDbContext _appDbContext;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IFileService _fileService;

        public ContactIntroController(AppDbContext appDbContext, IWebHostEnvironment webHostEnvironment,IFileService fileService)
        {
            _appDbContext = appDbContext;
            _webHostEnvironment = webHostEnvironment;
           _fileService = fileService;
        }

        public async Task<IActionResult> Index()
        {
            var model = new ContactIntroIndexViewModel
            {
                ContactIntro =await _appDbContext.ContactIntro.FirstOrDefaultAsync()
            };
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(ContactIntro contactIntro)
        {
            if (!ModelState.IsValid) return View(contactIntro);
            bool isExist = await _appDbContext.ContactIntro.AnyAsync(c => c.Title.ToLower().Trim() == contactIntro.Title.ToLower().Trim());

            if (isExist)
            {
                ModelState.AddModelError("Title", "This title is already exist");
                return View(contactIntro);
            }

            if (!_fileService.IsImage(contactIntro.Photo))
            {
                ModelState.AddModelError("Photo", "Uploaded file should be in image format");
                return View(contactIntro);
            }

            if (!_fileService.checkSize(contactIntro.Photo, 60))
            {
                ModelState.AddModelError("Photo", "Photo size is greater than 60kB");
                return View(contactIntro);
            }

            contactIntro.PhotoName = await _fileService.UploadAsync(contactIntro.Photo, _webHostEnvironment.WebRootPath);

            await _appDbContext.ContactIntro.AddAsync(contactIntro);
            await _appDbContext.SaveChangesAsync();
            return RedirectToAction("index");
        }



        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            var dbContactIntro = await _appDbContext.ContactIntro.FindAsync(id);
            if (dbContactIntro == null) return NotFound();
            var model = new ContactIntroUpdateViewModel
            {
                Id = dbContactIntro.Id,
                Title = dbContactIntro.Title,
                PrimaryText=dbContactIntro.PrimaryText,
                Description=dbContactIntro.Description,
                PhotoName=dbContactIntro.PhotoName
            };
            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> Update(int id, ContactIntroUpdateViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var dbContactIntro = await _appDbContext.ContactIntro.FindAsync(id);
            if (dbContactIntro == null) return NotFound();

            if (id != model.Id) return BadRequest();
            string fname = Path.Combine(_webHostEnvironment.WebRootPath, "assets/img", dbContactIntro.PhotoName);
            FileInfo file = new FileInfo(fname);
            if (file.Exists)
            {
                System.IO.File.Delete(fname);
                file.Delete();
            }

            if (!_fileService.IsImage(model.Photo))
            {
                ModelState.AddModelError("Photo", "Uploaded file should be in image format");
                return View(model);
            }

            if (!_fileService.checkSize(model.Photo,150))
            {
                ModelState.AddModelError("Photo", "Photo size is greater than 60kB");
                return View(model);
            }
           
            dbContactIntro.Title = model.Title;
            dbContactIntro.Description = model.Description;
            dbContactIntro.PrimaryText = model.PrimaryText;
            dbContactIntro.PhotoName = await _fileService.UploadAsync(model.Photo, _webHostEnvironment.WebRootPath);
            await _appDbContext.SaveChangesAsync();
            return RedirectToAction("index");
        }

        public async Task<IActionResult> Delete(int id)
        {
            var dbContactIntro = await _appDbContext.ContactIntro.FindAsync(id);
            if (dbContactIntro == null) return NotFound();
            return View(dbContactIntro);
        }

        public async Task<IActionResult> DeleteIntro(int id)
        {
            var dbContactIntro = await _appDbContext.ContactIntro.FindAsync(id);
            if (dbContactIntro == null) return NotFound();

            string fname = Path.Combine(_webHostEnvironment.WebRootPath, "assets/img", dbContactIntro.PhotoName);
            FileInfo file = new FileInfo(fname);
            if (file.Exists)
            {
                System.IO.File.Delete(fname);
                file.Delete();
            }

            _appDbContext.ContactIntro.Remove(dbContactIntro);
            return RedirectToAction("index");
        }

        public async Task<IActionResult> Details(int id)
        {
            var dbContactIntro = await _appDbContext.ContactIntro.FindAsync(id);
            if (dbContactIntro == null) return NotFound();
            return View(dbContactIntro);
        }
    }
}
