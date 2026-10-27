
using Microsoft.EntityFrameworkCore;
using TARge25Shop.Core.Domain;
using TARge25Shop.Core.Dto;
using TARge25Shop.Core.ServiceInterface;
using TARge25Shop.Data;

namespace TARge25Shop.ApplicationServices.Services
{
    public class BookServices : IBookServices
    {
        private readonly TARge25ShopContext _context;

        public BookServices(TARge25ShopContext context)
        {
            _context = context;
        }

        public async Task<Book> Create(BookDto dto)
        {
            var book = new Book
            {
                Id = Guid.NewGuid(),
                Title = dto.Title,
                Author = dto.Author,
                PublishedYear = dto.PublishedYear,
                IsAvailable = true
            };

            await _context.Books.AddAsync(book);
            await _context.SaveChangesAsync();

            return book;
        }

        public async Task<List<Book>> GetAll()
        {
            return await _context.Books.ToListAsync();
        }

        public async Task<List<Book>> GetByYear(int year)
        {
            return await _context.Books
                .Where(x => x.PublishedYear == year)
                .ToListAsync();
        }

        public async Task<Book?> GetByTitle(string title)
        {
            return await _context.Books
                .FirstOrDefaultAsync(x => x.Title == title);
        }

        public async Task<List<Book>> GetBorrowedBooks()
        {
            return await _context.Books
                .Where(x => x.IsAvailable == false)
                .ToListAsync();
        }

        public async Task<Book> DetailAsync(Guid id)
        {
            var book = await _context.Books
                .FirstOrDefaultAsync(x => x.Id == id);

            if (book == null)
            {
                throw new KeyNotFoundException("Book not found.");
            }

            return book;
        }

        public async Task<Book> Update(BookDto dto)
        {
            if (!dto.Id.HasValue)
            {
                throw new ArgumentException("Book ID is required.");
            }

            var book = await DetailAsync(dto.Id.Value);

            book.Title = dto.Title;
            book.Author = dto.Author;
            book.PublishedYear = dto.PublishedYear;

            await _context.SaveChangesAsync();

            return book;
        }

        public async Task<Book> Delete(Guid id)
        {
            var book = await DetailAsync(id);

            var hasLoans = await _context.Loans
                .AnyAsync(x => x.BookId == id);

            if (hasLoans)
            {
                throw new InvalidOperationException(
                    "Book has loan history and cannot be deleted.");
            }

            _context.Books.Remove(book);
            await _context.SaveChangesAsync();

            return book;
        }
    }
}
