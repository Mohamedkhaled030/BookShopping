namespace BookShopping.Infrustructure.Service.Abstruct
{
    public interface IFileService
    {
        void DeleteFile(string fileName);
        Task<string> SaveFile(IFormFile file, string[] allowedExtensions);
    }



}
