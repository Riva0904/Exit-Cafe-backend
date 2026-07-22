using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using ExitCafe.Application.Common.Interfaces;
using Microsoft.Extensions.Configuration;

namespace ExitCafe.Infrastructure.Services;

/// <summary>
/// Render's free-tier disk is ephemeral — it wipes on every deploy and on every idle spin-down/wake
/// cycle (roughly every 15 minutes of inactivity), which silently deleted uploaded product images
/// repeatedly. Cloudinary's free tier stores files independently of the app container, so uploads
/// survive both.
/// </summary>
public class CloudinaryFileStorageService : IFileStorageService
{
    private readonly Cloudinary _cloudinary;

    public CloudinaryFileStorageService(IConfiguration configuration)
    {
        var cloudName = configuration["Cloudinary:CloudName"] ?? throw new InvalidOperationException("Cloudinary:CloudName is not configured.");
        var apiKey = configuration["Cloudinary:ApiKey"] ?? throw new InvalidOperationException("Cloudinary:ApiKey is not configured.");
        var apiSecret = configuration["Cloudinary:ApiSecret"] ?? throw new InvalidOperationException("Cloudinary:ApiSecret is not configured.");

        _cloudinary = new Cloudinary(new Account(cloudName, apiKey, apiSecret)) { Api = { Secure = true } };
    }

    public async Task<string> SaveAsync(Stream content, string fileName, string subfolder, CancellationToken ct = default)
    {
        if (subfolder.Contains("..") || subfolder.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
            throw new ArgumentException("Invalid upload subfolder.", nameof(subfolder));

        var uploadParams = new ImageUploadParams
        {
            File = new FileDescription(fileName, content),
            Folder = $"exitcaff/{subfolder}",
            UseFilename = false,
            UniqueFilename = true,
            Overwrite = false,
        };

        var result = await _cloudinary.UploadAsync(uploadParams, ct);

        if (result.Error != null)
            throw new InvalidOperationException($"Cloudinary upload failed: {result.Error.Message}");

        return result.SecureUrl.ToString();
    }
}
