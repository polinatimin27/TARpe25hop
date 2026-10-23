
namespace TARge25Shop.Core.Domain
{
    public class FileToApi
    {
        public Guid Id { get; set; }
        public string? ExistingFilePath { get; set; }
        public Guid? SpaceshipId { get; set; }
        public Guid? KindergartenId { get; set; }
    }
}
