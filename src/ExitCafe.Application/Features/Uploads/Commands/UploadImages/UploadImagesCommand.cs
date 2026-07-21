using MediatR;

namespace ExitCafe.Application.Features.Uploads.Commands.UploadImages;

public record UploadedFilePayload(Stream Content, string FileName, string ContentType, long Length);

public record UploadImagesCommand(List<UploadedFilePayload> Files, string Subfolder) : IRequest<List<string>>;
