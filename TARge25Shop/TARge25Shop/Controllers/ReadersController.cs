
using Microsoft.AspNetCore.Mvc;
using TARge25Shop.Core.Domain;
using TARge25Shop.Core.Dto;
using TARge25Shop.Core.ServiceInterface;

namespace TARge25Shop.Controllers
{
    public class ReadersController : Controller
    {
        private readonly IReaderServices _readerServices;

        public ReadersController(IReaderServices readerServices)
        {
            _readerServices = readerServices;
        }

        public async Task<IActionResult> Index()
        {
            var readers = await _readerServices.GetAll();
            return View(readers);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new ReaderDto());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ReaderDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.FullName) ||
                string.IsNullOrWhiteSpace(dto.Email))
            {
                ModelState.AddModelError("", "Please fill all fields.");
                return View(dto);
            }

            await _readerServices.Create(dto);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Details(Guid id)
        {
            try
            {
                var reader = await _readerServices.DetailAsync(id);
                return View(reader);
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
                var reader = await _readerServices.DetailAsync(id);

                var dto = new ReaderDto
                {
                    Id = reader.Id,
                    FullName = reader.FullName,
                    Email = reader.Email
                };

                return View(dto);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ReaderDto dto)
        {
            if (!dto.Id.HasValue ||
                string.IsNullOrWhiteSpace(dto.FullName) ||
                string.IsNullOrWhiteSpace(dto.Email))
            {
                ModelState.AddModelError("", "Please fill all fields.");
                return View(dto);
            }

            try
            {
                await _readerServices.Update(dto);
                return RedirectToAction(nameof(Index));
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpGet]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var reader = await _readerServices.DetailAsync(id);
                return View(reader);
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
                await _readerServices.Delete(id);
                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }
    }
}
