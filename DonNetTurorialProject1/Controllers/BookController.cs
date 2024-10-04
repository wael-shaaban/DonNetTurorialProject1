using DonNetTurorialProject1.Data.Services;
using DonNetTurorialProject1.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace DonNetTurorialProject1.Controllers
{
    public class BookController(IBookService bookService) : Controller
    {

        // Retrieves and displays all books.
        // GET: Books
        public async Task<IActionResult> Index()
        {
            try
            {
                var books = bookService.GetAllWithRelatedNavigationProperites("BorrowRecords", true);
                return View(books);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while loading the books.";
                return View("Error");
            }
        }

        // GET: Books/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || id == 0)
            {
                TempData["ErrorMessage"] = "Book ID was not provided.";
                return View("NotFound");
            }
            try
            {
                var book = bookService.GetByID(id.Value);
                if (book is null)
                {
                    TempData["ErrorMessage"] = $"No book found with ID {id}.";
                    return View("NotFound");
                }
                return View(book);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while loading the book details.";
                return View("Error");
            }
        }

        // GET: Books/Create
        public IActionResult Create()
        {
            return View();
        }
        // POST: Books/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Book book)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // BookId and IsAvailable are not bound due to [BindNever]
                    var succes = bookService.Add(book);
                    if (succes != -1)
                    {
                        TempData["SuccessMessage"] = $"Successfully added the book: {book.Title}.";
                        return RedirectToAction(nameof(Index));
                    }
                    //ModelState.AddModelError("", "Books Not Saved Successfully! try again");
                    TempData["ErrorMessage"] = "Books Not Saved Successfully! try again";
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = "An error occurred while adding the book.";
                    return View(book);
                }
            }
            return View(book);
        }
        // GET: Books/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || id == 0)
            {
                TempData["ErrorMessage"] = "Book ID was not provided for editing.";
                return View("NotFound");
            }
            try
            {
                var book = bookService.GetByID(id.Value);
                if (book == null)
                {
                    TempData["ErrorMessage"] = $"No book found with ID {id} for editing.";
                    return View("NotFound");
                }
                return View(book);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while loading the book for editing.";
                return View("Error");
            }
        }
        // POST: Books/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id, Book book)
        {
            if (id == null || id == 0)
            {
                TempData["ErrorMessage"] = "Book ID was not provided for updating.";
                return View("NotFound");
            }
            if (ModelState.IsValid)
            {
                try
                {
                    var existingBook = bookService.GetByID(id.Value);
                    if (existingBook == null)
                    {
                        TempData["ErrorMessage"] = $"No book found with ID {id} for updating.";
                        return View("NotFound");
                    }
                    // Updating fields that can be edited
                    var success = bookService.Edit(book);
                    if (success != -1)
                    {
                        TempData["SuccessMessage"] = $"Successfully updated the book: {book.Title}.";
                        return RedirectToAction(nameof(Index));
                    }
                    TempData["ErrorMessage"] = "Books Not Updated Successfully! try again";
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    if (!BookExists(book.BookId))
                    {
                        TempData["ErrorMessage"] = $"No book found with ID {book.BookId} during concurrency check.";
                        return View("NotFound");
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "A concurrency error occurred during the update.";
                        return View("Error");
                    }
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = "An error occurred while updating the book.";
                    return View("Error");
                }
            }
            return View(book);
        }
        
        // GET: Books/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || id == 0)
            {
                TempData["ErrorMessage"] = "Book ID was not provided for deletion.";
                return View("NotFound");
            }
            try
            {
                var book = bookService.GetByID(id.Value);
                if (book == null)
                {
                    TempData["ErrorMessage"] = $"No book found with ID {id} for deletion.";
                    return View("NotFound");
                }
                return View(book);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while loading the book for deletion.";
                return View("Error");
            }
        }
        
        // POST: Books/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var book1 = bookService.GetByID(id);
                var book = bookService.Delete(id);
                if (book !=-1)
                {
                    TempData["ErrorMessage"] = $"No book found with ID {id} for deletion.";
                    return View("NotFound");
                }
                TempData["SuccessMessage"] = $"Successfully deleted the book: {book1.Title}.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while deleting the book.";
                return View("Error");
            }
        }
        private bool BookExists(int id) => bookService.GetByID(id) is not null;
    }
}