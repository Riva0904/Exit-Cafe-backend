using ExitCafe.Application.Common.Models;
using ExitCafe.Application.Features.Uploads.Commands.UploadImages;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExitCafe.WebApi.Controllers;

[ApiController]
[Route("api/uploads")]
[Authorize(Policy = "StaffAndAbove")]
public class UploadsController : ControllerBase
{
    private readonly IMediator _mediator;

    public UploadsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("images/{subfolder}")]
    [RequestSizeLimit(25_000_000)]
    public async Task<ActionResult<ApiResponse<List<string>>>> UploadImages(string subfolder, [FromForm] List<IFormFile> files, CancellationToken ct)
    {
        var payloads = files.Select(f => new UploadedFilePayload(f.OpenReadStream(), f.FileName, f.ContentType, f.Length)).ToList();
        var urls = await _mediator.Send(new UploadImagesCommand(payloads, subfolder), ct);
        // Storage backends may return either an app-relative path (local disk) or an already-absolute
        // URL (Cloudinary and similar). Only the former needs this API's own scheme+host prefixed.
        var baseUrl = $"{Request.Scheme}://{Request.Host}";
        var absoluteUrls = urls.Select(u => u.StartsWith("http://") || u.StartsWith("https://") ? u : $"{baseUrl}{u}").ToList();
        return Ok(ApiResponse<List<string>>.Ok(absoluteUrls, "Images uploaded."));
    }
}
