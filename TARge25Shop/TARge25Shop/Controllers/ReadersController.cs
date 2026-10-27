
using Microsoft.AspNetCore.Mvc;
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
    }
}
