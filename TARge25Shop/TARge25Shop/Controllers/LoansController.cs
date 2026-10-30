
using Microsoft.AspNetCore.Mvc;
using TARge25Shop.Core.ServiceInterface;

namespace TARge25Shop.Controllers
{
    public class LoansController : Controller
    {
        private readonly ILoanServices _loanServices;
        private readonly IBookServices _bookServices;
        private readonly IReaderServices _readerServices;

        public LoansController(
            ILoanServices loanServices,
            IBookServices bookServices,
            IReaderServices readerServices)
        {
            _loanServices = loanServices;
            _bookServices = bookServices;
            _readerServices = readerServices;
        }

        public async Task<IActionResult> Index(Guid? readerId)
        {
            var loans = readerId.HasValue
                ? await _loanServices.GetByReader(readerId.Value)
                : await _loanServices.GetAll();

            ViewBag.Readers = await _readerServices.GetAll();

            return View(loans);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await LoadCreateData();
            return View();
        }

        private async Task LoadCreateData()
        {
            ViewBag.Books = (await _bookServices.GetAll())
                .Where(x => x.IsAvailable)
                .ToList();

            ViewBag.Readers = await _readerServices.GetAll();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            Guid bookId, Guid readerId)
        {
            try
            {
                await _loanServices.Create(bookId, readerId);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex) when (
                ex is InvalidOperationException ||
                ex is KeyNotFoundException)
            {
                ModelState.AddModelError("", ex.Message);
                await LoadCreateData();
                return View();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Return(Guid id)
        {
            try
            {
                await _loanServices.ReturnBook(id);
            }
            catch (Exception ex) when (
                ex is InvalidOperationException ||
                ex is KeyNotFoundException)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Details(Guid id)
        {
            try
            {
                return View(await _loanServices.DetailAsync(id));
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
                return View(await _loanServices.DetailAsync(id));
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            Guid id, DateTime loanDate)
        {
            try
            {
                await _loanServices.UpdateLoanDate(id, loanDate);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex) when (
                ex is InvalidOperationException ||
                ex is KeyNotFoundException)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpGet]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                return View(await _loanServices.DetailAsync(id));
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
                await _loanServices.DeleteReturnedLoan(id);
            }
            catch (Exception ex) when (
                ex is InvalidOperationException ||
                ex is KeyNotFoundException)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
