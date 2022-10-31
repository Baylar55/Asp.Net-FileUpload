using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PurpleBuzz_Backend.Areas.Admin.ViewModels.FeaturedWorkComponent;
using PurpleBuzz_Backend.Areas.Admin.ViewModels.FeaturedWorkComponent.FeaturedWorkComponentPhoto;
using PurpleBuzz_Backend.DAL;
using PurpleBuzz_Backend.Helpers;
using PurpleBuzz_Backend.Models;

namespace PurpleBuzz_Backend.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class FeaturedWorkComponentController : Controller
    {
        private readonly AppDbContext _appDbContext;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IFileService _fileService;

        public FeaturedWorkComponentController(AppDbContext appDbContext, IWebHostEnvironment webHostEnvironment, IFileService fileService)
        {
            _appDbContext = appDbContext;
            _webHostEnvironment = webHostEnvironment;
            _fileService = fileService;
        }


        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var model = new FeaturedWorkComponentIndexViewModel
            {
                FeaturedWorkComponent = await _appDbContext.FeaturedWorkComponent.FirstOrDefaultAsync()
            };
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var featuredWorkComponent = await _appDbContext.FeaturedWorkComponent.FirstOrDefaultAsync();
            if (featuredWorkComponent != null) return NotFound();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(FeaturedWorkComponentCreateViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var featuredWorkComponent = new FeaturedWorkComponent
            {
                Title = model.Title,
                Description = model.Description
            };

            await _appDbContext.FeaturedWorkComponent.AddAsync(featuredWorkComponent);
            await _appDbContext.SaveChangesAsync();


            bool hasError = false;
            foreach (var photo in model.Photos)
            {
                if (!_fileService.IsImage(photo))
                {
                    ModelState.AddModelError("Photos", $"{photo.FileName} should be in image format");
                    hasError = true;
                }
                else if (!_fileService.checkSize(photo, 300))
                {
                    ModelState.AddModelError("Photos", $"{photo.FileName}'s size sould be smaller than 300kb");
                    hasError = true;
                }

            }
            if (hasError) return View(model);

            int order = 1;
            foreach (var photo in model.Photos)
            {
                var featuredWorkComponentPhoto = new FeaturedWorkComponentPhoto
                {
                    FeaturedWorkComponentId = featuredWorkComponent.Id,
                    Name = await _fileService.UploadAsync(photo, _webHostEnvironment.WebRootPath),
                    Order = order
                };
                await _appDbContext.FeaturedWorkComponentPhotos.AddAsync(featuredWorkComponentPhoto);
                await _appDbContext.SaveChangesAsync();

                order++;
            }
            return RedirectToAction("index");
        }

        [HttpGet]
        public async Task<IActionResult> Delete()
        {
            var featuredWorkComponent = await _appDbContext.FeaturedWorkComponent
                                                .Include(fwc => fwc.FeaturedWorkComponentPhotos)
                                                .FirstOrDefaultAsync();

            if (featuredWorkComponent == null) return NotFound();

            foreach (var photo in featuredWorkComponent.FeaturedWorkComponentPhotos)
            {
                _fileService.Delete(photo.Name, _webHostEnvironment.WebRootPath);
            }

            _appDbContext.FeaturedWorkComponent.Remove(featuredWorkComponent);
            await _appDbContext.SaveChangesAsync();

            return RedirectToAction("index");
        }

        [HttpGet]
        public async Task<IActionResult> Update()
        {
            var featuredWorkComponent = await _appDbContext.FeaturedWorkComponent
                                                            .Include(fwc => fwc.FeaturedWorkComponentPhotos)
                                                            .FirstOrDefaultAsync();

            if (featuredWorkComponent == null) return NotFound();

            var model = new FeaturedWorkComponentUpdateViewModel
            {
                Title = featuredWorkComponent.Title,
                Description = featuredWorkComponent.Description,
                FeaturedWorkComponentPhotos = featuredWorkComponent.FeaturedWorkComponentPhotos.ToList(),
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Update(FeaturedWorkComponentUpdateViewModel model)
        {
            var dbFeaturedWorkComponent = await _appDbContext.FeaturedWorkComponent
                                                            .Include(fwc => fwc.FeaturedWorkComponentPhotos)
                                                            .FirstOrDefaultAsync();

            model.FeaturedWorkComponentPhotos = dbFeaturedWorkComponent.FeaturedWorkComponentPhotos.ToList();

            if (ModelState.IsValid) return View(model);

            dbFeaturedWorkComponent.Title = model.Title;
            dbFeaturedWorkComponent.Description = model.Description;

            await _appDbContext.SaveChangesAsync();


            bool hasError = false;
            if (model.Photos != null)
            {
                foreach (var photo in model.Photos)
                {
                    if (!_fileService.IsImage(photo))
                    {
                        ModelState.AddModelError("Photos", $"{photo.FileName} should be in image format");
                        hasError = true;
                    }
                    else if (!_fileService.checkSize(photo, 300))
                    {
                        ModelState.AddModelError("Photos", $"{photo.FileName}'s size sould be smaller than 300kb");
                        hasError = true;
                    }

                }
                if (hasError) return View(model);
                int order = 1;
                foreach (var photo in model.Photos)
                {
                    var featuredWorkComponentPhoto = new FeaturedWorkComponentPhoto
                    {
                        FeaturedWorkComponentId = dbFeaturedWorkComponent.Id,
                        Name = await _fileService.UploadAsync(photo, _webHostEnvironment.WebRootPath),
                        Order = order
                    };
                    await _appDbContext.FeaturedWorkComponentPhotos.AddAsync(featuredWorkComponentPhoto);
                    await _appDbContext.SaveChangesAsync();

                    order++;
                }
            }
            return RedirectToAction("index");

        }



        #region FeatureWorkComponentPhoto

        [HttpGet]
        public async Task<IActionResult> UpdatePhoto(int id)
        {
            var featuredWorkComponentPhoto = await _appDbContext.FeaturedWorkComponentPhotos.FindAsync(id);
            if (featuredWorkComponentPhoto == null) return NotFound();

            var model = new FeaturedWorkComponentPhotoUpdateViewModel
            {
                Order = featuredWorkComponentPhoto.Order
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> UpdatePhoto(int id, FeaturedWorkComponentPhotoUpdateViewModel model)
        {
            if (!ModelState.IsValid) return View(model);
            if (id != model.Id) return BadRequest();
            var featuredWorkComponentPhoto = await _appDbContext.FeaturedWorkComponentPhotos.FindAsync(model.Id);
            if (featuredWorkComponentPhoto == null) return NotFound();
            featuredWorkComponentPhoto.Order = model.Order;

            await _appDbContext.SaveChangesAsync();

            return RedirectToAction("update", "featuredworkcomponent", new { id = featuredWorkComponentPhoto.FeaturedWorkComponentId });
        }

        [HttpGet]
        public async Task<IActionResult> DeletePhoto(int id)
        {
            var featuredWorkComponentPhoto = await _appDbContext.FeaturedWorkComponentPhotos.FindAsync(id);
            if (featuredWorkComponentPhoto == null) return NotFound();
            _fileService.Delete(featuredWorkComponentPhoto.Name, _webHostEnvironment.WebRootPath);
            _appDbContext.FeaturedWorkComponentPhotos.Remove(featuredWorkComponentPhoto);
            await _appDbContext.SaveChangesAsync();
            return RedirectToAction("update");
        }


        #endregion


        public async Task<IActionResult> Details()
        {
            var featuredworkComponent = await _appDbContext.FeaturedWorkComponent
                .Include(rcwp => rcwp.FeaturedWorkComponentPhotos)
                .FirstOrDefaultAsync();

            if (featuredworkComponent == null) return NotFound();

            var model = new FeaturedWorkComponentDetailsViewModel
            {
                Id = featuredworkComponent.Id,
                Title = featuredworkComponent.Title,
                Description = featuredworkComponent.Description,
                FeaturedWorkComponentPhotos = _appDbContext.FeaturedWorkComponentPhotos.ToList()
            };

            return View(model);
        }
    }
}
