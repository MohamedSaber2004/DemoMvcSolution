using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.BusinessLogic.Services.AttachmentService
{
    public interface IAttachmentService
    {
        // Upload
        public string? Upload(IFormFile file, string destinationFolderName);

        // Delete 
        public bool Delete(string filePath);
    }
}
