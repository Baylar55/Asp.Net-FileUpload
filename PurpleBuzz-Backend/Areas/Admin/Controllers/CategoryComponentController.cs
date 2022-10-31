using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PurpleBuzz_Backend.Areas.Admin.ViewModels.CategoryComponent;
using PurpleBuzz_Backend.DAL;
using PurpleBuzz_Backend.Helpers;
using PurpleBuzz_Backend.Models;

namespace PurpleBuzz_Backend.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryComponentController : Controller
    {
        private readonly AppDbContext _appDbContext;
        private readonly IFileService _fileService;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public CategoryComponentController(AppDbContext appDbContext, IFileService fileService, IWebHostEnvironment webHostEnvironment)
        {
            _appDbContext = appDbContext;
            _fileService = fileService;
            _webHostEnvironment = webHostEnvironment;
        }
        public async Task<IActionResult> Index()
        {
            var model = new CategoryComponentIndexViewModel
            {
                CategoryComponents = await _appDbContext.CategoryComponents.Include(cc => cc.Category).ToListAsync()
            };
            return View(model);
        }
        public async Task<IActionResult> Create()
        {
            var model = new CategoryComponentCreateViewModel
            {
                Categories = await _appDbContext.Categories.Select(c => new SelectListItem
                {
                    Text = c.Title,
                    Value = c.Id.ToString()
                }).ToListAsync()
            };
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> Create(CategoryComponentCreateViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            if (await _appDbContext.Categories.FindAsync(model.CategoryId) == null)
            {
                ModelState.AddModelError("CategoryId", "This category isn't exist");
            }

            bool isExist = await _appDbContext.CategoryComponents
                        .AnyAsync(cc => cc.Title.ToLower().Trim() == model.Title.ToLower().Trim());
            if (isExist)
            {
                ModelState.AddModelError("Title", "This category component is already exist");
            }

            if (!_fileService.IsImage(model.Photo))
            {
                ModelState.AddModelError("Photo", "Uploaded file should be in image format");
                return View(model);
            }

            var categoryComponent = new CategoryComponent
            {
                Title = model.Title,
                Description = model.Description,
                CategoryId = model.CategoryId,
                FilePath = await _fileService.UploadAsync(model.Photo, _webHostEnvironment.WebRootPath)
            };

            await _appDbContext.AddAsync(categoryComponent);
            await _appDbContext.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            var categoryComponent = await _appDbContext.CategoryComponents.FindAsync(id);
            if (categoryComponent == null) return NotFound();
            var model = new CategoryComponentUpdateViewModel
            {
                Title = categoryComponent.Title,
                Description = categoryComponent.Description,
                PhotoPath = categoryComponent.FilePath,
                CategoryId = categoryComponent.CategoryId,
                Categories = await _appDbContext.Categories.Select(c => new SelectListItem
                {
                    Text = c.Title,
                    Value = c.Id.ToString()
                })
                .ToListAsync()
            };
            return View(model);
        }

        public async Task<IActionResult> Update(int id, CategoryComponentUpdateViewModel model)
        {
            model.Categories = await _appDbContext.Categories.Select(c => new SelectListItem
            {
                Text = c.Title,
                Value = c.Id.ToString()
            })
                .ToListAsync();

            if (!ModelState.IsValid) return View(model);

            if (id != model.Id) return BadRequest();

            var categoryComponent = await _appDbContext.CategoryComponents.FindAsync(id);
            if (categoryComponent == null) return NotFound();

            bool isExist = await _appDbContext.CategoryComponents.AnyAsync(cc => cc.Title.ToLower().Trim() == model.Title.ToLower().Trim() && cc.Id != categoryComponent.Id);
            if (isExist)
            {
                ModelState.AddModelError("Title", "This title is already exist");
            }

            var category = await _appDbContext.Categories.FindAsync(model.CategoryId);
            if(category == null) return NotFound();

            categoryComponent.Title=model.Title;
            categoryComponent.Description=model.Description;
            if (model.Photo != null)
            {
                if (!_fileService.IsImage(model.Photo))
                {
                    ModelState.AddModelError("Photo", "Photo must be in image format");
                    return View(model);
                }
                if (!_fileService.checkSize(model.Photo, 300))
                {
                    ModelState.AddModelError("Photo", "Photo size should be smaller than 300kb");
                    return View(model);
                }
                _fileService.Delete(model.PhotoPath, _webHostEnvironment.WebRootPath);
                categoryComponent.FilePath = await _fileService.UploadAsync(model.Photo, _webHostEnvironment.WebRootPath);
            }
            await _appDbContext.SaveChangesAsync();
            return RedirectToAction("index");
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var categoryComponent = await _appDbContext.CategoryComponents.FindAsync(id);
            if(categoryComponent==null) return NotFound();

            _fileService.Delete(categoryComponent.FilePath,_webHostEnvironment.WebRootPath);
            _appDbContext.CategoryComponents.Remove(categoryComponent);
            await _appDbContext.SaveChangesAsync();
            return RedirectToAction("index");
        }


    }
}
