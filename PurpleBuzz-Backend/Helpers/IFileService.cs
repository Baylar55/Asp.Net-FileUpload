namespace PurpleBuzz_Backend.Helpers
{
    public interface IFileService
    {
        void Delete(string fileName, string webRootPath);
        Task<string> UploadAsync(IFormFile file, string webRootPath);
        bool IsImage(IFormFile formFile);
        bool checkSize(IFormFile formFile, int maxSize);
    }
}
