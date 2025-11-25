namespace ULIP_proj.Interfaces
{
    public interface IFileService
    {
        Task<string> UploadFileAsync(IFormFile file, string folder);
        Task<byte[]> DownloadFileAsync(string filePath);
        Task<bool> DeleteFileAsync(string filePath);
        Task<string> UploadPolicyDocumentAsync(IFormFile file, int policyId);
        Task<byte[]> DownloadPolicyDocumentAsync(int policyId, string documentType);
    }
}
