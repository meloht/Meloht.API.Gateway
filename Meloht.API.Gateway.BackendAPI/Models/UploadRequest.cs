namespace Meloht.API.Gateway.BackendAPI.Models
{
    public class UploadRequest
    {
        public string UserId { get; set; } = "";

        public string Remark { get; set; } = "";

        public IFormFile File { get; set; } = null!;
    }
}
