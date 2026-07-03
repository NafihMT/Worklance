using System;
using System.IO;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Worklance.Application.Interfaces.AuthInterface;
using Tesseract;

namespace Worklance.Infrastructure.Services.OCR
{
    public class OcrService : IOcrService
    {
        private static readonly HttpClient _httpClient = new HttpClient();

        public async Task<string> ReadAadhaarNumberAsync(string imagePath)
        {
            byte[] imageBytes;

            if (imagePath.StartsWith("http://", StringComparison.OrdinalIgnoreCase) || 
                imagePath.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
            {
                try
                {
                    imageBytes = await _httpClient.GetByteArrayAsync(imagePath);
                }
                catch (Exception ex)
                {
                    throw new Exception($"Failed to download image from URL: {imagePath}. Error: {ex.Message}", ex);
                }
            }
            else
            {
                try
                {
                    imageBytes = await File.ReadAllBytesAsync(imagePath);
                }
                catch (Exception ex)
                {
                    throw new Exception($"Failed to read local image file: {imagePath}. Error: {ex.Message}", ex);
                }
            }

            return await Task.Run(() =>
            {
                try
                {
                    // Use "./tessdata" or check for fallback folder
                    var tessDataPath = "./tessdata";
                    if (!Directory.Exists(tessDataPath))
                    {
                        // Fallback to AppDomain.CurrentDomain.BaseDirectory tessdata if local doesn't exist
                        tessDataPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "tessdata");
                    }

                    using var engine = new TesseractEngine(tessDataPath, "eng", EngineMode.Default);
                    using var img = Pix.LoadFromMemory(imageBytes);
                    using var page = engine.Process(img);
                    var text = page.GetText();

                    var match = Regex.Match(text, @"\d{4}\s?\d{4}\s?\d{4}");
                    if (match.Success)
                    {
                        return match.Value.Replace(" ", "").Replace("\n", "").Replace("\r", "").Trim();
                    }

                    return string.Empty;
                }
                catch (Exception ex)
                {
                    throw new Exception($"Tesseract OCR processing failed. Error: {ex.Message}", ex);
                }
            });
        }
    }
}
