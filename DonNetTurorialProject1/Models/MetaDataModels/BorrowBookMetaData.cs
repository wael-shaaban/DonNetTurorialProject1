namespace DonNetTurorialProject1.Models.MetaDataModels
{
  using System.ComponentModel.DataAnnotations;

public class BorrowRecordMetadata
{
    [Required(ErrorMessage = "Please enter Borrower Name")]
    public string BorrowerName { get; set; }

    [Required(ErrorMessage = "Please enter Borrower Email Address")]
    [EmailAddress(ErrorMessage = "Please enter a valid Email Address")]
    public string BorrowerEmail { get; set; }

    [Required(ErrorMessage = "Please enter Borrower Phone Number")]
    [Phone(ErrorMessage = "Please enter a valid Phone Number")]
    public string Phone { get; set; }

    [DataType(DataType.DateTime)]
    public DateTime BorrowDate { get; set; }

    [DataType(DataType.DateTime)]
    public DateTime? ReturnDate { get; set; }
}

}
