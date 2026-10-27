
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
    }
}
