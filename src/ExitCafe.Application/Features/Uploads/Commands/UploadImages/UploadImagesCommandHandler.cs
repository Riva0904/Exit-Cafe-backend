using ExitCafe.Application.Common.Interfaces;
using MediatR;

namespace ExitCafe.Application.Features.Uploads.Commands.UploadImages;

public class UploadImagesCommandHandler : IRequestHandler<UploadImagesCommand, List<string>>
{
    private readonly IFileStorageService _fileStorage;

    public UploadImagesCommandHandler(IFileStorageService fileStorage)
    {
        _fileStorage = fileStorage;
    }

    public async Task<List<string>> Handle(UploadImagesCommand request, CancellationToken ct)
    {
        var urls = new List<string>();
        foreach (var file in request.Files)
        {
            var url = await _fileStorage.SaveAsync(file.Content, file.FileName, request.Subfolder, ct);
            urls.Add(url);
        }
        return urls;
    }
}
