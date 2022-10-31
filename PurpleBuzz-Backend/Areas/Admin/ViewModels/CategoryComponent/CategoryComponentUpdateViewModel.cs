using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace PurpleBuzz_Backend.Areas.Admin.ViewModels.CategoryComponent
{
    public class CategoryComponentUpdateViewModel
    {
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }

        [MinLength(10)]
        public string Description { get; set; }

        public IFormFile? Photo { get; set; }

        public string PhotoPath { get; set; }

        [Required]
        [Display(Name = "Category")]
        public int CategoryId { get; set; }

        public List<SelectListItem>? Categories { get; set; }
    }
}
