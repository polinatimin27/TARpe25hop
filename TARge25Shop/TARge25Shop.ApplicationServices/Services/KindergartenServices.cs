using Microsoft.EntityFrameworkCore;
using TARge25Shop.Core.Domain;
using TARge25Shop.Core.Dto;
using TARge25Shop.Core.ServiceInterface;
using TARge25Shop.Data;

namespace TARge25Shop.ApplicationServices.Services
{
    public class KindergartenServices : IKindergartenServices
    {
        // _context on vajalik andmebaasiga töötamiseks
        private readonly TARge25ShopContext _context;

        // Konstruktor saab contexti ja salvestab selle
        public KindergartenServices(TARge25ShopContext context)
        {
            _context = context;
        }

        // Loob uue lasteaia kirje
        // dto sisaldab vormist saadud andmeid
        public async Task<Kindergarten> Create(KindergartenDto dto)
        {
            // Teeb uue objekti ja paneb vormi andmed selle sisse
            var kindergarten = new Kindergarten
            {
                Id = Guid.NewGuid(), // Loob uue unikaalse ID
                GroupName = dto.GroupName,
                ChildrenCount = dto.ChildrenCount,
                KindergartenName = dto.KindergartenName,
                TeacherName = dto.TeacherName,
                CreatedAt = DateTime.Now, // Loomise aeg
                UpdatedAt = DateTime.Now  // Viimase muutmise aeg
            };

            // Lisab uue kirje ja salvestab selle andmebaasis
            await _context.Kindergartens.AddAsync(kindergarten);
            await _context.SaveChangesAsync();

            // Tagastab loodud kirje
            return kindergarten;
        }

        // Otsib ühe kirje ID järgi
        public async Task<Kindergarten> DetailAsync(Guid id)
        {
            // Võtab esimese kirje, mille ID on sama
            // Kui kirjet ei leia, on tulemus null
            var kindergarten = await _context.Kindergartens
                .FirstOrDefaultAsync(x => x.Id == id);

            // Kui kirjet ei ole, annab veateate
            if (kindergarten == null)
            {
                throw new KeyNotFoundException("Kindergarten not found.");
            }

            // Tagastab leitud kirje
            return kindergarten;
        }

        // Muudab olemasoleva kirje andmeid
        public async Task<Kindergarten> Update(KindergartenDto dto)
        {
            // Kontrollib, kas ID on olemas
            // Ilma ID-ta ei tea, millist kirjet muuta
            if (!dto.Id.HasValue)
            {
                throw new ArgumentException("Kindergarten ID is required.");
            }

            // Value võtab ID väärtuse
            // DetailAsync otsib selle ID-ga kirje
            var kindergarten = await DetailAsync(dto.Id.Value);

            // Paneb vanade andmete asemele vormist saadud andmed
            kindergarten.GroupName = dto.GroupName;
            kindergarten.ChildrenCount = dto.ChildrenCount;
            kindergarten.KindergartenName = dto.KindergartenName;
            kindergarten.TeacherName = dto.TeacherName;

            // Uuendab muutmise aega, loomise aeg jääb samaks
            kindergarten.UpdatedAt = DateTime.Now;

            // Salvestab muudatused andmebaasis
            await _context.SaveChangesAsync();

            return kindergarten;
        }

        // Kustutab kirje ID järgi
        public async Task<Kindergarten> Delete(Guid id)
        {
            // Otsib kirje sama DetailAsync meetodiga
            var kindergarten = await DetailAsync(id);

            // Eemaldab kirje ja salvestab muudatuse andmebaasis
            _context.Kindergartens.Remove(kindergarten);
            await _context.SaveChangesAsync();

            // Tagastab kustutatud kirje andmed
            return kindergarten;
        }
    }
}