
using Microsoft.AspNetCore.Http;
using TARge25Shop.Core.Domain;
using TARge25Shop.Core.Dto;

namespace TARge25Shop.Core.ServiceInterface
{
    public interface IFileServices
    {
        void FilesToApi(SpaceshipDto dto, Spaceship domain);

        Task<FileToApi> RemoveImageFromApi(FileToApiDto dto);

        Task<List<FileToApi>> RemoveImagesFromApi(FileToApiDto[] dtos);

        Task SaveKindergartenImagesAsync(
            IEnumerable<IFormFile> files,
            Guid kindergartenId);

        Task RemoveKindergartenImagesAsync(Guid kindergartenId);
    }
}
