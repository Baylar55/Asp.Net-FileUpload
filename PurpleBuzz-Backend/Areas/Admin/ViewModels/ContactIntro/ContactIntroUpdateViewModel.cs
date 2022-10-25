using System.ComponentModel.DataAnnotations;

namespace PurpleBuzz_Backend.Areas.Admin.ViewModels.ContactIntro
{
    public class ContactIntroUpdateViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Title can't be null"), MinLength(3, ErrorMessage = "Title must contain 3 letter at least")]
        public string? Title { get; set; }

        [Required(ErrorMessage = "Text can't be null")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Primary text can't be null")]
        public string PrimaryText { get; set; }

        public string? PhotoName { get; set; }
      
        public IFormFile? Photo { get; set; }
    }
}
