using System.IO;
using System.Threading.Tasks;

namespace Worklance.Application.Interfaces.CloudinaryInterface
{
    public interface ICloudinaryService
    {
        Task<string> UploadImageAsync(Stream stream, string fileName);
        Task<string> UploadFileAsync(Stream stream, string fileName, string folder);
    }
}
