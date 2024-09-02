namespace Elderly_Canteen.Services.Interfaces
{
    public interface IOssService
    {
        Task<string> UploadFileAsync(IFormFile file, string fileName);
        string GetDefaultImageUrl();
        string GetDefaultProtrateUrl();
    }
}
