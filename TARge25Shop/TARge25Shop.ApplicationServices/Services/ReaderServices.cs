
using Microsoft.EntityFrameworkCore;
using TARge25Shop.Core.Domain;
using TARge25Shop.Core.Dto;
using TARge25Shop.Core.ServiceInterface;
using TARge25Shop.Data;

namespace TARge25Shop.ApplicationServices.Services
{
    public class ReaderServices : IReaderServices
    {
        private readonly TARge25ShopContext _context;

        public ReaderServices(TARge25ShopContext context)
        {
            _context = context;
        }

        public async Task<Reader> Create(ReaderDto dto)
        {
            var reader = new Reader
            {
                Id = Guid.NewGuid(),
                FullName = dto.FullName,
                Email = dto.Email
            };

            await _context.Readers.AddAsync(reader);
            await _context.SaveChangesAsync();

            return reader;
        }

        public async Task<List<Reader>> GetAll()
        {
            return await _context.Readers.ToListAsync();
        }

        public async Task<Reader> DetailAsync(Guid id)
        {
            var reader = await _context.Readers
                .FirstOrDefaultAsync(x => x.Id == id);

            if (reader == null)
            {
                throw new KeyNotFoundException("Reader not found.");
            }

            return reader;
        }

        public async Task<Reader> Update(ReaderDto dto)
        {
            if (!dto.Id.HasValue)
            {
                throw new ArgumentException("Reader ID is required.");
            }

            var reader = await DetailAsync(dto.Id.Value);

            reader.FullName = dto.FullName;
            reader.Email = dto.Email;

            await _context.SaveChangesAsync();

            return reader;
        }

        public async Task<Reader> Delete(Guid id)
        {
            var reader = await DetailAsync(id);

            bool hasLoans = await _context.Loans
                .AnyAsync(x => x.ReaderId == id);

            if (hasLoans)
            {
                throw new InvalidOperationException(
                    "Reader has loan history and cannot be deleted.");
            }

            _context.Readers.Remove(reader);
            await _context.SaveChangesAsync();

            return reader;
        }
    }
}
