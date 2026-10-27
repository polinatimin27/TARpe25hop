
using TARge25Shop.Core.Domain;
using TARge25Shop.Core.Dto;

namespace TARge25Shop.Core.ServiceInterface
{
    public interface IBookServices
    {
        Task<Book> Create(BookDto dto);
        Task<Book> DetailAsync(Guid id);
        Task<Book> Update(BookDto dto);
        Task<Book> Delete(Guid id);

        Task<List<Book>> GetAll();
        Task<List<Book>> GetByYear(int year);
        Task<Book?> GetByTitle(string title);
        Task<List<Book>> GetBorrowedBooks();
    }
}
