using Castle.Core.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Demo.BusinessLogic.Services.AttachmentService
{
    public class AttachmentService(ILogger<AttachmentService> _logger) : IAttachmentService
    {
        List<string> allowedExtensions = [".svg", ".jpg", ".jpeg", ".png"];
        const int maxSize = 2_097_152;
        public string? Upload(IFormFile file, string FolderName)
        {
            // check Extension
            var extension = Path.GetExtension(file.FileName); // ex: .png
            if (!allowedExtensions.Contains(extension)) return null;

            // Check Size
            if (file.Length == 0 || file.Length > maxSize) return null;

            // Get Located Folder Path
            var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Files", FolderName);

            // Make Attachment Name Unique - use GUID
            var fileName = $"{Guid.NewGuid()}_{file.FileName}";

            // Get File Path
            var filePath = Path.Combine(folderPath, fileName); // File Location 

            // Create File Stream To Copy File
            using FileStream fs = new FileStream(filePath, FileMode.Create);

            // Use Stream To Copy File
            file.CopyTo(fs);

            // Return FileName To Store In Database
            return fileName;
        }

        public bool Delete(string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    Console.WriteLine("File does not exist: " + filePath);
                    return false;
                }

                File.Delete(filePath);
                Console.WriteLine("File deleted successfully: " + filePath);
                return true;
            }
            catch (UnauthorizedAccessException ex)
            {
                Console.WriteLine($"Permission issue: {ex.Message}");
                return false;
            }
            catch (IOException ex)
            {
                Console.WriteLine($"Error deleting file: {ex.Message}");
                return false;
            }
        }

    }
}
