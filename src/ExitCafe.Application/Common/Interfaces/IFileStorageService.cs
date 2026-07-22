namespace ExitCafe.Application.Common.Interfaces;

public interface IFileStorageService
{
    /// <summary>
    /// Persists the stream and returns either a URL path relative to the app root
    /// (e.g. "/uploads/products/&lt;guid&gt;.jpg") or an already-absolute URL, depending on the
    /// backing store. Callers must not assume which — see UploadsController.
    /// </summary>
    Task<string> SaveAsync(Stream content, string fileName, string subfolder, CancellationToken ct = default);
}
