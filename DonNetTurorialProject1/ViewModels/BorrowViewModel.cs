
namespace DonNetTurorialProject1.ViewModels
{
    //CreateOrUpdateViewModel
    public class BorrowViewModel
    {
        [Required]
        public int BookId { get; set; }

        [BindNever]
        public string? BookTitle { get; set; }

        [Required(ErrorMessage = "Your name is required.")]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters.")]
        public string BorrowerName { get; set; }

        [Required(ErrorMessage = "Your email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string BorrowerEmail { get; set; }

        [Required(ErrorMessage = "Your Phone Number is required.")]
        [Phone(ErrorMessage = "Invalid Phone Number")]
        public string Phone { get; set; }
    }
}