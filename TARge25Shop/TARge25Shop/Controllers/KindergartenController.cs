
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TARge25Shop.Core.Dto;
using TARge25Shop.Core.ServiceInterface;
using TARge25Shop.Data;
using TARge25Shop.Models.Kindergarten;

namespace TARge25Shop.Controllers
{
    public class KindergartenController : Controller
    {
        private readonly TARge25ShopContext _context;
        private readonly IKindergartenServices _kindergartenServices;
        private readonly IFileServices _fileServices;

        public KindergartenController(
            TARge25ShopContext context,
            IKindergartenServices kindergartenServices,
            IFileServices fileServices)
        {
            _context = context;
            _kindergartenServices = kindergartenServices;
            _fileServices = fileServices;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var kindergartens = await _context.Kindergartens
                .Select(x => new KindergartenIndexViewModel
                {
                    Id = x.Id,
                    GroupName = x.GroupName,
                    ChildrenCount = x.ChildrenCount,
                    KindergartenName = x.KindergartenName,
                    TeacherName = x.TeacherName,
                    CreatedAt = x.CreatedAt,
                    UpdatedAt = x.UpdatedAt
                })
                .ToListAsync();

            return View(kindergartens);
        }

        [HttpGet]
        public IActionResult CreateUpdate()
        {
            return View(new KindergartenCreateUpdateViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateUpdate(
            KindergartenCreateUpdateViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            var dto = new KindergartenDto
            {
                Id = viewModel.Id,
                GroupName = viewModel.GroupName,
                ChildrenCount = viewModel.ChildrenCount,
                KindergartenName = viewModel.KindergartenName,
                TeacherName = viewModel.TeacherName,
                CreatedAt = viewModel.CreatedAt,
                UpdatedAt = viewModel.UpdatedAt
            };

            if (viewModel.Id == null)
            {
                var kindergarten =
                    await _kindergartenServices.Create(dto);

                if (viewModel.Files != null &&
                    viewModel.Files.Count > 0 &&
                    kindergarten.Id.HasValue)
                {
                    await _fileServices.SaveKindergartenImagesAsync(
                        viewModel.Files,
                        kindergarten.Id.Value);
                }
            }
            else
            {
                await _kindergartenServices.Update(dto);

                if (viewModel.Files != null &&
                    viewModel.Files.Count > 0)
                {
                    await _fileServices.SaveKindergartenImagesAsync(
                        viewModel.Files,
                        viewModel.Id.Value);
                }
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var kindergarten =
                await _kindergartenServices.DetailAsync(id);

            var viewModel = new KindergartenCreateUpdateViewModel
            {
                Id = kindergarten.Id,
                GroupName = kindergarten.GroupName,
                ChildrenCount = kindergarten.ChildrenCount,
                KindergartenName = kindergarten.KindergartenName,
                TeacherName = kindergarten.TeacherName,
                CreatedAt = kindergarten.CreatedAt,
                UpdatedAt = kindergarten.UpdatedAt
            };

            return View("CreateUpdate", viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Details(Guid id)
        {
            var kindergarten =
                await _kindergartenServices.DetailAsync(id);

              var images = await _context.FileToApis
                .Where(x => x.KindergartenId == id)
                .Select(x => new KindergartenImageViewModel
                {
                    Id = x.Id,
                    FilePath = x.ExistingFilePath
                })
                .ToListAsync();

            var viewModel = new KindergartenDetailsViewModel
            {
                Id = kindergarten.Id,
                GroupName = kindergarten.GroupName,
                ChildrenCount = kindergarten.ChildrenCount,
                KindergartenName = kindergarten.KindergartenName,
                TeacherName = kindergarten.TeacherName,
                CreatedAt = kindergarten.CreatedAt,
                UpdatedAt = kindergarten.UpdatedAt,
                Images = images
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveImage(
            Guid id, Guid imageId)
        {
            var image = await _context.FileToApis
                .FirstOrDefaultAsync(x =>
                    x.Id == imageId &&
                    x.KindergartenId == id);

            if (image == null)
            {
                return NotFound();
            }

            var dto = new FileToApiDto
            {
                Id = imageId
            };

            await _fileServices.RemoveImageFromApi(dto);

            return RedirectToAction(nameof(Details),
                new { id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveAllImages(Guid id)
        {
            var kindergarten = await _context.Kindergartens
                .FirstOrDefaultAsync(x => x.Id == id);

            if (kindergarten == null)
            {
                return NotFound();
            }

            await _fileServices.RemoveKindergartenImagesAsync(id);

            return RedirectToAction(nameof(Details),
                new { id });
        }

        [HttpGet]
        public async Task<IActionResult> Delete(Guid id)
        {
            var kindergarten =
                await _kindergartenServices.DetailAsync(id);

            var viewModel = new KindergartenDeleteViewModel
            {
                Id = kindergarten.Id,
                GroupName = kindergarten.GroupName,
                ChildrenCount = kindergarten.ChildrenCount,
                KindergartenName = kindergarten.KindergartenName,
                TeacherName = kindergarten.TeacherName,
                CreatedAt = kindergarten.CreatedAt,
                UpdatedAt = kindergarten.UpdatedAt
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmation(Guid id)
        {
            await _fileServices.RemoveKindergartenImagesAsync(id);
            await _kindergartenServices.Delete(id);

            return RedirectToAction(nameof(Index));
        }
    }
}
