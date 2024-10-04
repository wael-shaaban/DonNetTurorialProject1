using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DonNetTurorialProject1.Models.Entities
{
    public class BorrowRecord
    {
        [Key]
        public int BorrowRecordId { get; set; } //PK
        [Required(ErrorMessage = "Please enter Borrower Name")]
        public string BorrowerName { get; set; }
        [Required(ErrorMessage = "Please enter Borrower Email Address")]
        [EmailAddress(ErrorMessage = "Please enter a Email Address")]
        public string BorrowerEmail { get; set; }
        [Required(ErrorMessage = "Please enter Borrower Phone Number")]
        [Phone(ErrorMessage = "Please enter a Valid Phone Number")]
        public string Phone { get; set; }
        [BindNever]
        [DataType(DataType.DateTime)]
        public DateTime BorrowDate { get; set; } = DateTime.UtcNow;
        [DataType(DataType.DateTime)]
        public DateTime? ReturnDate { get; set; }
        // Navigation Properties
        [Required]
        [ForeignKey(nameof(Book))]
        public int BookId { get; set; } //FK
        [BindNever]
        public Book Book { get; set; }
    }
}
