using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PurpleBuzz_Backend.Areas.Admin.ViewModels;
using PurpleBuzz_Backend.DAL;
using PurpleBuzz_Backend.Models;

namespace PurpleBuzz_Backend.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ContactIntroController : Controller
    {
        private readonly AppDbContext _appDbContext;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ContactIntroController(AppDbContext appDbContext,IWebHostEnvironment webHostEnvironment)
        {
            _appDbContext = appDbContext;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<IActionResult> Index()
        {
            var model = new ContactIntroIndexViewModel
            {
                ContactIntro = await _appDbContext.ContactIntro.ToListAsync()
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

            if (!contactIntro.Photo.ContentType.Contains("image/"))
            {
                ModelState.AddModelError("Photo", "Uploaded file should be in image format");
                return View(contactIntro);
            }

            if (contactIntro.Photo.Length / 1024 > 60)
            {
                ModelState.AddModelError("Photo", "Photo size is greater than 60kB");
                return View(contactIntro);
            }

            var fileName = $"{Guid.NewGuid()}_{contactIntro.Photo.FileName}";
            var path = Path.Combine(_webHostEnvironment.WebRootPath, "assets/img", fileName);

            using (FileStream fileStream = new FileStream(path, FileMode.Create, FileAccess.ReadWrite))
            {
                await contactIntro.Photo.CopyToAsync(fileStream);
            }

                contactIntro.PhotoName = fileName;

            await _appDbContext.ContactIntro.AddAsync(contactIntro);
            await _appDbContext.SaveChangesAsync();
            return RedirectToAction("index");
        }



        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            var dbContactIntro = await _appDbContext.ContactIntro.FindAsync(id);
            if (dbContactIntro == null) return NotFound();
            return View(dbContactIntro);
        }


        [HttpPost]
        public async Task<IActionResult> Update(int id, ContactIntro ContactIntro)
        {
            if (!ModelState.IsValid) return View(ContactIntro);

            var dbContactIntro = await _appDbContext.ContactIntro.FindAsync(id);
            if (dbContactIntro == null) return NotFound();

            if (id != ContactIntro.Id) return BadRequest();
            string fname = Path.Combine(_webHostEnvironment.WebRootPath, "assets/img", dbContactIntro.PhotoName);
            FileInfo file = new FileInfo(fname);
            if (file.Exists)
            {
                System.IO.File.Delete(fname);
                file.Delete();
            }

            if (!ContactIntro.Photo.ContentType.Contains("image/"))
            {
                ModelState.AddModelError("Photo", "Uploaded file should be in image format");
                return View(ContactIntro);
            }

            if (ContactIntro.Photo.Length / 1024 > 150)
            {
                ModelState.AddModelError("Photo", "Photo size is greater than 60kB");
                return View(ContactIntro);
            }
            var fileName = $"{Guid.NewGuid()}'_'{ContactIntro.Photo.FileName}";
            var path = Path.Combine(_webHostEnvironment.WebRootPath, "assets/img", fileName);

            using (FileStream fileStream = new FileStream(path, FileMode.Create, FileAccess.ReadWrite))
            {
                await ContactIntro.Photo.CopyToAsync(fileStream);
            }

            dbContactIntro.Title = ContactIntro.Title;
            dbContactIntro.Description = ContactIntro.Description;
            dbContactIntro.PrimaryText = ContactIntro.PrimaryText;
            dbContactIntro.PhotoName = fileName;
            await _appDbContext.SaveChangesAsync();
            return RedirectToAction("index");
        }


        public async Task<IActionResult> Delete(int id)
        {
            var dbContactIntro = await _appDbContext.ContactIntro.FindAsync(id);
            if (dbContactIntro == null) return NotFound();
            return View(dbContactIntro);
        }

        public async Task<IActionResult> DeleteMember(int id)
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

        //[HttpGet]
        //public async Task<IActionResult> Update(int id)
        //{
        //    var contactIntroComponent = await _appDbContext.ContactIntro.FindAsync(id);
        //    if (contactIntroComponent == null) return NotFound();
        //    return View(contactIntroComponent);
        //}

        //[HttpPost]
        //public async Task<IActionResult> Update(int id, ContactIntro contactIntroComponent)
        //{
        //    if (!ModelState.IsValid) return View(contactIntroComponent);
        //    bool isExist = await _appDbContext.ContactIntro.AnyAsync(c => c.Title.ToLower().Trim() == contactIntroComponent.Title.ToLower().Trim() && c.Id != contactIntroComponent.Id);
        //    if (isExist)
        //    {
        //        ModelState.AddModelError("Title", "This title is already exist");
        //        return View(contactIntroComponent);
        //    }
        //    if (id != contactIntroComponent.Id) return BadRequest();

        //    var dbContactIntroComponent = await _appDbContext.ContactIntro.FindAsync(id);
        //    if (dbContactIntroComponent == null) return NotFound();
        //    dbContactIntroComponent.Title = contactIntroComponent.Title;
        //    dbContactIntroComponent.Description = contactIntroComponent.Description;
        //    dbContactIntroComponent.FilePath = contactIntroComponent.FilePath;
        //    await _appDbContext.SaveChangesAsync();
        //    return RedirectToAction("index");
        //}

        //[HttpGet]
        //public async Task<IActionResult> Delete(int id)
        //{
        //    var contactIntroComponent = await _appDbContext.ContactIntro.FindAsync(id);
        //    if (contactIntroComponent == null) return NotFound();
        //    return View(contactIntroComponent);
        //}

        //[HttpPost]
        //public async Task<IActionResult> DeleteComponent(int id)
        //{
        //    var contactIntroComponent = await _appDbContext.ContactIntro.FindAsync(id);
        //    if (contactIntroComponent == null) return NotFound();
        //    _appDbContext.Remove(contactIntroComponent);
        //    await _appDbContext.SaveChangesAsync();
        //    return RedirectToAction("index");
        //}

        //[HttpGet]
        //public async Task<IActionResult> Details(int id)
        //{
        //    var contactIntroComponent = await _appDbContext.ContactIntro.FindAsync(id);
        //    if (contactIntroComponent == null) return NotFound();
        //    return View(contactIntroComponent);
        //}
    }
}
