
using TARge25Shop.Core.Domain;

namespace TARge25Shop.Core.ServiceInterface
{
    public interface ILoanServices
    {
        Task<Loan> Create(Guid bookId, Guid readerId);
        Task<Loan> ReturnBook(Guid loanId);
        Task<List<Loan>> GetAll();
        Task<List<Loan>> GetByReader(Guid readerId);
    }
}
