using Microsoft.AspNetCore.Http; // Vajalik IFormFile jaoks
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using TARge25Shop.Core.Domain;
using TARge25Shop.Core.Dto;
using TARge25Shop.Core.ServiceInterface;
using TARge25Shop.Data;

namespace TARge25Shop.ApplicationServices.Services
{
    public class FileServices : IFileServices
    {
        // _webHost aitab leida failide kausta
        private readonly IHostEnvironment _webHost;

        // _context on vajalik andmebaasiga töötamiseks
        private readonly TARge25ShopContext _context;

        // Konstruktor saab webHosti ja contexti
        public FileServices
            (
                IHostEnvironment webHost,
                TARge25ShopContext context
            )
        {
            // Salvestab need, et neid saaks selles klassis kasutada
            _webHost = webHost;
            _context = context;
        }

        public void FilesToApi(SpaceshipDto dto, Spaceship domain)
        {
            if (dto.Files != null && dto.Files.Count > 0)
            {
                //kui Directoryt ei ole olemas, siis tee Directory
                // \\wwwroot\\multipleFileUpload\\
                //tuleb kasutada webHosti
                if (!Directory.Exists(_webHost.ContentRootPath + "\\wwwroot\\multipleFileUpload\\"))
                {
                    Directory.CreateDirectory(_webHost.ContentRootPath + "\\wwwroot\\multipleFileUpload\\");
                }

                foreach (var file in dto.Files)
                {
                    //tuleb teha muutuja, kus on failide asukoht e kuhu hakatakse salvestama
                    string uploadsFolder = Path.Combine(_webHost.ContentRootPath, "wwwroot", "multipleFileUpload");
                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;

                    //tuleb kaks ülevalpool olevat muutujat kombineerida üheks
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        file.CopyTo(fileStream);

                        //tuleb Domaini teha class FileToApi,
                        //kus on muutujad Id, ExistingFilePath ja SpaceshipId
                        FileToApi path = new FileToApi
                        {
                            Id = Guid.NewGuid(),
                            ExistingFilePath = uniqueFileName,
                            SpaceshipId = domain.Id
                        };

                        //tuleb lisada context construktorisse
                        _context.FileToApis.AddAsync(path);
                    }
                }
            }
        }

        public async Task<FileToApi> RemoveImageFromApi(FileToApiDto dto)
        {
            //kui soovin kustutada faili, siis pean läbi Id pildi ülesse otsima
            var imageId = await _context.FileToApis
                .FirstOrDefaultAsync(x => x.Id == dto.Id);

            //teha muutuja filePath, mis näitab failide asukohta
            var filePath = _webHost.ContentRootPath + "\\wwwroot\\multipleFileUpload\\"
                + imageId.ExistingFilePath;

            //kui fail asub selles kaustas, siis kustuta
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            _context.FileToApis.Remove(imageId);
            await _context.SaveChangesAsync();

            // Pärast kustutamist tagastab meetod nulli
            return null;
        }

        //<List<FileToApi>> lisati sellepärast, et faile on mitu, mida kustutada
        public async Task<List<FileToApi>> RemoveImagesFromApi(FileToApiDto[] dtos)
        {
            // dtos sisaldab pilte, mida soovin kustutada
            // foreach võtab need ükshaaval läbi
            foreach (var dto in dtos)
            {
                //kui soovin kustutada faili, siis pean läbi Id pildi ülesse otsima
                var imageId = await _context.FileToApis
                    .FirstOrDefaultAsync(x => x.Id == dto.Id);

                //teha muutuja filePath, mis näitab failide asukohta
                var filePath = _webHost.ContentRootPath + "\\wwwroot\\multipleFileUpload\\"
                    + imageId.ExistingFilePath;

                //kui fail asub selles kaustas, siis kustuta
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }

                _context.FileToApis.Remove(imageId);
                await _context.SaveChangesAsync();
            }

            return null;
        }

        // Kindergarten piltide lisamine
        public async Task SaveKindergartenImagesAsync(
            IEnumerable<IFormFile> files,
            Guid kindergartenId)
        {
            // files on üles laaditud failid
            // kindergartenId näitab, millisele lasteaiale pildid kuuluvad

            // Siin on kaust, kuhu pildid salvestatakse
            string uploadsFolder = Path.Combine(
                _webHost.ContentRootPath,
                "wwwroot",
                "multipleFileUpload");

            // Kui kausta ei ole, siis loob selle
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            // Käib kõik failid ükshaaval läbi
            foreach (var file in files)
            {
                // Kui fail on tühi, jätab selle vahele
                if (file.Length == 0)
                {
                    continue;
                }

                // Võtab faili laiendi, näiteks .jpg või .png
                // Muudab laiendi väikesteks tähtedeks
                string extension = Path.GetExtension(file.FileName)
                    .ToLowerInvariant();

                // Kontrollib, kas faili laiend on lubatud
                // Kui ei ole, jätab faili vahele
                if (extension != ".jpg" &&
                    extension != ".jpeg" &&
                    extension != ".png" &&
                    extension != ".gif" &&
                    extension != ".webp")
                {
                    continue;
                }

                // Kui fail on suurem kui 5 MB, jätab selle vahele
                if (file.Length > 5 * 1024 * 1024)
                {
                    continue;
                }

                // Loob failile unikaalse nime ja lisab laiendi
                string uniqueFileName = Guid.NewGuid().ToString()
                    + extension;

                // Ühendab kausta asukoha ja faili nime
                string filePath = Path.Combine(
                    uploadsFolder,
                    uniqueFileName);

                // Salvestab faili kausta
                // await ootab, kuni salvestamine on valmis
                using (var fileStream = new FileStream(
                    filePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }

                // Loob pildi andmed andmebaasi jaoks
                FileToApi path = new FileToApi
                {
                    Id = Guid.NewGuid(),                // Pildi ID
                    ExistingFilePath = uniqueFileName, // Faili nimi
                    KindergartenId = kindergartenId   // Lasteaia ID
                };

                // Lisab pildi andmed andmebaasi
                await _context.FileToApis.AddAsync(path);
            }

            // Salvestab muudatused andmebaasis
            await _context.SaveChangesAsync();
        }

        // Kindergarten kõikide piltide kustutamine
        public async Task RemoveKindergartenImagesAsync(
            Guid kindergartenId)
        {
            // Otsib kõik pildid, mis kuuluvad sellele lasteaiale
            var images = await _context.FileToApis
                .Where(x => x.KindergartenId == kindergartenId)
                .ToListAsync();

            // Käib leitud pildid ükshaaval läbi
            foreach (var image in images)
            {
                // Leiab pildi asukoha kaustas
                string filePath = Path.Combine(
                    _webHost.ContentRootPath,
                    "wwwroot",
                    "multipleFileUpload",
                    image.ExistingFilePath ?? "");

                // Kui fail on olemas, kustutab selle kaustast
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }

                // Eemaldab pildi andmed andmebaasist
                _context.FileToApis.Remove(image);
            }

            // Salvestab muudatused andmebaasis
            await _context.SaveChangesAsync();
        }
    }
}