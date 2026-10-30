
using TARge25Shop.Core.Domain;
using TARge25Shop.Core.Dto;

namespace TARge25Shop.Core.ServiceInterface
{
    public interface IReaderServices
    {
        Task<Reader> Create(ReaderDto dto);
        Task<List<Reader>> GetAll();
        Task<Reader> DetailAsync(Guid id);
        Task<Reader> Update(ReaderDto dto);
        Task<Reader> Delete(Guid id);
    }
}
