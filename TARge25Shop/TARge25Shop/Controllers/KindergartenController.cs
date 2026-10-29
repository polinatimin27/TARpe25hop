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
        // Vajalik andmebaasist andmete küsimiseks
        private readonly TARge25ShopContext _context;

        // Teenus lasteaia lisamiseks, muutmiseks ja kustutamiseks
        private readonly IKindergartenServices _kindergartenServices;

        // Teenus piltide salvestamiseks ja kustutamiseks
        private readonly IFileServices _fileServices;

        // Konstruktor saab vajalikud teenused
        public KindergartenController(
            TARge25ShopContext context,
            IKindergartenServices kindergartenServices,
            IFileServices fileServices)
        {
            // Salvestab need, et neid saaks selles klassis kasutada
            _context = context;
            _kindergartenServices = kindergartenServices;
            _fileServices = fileServices;
        }

        // GET avab lehe
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            // Võtab andmebaasist lasteaedade andmed
            // Select paneb iga kirje andmed ViewModeli sisse
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

            // Annab nimekirja vaatele, et seda lehel näidata
            return View(kindergartens);
        }

        [HttpGet]
        public IActionResult CreateUpdate()
        {
            // Avab tühja vormi uue kirje lisamiseks
            return View(new KindergartenCreateUpdateViewModel());
        }

        // POST võtab vastu vormist saadetud andmed
        [HttpPost]
        // Kontrollib vormiga saadetud turvakoodi
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateUpdate(
            KindergartenCreateUpdateViewModel viewModel)
        {
            // Kui andmed ei vasta nõuetele, näitab vormi uuesti
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            // Paneb vormi andmed DTO sisse, et anda need teenusele
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

            // Kui ID puudub, siis lisatakse uus kirje
            if (viewModel.Id == null)
            {
                var kindergarten =
                    await _kindergartenServices.Create(dto);

                // Kontrollib, kas failid ja loodud kirje ID on olemas
                if (viewModel.Files != null &&
                    viewModel.Files.Count > 0 &&
                    kindergarten.Id.HasValue)
                {
                    // Salvestab pildid ja seob need uue kirjega
                    await _fileServices.SaveKindergartenImagesAsync(
                        viewModel.Files,
                        kindergarten.Id.Value);
                }
            }
            else
            {
                // Kui ID on olemas, muudab olemasolevat kirjet
                await _kindergartenServices.Update(dto);

                // Kontrollib, kas vormiga saadeti uusi faile
                if (viewModel.Files != null &&
                    viewModel.Files.Count > 0)
                {
                    // Lisab uued pildid selle kirje juurde
                    await _fileServices.SaveKindergartenImagesAsync(
                        viewModel.Files,
                        viewModel.Id.Value);
                }
            }

            // Pärast salvestamist läheb tagasi nimekirja
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            // Otsib muudetava kirje ID järgi
            var kindergarten =
                await _kindergartenServices.DetailAsync(id);

            // Paneb olemasolevad andmed vormi jaoks ViewModeli sisse
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

            // Kasutab sama vormi, mida kasutatakse lisamisel
            return View("CreateUpdate", viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Details(Guid id)
        {
            // Otsib valitud kirje andmed
            var kindergarten =
                await _kindergartenServices.DetailAsync(id);

            // Where valib ainult selle lasteaia pildid
            // Select võtab iga pildi ID ja faili nime
            var images = await _context.FileToApis
                .Where(x => x.KindergartenId == id)
                .Select(x => new KindergartenImageViewModel
                {
                    Id = x.Id,
                    FilePath = x.ExistingFilePath
                })
                .ToListAsync();

            // Paneb kirje andmed ja pildid ühte ViewModeli
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

            // Näitab andmeid ja pilte Details lehel
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveImage(
            Guid id, Guid imageId)
        {
            // id on lasteaia ID ja imageId on pildi ID
            // Kontrollib ka, et pilt kuulub sellele lasteaiale
            var image = await _context.FileToApis
                .FirstOrDefaultAsync(x =>
                    x.Id == imageId &&
                    x.KindergartenId == id);

            // Kui pilti ei leitud, tagastab 404 vastuse
            if (image == null)
            {
                return NotFound();
            }

            // Annab kustutatava pildi ID DTO kaudu teenusele
            var dto = new FileToApiDto
            {
                Id = imageId
            };

            // Teenus kustutab pildi failist ja andmebaasist
            await _fileServices.RemoveImageFromApi(dto);

            // Läheb sama lasteaia Details lehele tagasi
            return RedirectToAction(nameof(Details),
                new { id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveAllImages(Guid id)
        {
            // Kontrollib, kas selle ID-ga lasteaia kirje on olemas
            var kindergarten = await _context.Kindergartens
                .FirstOrDefaultAsync(x => x.Id == id);

            if (kindergarten == null)
            {
                return NotFound();
            }

            // Kustutab kõik selle lasteaia pildid
            await _fileServices.RemoveKindergartenImagesAsync(id);

            // Läheb sama kirje Details lehele tagasi
            return RedirectToAction(nameof(Details),
                new { id });
        }

        [HttpGet]
        public async Task<IActionResult> Delete(Guid id)
        {
            // Otsib kirje, mida soovitakse kustutada
            var kindergarten =
                await _kindergartenServices.DetailAsync(id);

            // Paneb andmed ViewModeli sisse
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

            // Näitab kustutamise kinnitamise lehte
            // Siin kirjet veel ei kustutata
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmation(Guid id)
        {
            // Kõigepealt kustutab selle lasteaia pildid
            await _fileServices.RemoveKindergartenImagesAsync(id);

            // Siis kustutab lasteaia kirje
            await _kindergartenServices.Delete(id);

            // Läheb tagasi nimekirja
            return RedirectToAction(nameof(Index));
        }
    }
}