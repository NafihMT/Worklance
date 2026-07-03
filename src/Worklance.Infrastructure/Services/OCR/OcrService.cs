using System;
using System.IO;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Worklance.Application.Interfaces.AuthInterface;

namespace Worklance.Infrastructure.Services.OCR
{
    public class OcrService : IOcrService
    {
        public Task<string> ReadAadhaarNumberAsync(string imagePath)
        {
            // Tesseract temporarily removed to fix native memory crashes
            return Task.FromResult(string.Empty);
        }
    }
}
