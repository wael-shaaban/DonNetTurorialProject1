using DonNetTurorialProject1.Data.Repositories;
using DonNetTurorialProject1.Models.Entities;

namespace DonNetTurorialProject1.Controllers
{
    public class BorrowController(IUnitofWorkService _unitofWorkService) : Controller
    {
        // Displays the borrow form for a specific book.
        // GET: Borrow/Create/5
        public async Task<IActionResult> Create(int? bookId)
        {
            if (bookId == null || bookId == 0)
            {
                TempData["ErrorMessage"] = "Book ID was not provided for borrowing.";
                return View("NotFound");
            }
            try
            {
                var book = _unitofWorkService.BookService.GetByID(bookId.Value);
                if (book == null)
                {
                    TempData["ErrorMessage"] = $"No book found with ID {bookId} to borrow.";
                    return View("NotFound");
                }
                if (!book.IsAvailable)
                {
                    TempData["ErrorMessage"] = $"The book '{book.Title}' is currently not available for borrowing.";
                    return View("NotAvailable");
                }
                return View(_unitofWorkService.BookService.GetBorrowerdBook(book));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while loading the borrow form.";
                return View("Error");
            }
        }
        // Processes the borrowing action, creates a BorrowRecord, updates the book's availability
        // POST: Borrow/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BorrowViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            try
            {
                var book = _unitofWorkService.BookService.GetByID(model.BookId);
                if (book is null)
                {
                    TempData["ErrorMessage"] = $"No book found with ID {model.BookId} to borrow.";
                    return View("NotFound");
                }
                if (!book.IsAvailable)
                {
                    TempData["ErrorMessage"] = $"The book '{book.Title}' is already borrowed.";
                    return View("NotAvailable");
                }
                var borrowRecord = _unitofWorkService.BorrowBookService.GetBorrowRecord(model);
                // Update the book's availability
                book.IsAvailable = false;
               int success =  _unitofWorkService.BorrowBookService.Add(borrowRecord);
                if (success != -1)
                {
                    TempData["SuccessMessage"] = $"Successfully borrowed the book: {book.Title}.";
                    return RedirectToAction("Index", "Books");
                }
                TempData["ErrorMessage"] = $"The book not exist";
                return View("NotAvailable");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while processing the borrowing action.";
                return View("Error");
            }
        }
        // Displays the return confirmation for a specific borrow record
        // GET: Borrow/Return/5
        public async Task<IActionResult> Return(int? borrowRecordId)
        {
            if (borrowRecordId == null || borrowRecordId == 0)
            {
                TempData["ErrorMessage"] = "Borrow Record ID was not provided for returning.";
                return View("NotFound");
            }
            try
            {
                var borrowRecord = _unitofWorkService.BorrowBookService.GetBorrowRecordWithRelated(borrowRecordId.Value, "Book");
                if (borrowRecord == null)
                {
                    TempData["ErrorMessage"] = $"No borrow record found with ID {borrowRecordId} to return.";
                    return View("NotFound");
                }
                if (borrowRecord.ReturnDate is not null)
                {
                    TempData["ErrorMessage"] = $"The borrow record for '{borrowRecord.Book.Title}' has already been returned.";
                    return View("AlreadyReturned");
                }
                var returnViewModel = _unitofWorkService.BorrowBookService.GetBorrowViewModel(borrowRecord);
                return View(returnViewModel);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while loading the return confirmation.";
                return View("Error");
            }
        }
        // Processes the return action, updates the BorrowRecord with the return date, updates the book's availability
        // POST: Borrow/Return/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Return(ReturnViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            try
            {
                var borrowRecord = _unitofWorkService.BorrowBookService.GetBorrowRecordWithRelated(model.BorrowRecordId, "Book");
                if (borrowRecord == null)
                {
                    TempData["ErrorMessage"] = $"No borrow record found with ID {model.BorrowRecordId} to return.";
                    return View("NotFound");
                }
                if (borrowRecord.ReturnDate != null)
                {
                    TempData["ErrorMessage"] = $"The borrow record for '{borrowRecord.Book.Title}' has already been returned.";
                    return View("AlreadyReturned");
                }
                // Update the borrow record
                borrowRecord.ReturnDate = DateTime.UtcNow;
                // Update the book's availability
                borrowRecord.Book.IsAvailable = true;
                var success = _unitofWorkService.BorrowBookService.Update(borrowRecord);
                if (success != -1)
                {
                    TempData["SuccessMessage"] = $"Successfully returned the book: {borrowRecord.Book.Title}.";
                    return RedirectToAction("Index", "Books");
                }
                TempData["ErrorMessage"] = $"The book not exist";
                return View("NotAvailable");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while processing the return action.";
                return View("Error");
            }
        }
    }
}
