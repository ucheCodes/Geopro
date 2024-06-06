using Microsoft.AspNetCore.Components.Forms;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Formats.Gif;
using SixLabors.ImageSharp.Metadata;

namespace PenPro.Data;

class UploadImage
{
    private IWebHostEnvironment? _environment { get; set; }
    private long maxFileSize = 1024 * 1024 * 10;
    public UploadImage(IWebHostEnvironment environment)
    {
        _environment = environment;
    }
    public async Task<string> CaptureFile(IBrowserFile file)
    {
        if (file == null)
        {
            return "";
        }
        string path = "";
        try
        {
            if (_environment != null)
            {
                var uploadPath = Path.Combine(_environment.WebRootPath, "Upload");
                if (!Directory.Exists(uploadPath))
                {
                    Directory.CreateDirectory(uploadPath);
                }
                string newFilename = Path.ChangeExtension(Path.GetRandomFileName(), Path.GetExtension(file.Name));
                path = Path.Combine(uploadPath, newFilename);

                string fileExtension = Path.GetExtension(file.Name);
                string relativePath = Path.Combine("Upload", newFilename);
                await using FileStream fs = new(path, FileMode.Create);
                await file.OpenReadStream(maxFileSize).CopyToAsync(fs);
                return Path.Combine("./", relativePath);
            }
            return "";

        }
        catch (Exception ex)
        {
            return $"error: {ex.Message}";
        }

    }
    public async Task<string> ConvertImageToWebp(IBrowserFile file)
    {
        try
        {
            var imageStream = file?.OpenReadStream(file.Size);
            if (_environment != null && imageStream != null)
            {
                using (var image = await Image.LoadAsync(imageStream))
                {
                    var outputPath = Path.ChangeExtension("./Upload/" + Path.GetRandomFileName(), ".webp");
                    string absolutePath = Path.Combine(_environment.WebRootPath, outputPath);
                    // Ensure the directory exists
                    Directory.CreateDirectory(Path.GetDirectoryName(absolutePath) ?? "");
                    // Save the image as WebP
                    using (var outputStream = new FileStream(absolutePath, FileMode.Create))
                    {
                        image.SaveAsWebp(outputStream);
                    }
                    return outputPath;//serves as relativePath
                }
            }
        }
        catch (Exception ex)
        {
             return $"error: {ex.Message}";;
        }
        return string.Empty;
    }
    
}