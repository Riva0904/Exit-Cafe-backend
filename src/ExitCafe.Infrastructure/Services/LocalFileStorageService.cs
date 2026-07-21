using ExitCafe.Application.Common.Interfaces;
using Microsoft.AspNetCore.Hosting;

namespace ExitCafe.Infrastructure.Services;

public class LocalFileStorageService : IFileStorageService
{
    private readonly IWebHostEnvironment _env;

    public LocalFileStorageService(IWebHostEnvironment env)
    {
        _env = env;
    }

    public async Task<string> SaveAsync(Stream content, string fileName, string subfolder, CancellationToken ct = default)
    {
        // Defense in depth: the command validator already allowlists subfolder, but never trust a
        // caller-supplied path segment when building a filesystem path.
        if (subfolder.Contains("..") || subfolder.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
            throw new ArgumentException("Invalid upload subfolder.", nameof(subfolder));

        var webRoot = _env.WebRootPath ?? Path.Combine(_env.ContentRootPath, "wwwroot");
        var targetDir = Path.Combine(webRoot, "uploads", subfolder);
        Directory.CreateDirectory(targetDir);

        var safeExtension = Path.GetExtension(fileName) is { Length: > 0 } ext ? ext : "";
        var storedName = $"{Guid.NewGuid():N}{safeExtension}";
        var fullPath = Path.Combine(targetDir, storedName);

        await using var fileStream = File.Create(fullPath);
        await content.CopyToAsync(fileStream, ct);

        return $"/uploads/{subfolder}/{storedName}";
    }
}
