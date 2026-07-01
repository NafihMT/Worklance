using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Worklance.Application.Interfaces.Services;

public interface IFileStorageService
{
    Task<string> SaveFileAsync(Stream fileStream, string fileName, string folderName, CancellationToken cancellationToken = default);
    void DeleteFile(string filePath);
    Task<Stream> GetFileStreamAsync(string relativeFilePath);
}
