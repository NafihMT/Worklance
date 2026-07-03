using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Worklance.Application.Interfaces.Services;

namespace Worklance.Infrastructure.Services;

public class LocalFileStorageService : IFileStorageService
{
    public async Task<string> SaveFileAsync(Stream fileStream, string fileName, string folderName, CancellationToken cancellationToken = default)
    {
        var webRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");

        var targetFolder = Path.Combine(webRootPath, folderName);
        if (!Directory.Exists(targetFolder))
        {
            Directory.CreateDirectory(targetFolder);
        }

        var uniqueFileName = $"{Guid.NewGuid()}_{fileName}";
        var filePath = Path.Combine(targetFolder, uniqueFileName);

        using (var destinationStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
        {
            await fileStream.CopyToAsync(destinationStream, cancellationToken);
        }

        return $"/{folderName}/{uniqueFileName}";
    }

    public void DeleteFile(string relativeFilePath)
    {
        if (string.IsNullOrEmpty(relativeFilePath)) return;

        var webRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
        var normalizedPath = relativeFilePath.TrimStart('/');
        var fullPath = Path.Combine(webRootPath, normalizedPath);

        if (File.Exists(fullPath))
        {
            File.Delete(fullPath);
        }
    }

    public Task<Stream> GetFileStreamAsync(string relativeFilePath)
    {
        if (string.IsNullOrEmpty(relativeFilePath))
        {
            throw new FileNotFoundException("File path is empty.");
        }

        var webRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
        var normalizedPath = relativeFilePath.TrimStart('/');
        var filePath = Path.Combine(webRootPath, normalizedPath);

        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException("File not found on disk.");
        }

        return Task.FromResult<Stream>(new FileStream(filePath, FileMode.Open, FileAccess.Read));
    }
}
