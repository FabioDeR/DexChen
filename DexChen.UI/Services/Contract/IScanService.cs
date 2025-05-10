using DexChen.Domain.Dtos;
using Microsoft.AspNetCore.Components.Forms;

namespace DexChen.UI.Services.Contract
{
    public interface IScanService
    {
        Task<ScanResult?> AnalyzeImageFromBase64Async(string base64Image);
        Task<ScanResult?> AnalyzeImageFromBytesAsync(byte[] bytes, string fileName, string contentType);
        Task<ScanResult?> AnalyzeUploadedFileAsync(InputFileChangeEventArgs e);
    }
}
