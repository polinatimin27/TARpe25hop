
using Microsoft.AspNetCore.Mvc;
using TARge25Shop.Core.Dto;
using TARge25Shop.Core.ServiceInterface;

namespace TARge25Shop.Controllers
{
    public class BooksController : Controller
    {
        private readonly IBookServices _bookServices;

        public BooksController(IBookServices bookServices)
        {
            _bookServices = bookServices;
        }

        public async Task<IActionResult> Index(
            int? year, string? title, bool borrowed = false)
        {
            var books = borrowed
                ? await _bookServices.GetBorrowedBooks()
                : year.HasValue
                    ? await _bookServices.GetByYear(year.Value)
                    : !string.IsNullOrWhiteSpace(title)
                        ? (await _bookServices.GetAll())
                            .Where(x => x.Title.Contains(
                                title, StringComparison.OrdinalIgnoreCase))
                            .ToList()
                        : await _bookServices.GetAll();

            return View(books);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new BookDto());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BookDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Title) ||
                string.IsNullOrWhiteSpace(dto.Author) ||
                dto.PublishedYear <= 0)
            {
                ModelState.AddModelError("", "Please fill all fields.");
                return View(dto);
            }

            await _bookServices.Create(dto);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Details(Guid id)
        {
            try
            {
                var book = await _bookServices.DetailAsync(id);
                return View(book);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            try
            {
                var book = await _bookServices.DetailAsync(id);

                return View(new BookDto
                {
                    Id = book.Id,
                    Title = book.Title,
                    Author = book.Author,
                    PublishedYear = book.PublishedYear
                });
            }
            catch (KeyNotFoundException)
            {
                TempData["Error"] = "Book not found.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(BookDto dto)
        {
            if (!dto.Id.HasValue ||
                string.IsNullOrWhiteSpace(dto.Title) ||
                string.IsNullOrWhiteSpace(dto.Author) ||
                dto.PublishedYear <= 0)
            {
                ModelState.AddModelError("", "Please check the fields.");
                return View(dto);
            }

            try
            {
                await _bookServices.Update(dto);
                return RedirectToAction(nameof(Index));
            }
            catch (KeyNotFoundException)
            {
                TempData["Error"] = "Book not found.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpGet]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var book = await _bookServices.DetailAsync(id);
                return View(book);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            try
            {
                await _bookServices.Delete(id);
            }
            catch (InvalidOperationException)
            {
                TempData["Error"] =
                    "Book has loan history and cannot be deleted.";
            }
            catch (KeyNotFoundException)
            {
                TempData["Error"] = "Book not found.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
