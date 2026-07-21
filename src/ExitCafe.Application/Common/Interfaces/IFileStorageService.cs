namespace ExitCafe.Application.Common.Interfaces;

public interface IFileStorageService
{
    /// <summary>Saves the stream to disk and returns a URL path relative to the app root (e.g. "/uploads/products/&lt;guid&gt;.jpg").</summary>
    Task<string> SaveAsync(Stream content, string fileName, string subfolder, CancellationToken ct = default);
}
