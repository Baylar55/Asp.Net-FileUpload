using PurpleBuzz_Backend.Models;

namespace PurpleBuzz_Backend.Areas.Admin.ViewModels.FeaturedWorkComponent
{
    public class FeaturedWorkComponentUpdateViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public List<IFormFile>? Photos { get; set; }
        public List<Models.FeaturedWorkComponentPhoto>  FeaturedWorkComponentPhotos { get; set; }
    }
}
