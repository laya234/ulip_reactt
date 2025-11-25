using ULIP_proj.Interfaces;

namespace ULIP_proj.Services
{
    public class FileService : IFileService
    {
        private readonly IWebHostEnvironment _environment;

        public FileService(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        public async Task<string> UploadFileAsync(IFormFile file, string folder)
        {
            if (file == null || file.Length == 0) return null;

            var rootPath = _environment.WebRootPath ?? _environment.ContentRootPath;
            var uploadsFolder = Path.Combine(rootPath, "uploads", folder);
            Directory.CreateDirectory(uploadsFolder);

            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var filePath = Path.Combine(uploadsFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return Path.Combine("uploads", folder, fileName);
        }

        public async Task<byte[]> DownloadFileAsync(string filePath)
        {
            var rootPath = _environment.WebRootPath ?? _environment.ContentRootPath;
            var fullPath = Path.Combine(rootPath, filePath);
            if (!File.Exists(fullPath)) return null;

            return await File.ReadAllBytesAsync(fullPath);
        }

        public async Task<bool> DeleteFileAsync(string filePath)
        {
            var rootPath = _environment.WebRootPath ?? _environment.ContentRootPath;
            var fullPath = Path.Combine(rootPath, filePath);
            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
                return true;
            }
            return false;
        }

        public async Task<string> UploadPolicyDocumentAsync(IFormFile file, int policyId)
        {
            return await UploadFileAsync(file, $"policies/{policyId}");
        }

        public async Task<byte[]> DownloadPolicyDocumentAsync(int policyId, string documentType)
        {
            var rootPath = _environment.WebRootPath ?? _environment.ContentRootPath;
            var folder = $"policies/{policyId}";
            var uploadsPath = Path.Combine(rootPath, "uploads", folder);
            
            if (!Directory.Exists(uploadsPath)) return null;
            
            var files = Directory.GetFiles(uploadsPath);
            var file = files.FirstOrDefault(f => f.Contains(documentType));
            
            if (file != null)
            {
                return await File.ReadAllBytesAsync(file);
            }
            return null;
        }
    }
}
