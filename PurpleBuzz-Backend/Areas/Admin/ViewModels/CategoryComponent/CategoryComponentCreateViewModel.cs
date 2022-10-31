using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace PurpleBuzz_Backend.Areas.Admin.ViewModels.CategoryComponent
{
    public class CategoryComponentCreateViewModel
    {
        [Required]
        public string Title { get; set; }

        [MinLength(10)]
        public string Description { get; set; }

        [Required]
        public IFormFile Photo { get; set; }

        [Required]
        [Display(Name = "Category")]
        public int CategoryId { get; set; }

        public List<SelectListItem>? Categories { get; set; }
    }
}
