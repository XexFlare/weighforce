using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace WeighForce.Services
{
  class FileUploadService
  {
    public static async Task<string> WriteFile(IFormFile file, string destinationPath, bool relative = false)
    {
      string fileName;
      try
      {
        var extension = "." + file.FileName.Split('.')[file.FileName.Split('.').Length - 1];
        fileName = DateTime.UtcNow.Ticks + extension;

        var pathBuilt = Path.Combine(Directory.GetCurrentDirectory(), destinationPath);

        if (!Directory.Exists(pathBuilt))
        {
          Directory.CreateDirectory(pathBuilt);
        }

        var path = Path.Combine(Directory.GetCurrentDirectory(), destinationPath,
           fileName);

        using (var stream = new FileStream(path, FileMode.Create))
        {
          await file.CopyToAsync(stream);
        }
        return relative ? fileName : path;
      }
      catch (Exception)
      {
        //log error
        return null;
      }
    }
  }
}