
using Microsoft.EntityFrameworkCore;
using TARge25Shop.Core.Domain;
using TARge25Shop.Core.ServiceInterface;
using TARge25Shop.Data;

namespace TARge25Shop.ApplicationServices.Services
{
    public class LoanServices : ILoanServices
    {
        private readonly TARge25ShopContext _context;

        public LoanServices(TARge25ShopContext context)
        {
            _context = context;
        }

        public async Task<Loan> Create(Guid bookId, Guid readerId)
        {
            var book = await _context.Books.FindAsync(bookId);
            var reader = await _context.Readers.FindAsync(readerId);

            if (book == null || reader == null)
            {
                throw new KeyNotFoundException("Book or reader not found.");
            }

            if (!book.IsAvailable)
            {
                throw new InvalidOperationException("Book is not available.");
            }

            var loan = new Loan
            {
                Id = Guid.NewGuid(),
                BookId = bookId,
                ReaderId = readerId,
                LoanDate = DateTime.Now,
                ReturnDate = null
            };

            book.IsAvailable = false;

            await _context.Loans.AddAsync(loan);
            await _context.SaveChangesAsync();

            return loan;
        }

        public async Task<Loan> ReturnBook(Guid loanId)
        {
            var loan = await _context.Loans
                .Include(x => x.Book)
                .FirstOrDefaultAsync(x => x.Id == loanId);

            if (loan == null)
            {
                throw new KeyNotFoundException("Loan not found.");
            }

            if (loan.ReturnDate.HasValue)
            {
                throw new InvalidOperationException("Book already returned.");
            }

            loan.ReturnDate = DateTime.Now;
            loan.Book.IsAvailable = true;

            await _context.SaveChangesAsync();

            return loan;
        }

        public async Task<List<Loan>> GetAll()
        {
            return await _context.Loans
                .Include(x => x.Book)
                .Include(x => x.Reader)
                .ToListAsync();
        }

        public async Task<List<Loan>> GetByReader(Guid readerId)
        {
            return await _context.Loans
                .Include(x => x.Book)
                .Where(x => x.ReaderId == readerId)
                .ToListAsync();
        }
    }
}
