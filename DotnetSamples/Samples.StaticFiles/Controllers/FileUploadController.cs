using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Net.Http.Headers;
using Samples.StaticFiles.Enums;
using Samples.StaticFiles.Filters;
using Samples.StaticFiles.Models;
using Samples.StaticFiles.Services;
using Samples.StaticFiles.Utils;

namespace Samples.StaticFiles.Controllers;

/// <summary>
/// 控制图片上传操作的控制器
/// </summary>
[ApiController]
[Route("[controller]")]
public class FileUploadController : ControllerBase
{
    private readonly ILogger<FileUploadController> _logger;
    private readonly FileUploadService _service;

    public FileUploadController(ILogger<FileUploadController> logger, FileUploadService service)
    {
        _logger = logger;
        _service = service;
    }

    /// <summary>
    /// 文件上传接口
    /// </summary>
    /// <param name="request">需要上传的文件信息实体</param>
    /// <returns></returns>
    [HttpPost]
    [Route(nameof(Upload))]
    [UploadFileSizeLimit(ActionTypeEnums.UploadImage)]
    public async Task<ResponseBase<FileUploadResult>> Upload([FromForm] ImageUploadRequest request)
    {
        FileUploadResult result = await _service.SaveFile(request);

        return ResponseBase<FileUploadResult>.Result(result);
    }

    /// <summary>
    /// 视频上传接口
    /// </summary>
    /// <param name="request">需要上传的视频文件信息实体</param>
    /// <returns></returns>
    [HttpPost]
    [Route(nameof(UploadVideo))]
    [UploadFileSizeLimit(ActionTypeEnums.UploadVideo)]
    public async Task<ResponseBase<FileUploadResult>> UploadVideo([FromForm] FileUploadRequest request)
    {
        FileUploadResult result = await _service.SaveVideo(request);

        return ResponseBase<FileUploadResult>.Result(result);
    }

    /// <summary>
    /// 大文件上传接口（无限制，需要验签）
    /// </summary>
    /// <returns></returns>
    /// <remarks>
    /// 使用multipart/form-data的请求类型，form-data里只有一项，
    /// key使用项目名称，value是要上传的文件，可以使用Postman进行模拟请求
    /// </remarks>
    [HttpPost]
    [SignRequirement(SignVerifyEnums.SHA256)]
    [RequestSizeLimit(long.MaxValue)]
    [Route(nameof(UploadLargeFile))]
    public async Task<ResponseBase<FileUploadResult>> UploadLargeFile()
    {
        var result = new FileUploadResult();

        if (!MultipartRequestHelper.IsMultipartContentType(Request.ContentType))
        {
            result.Result = ResponseStatusEnums.UploadFileRequestError;
            return ResponseBase<FileUploadResult>.Result(result);
        }

        var boundary = MultipartRequestHelper.GetBoundary(
            MediaTypeHeaderValue.Parse(Request.ContentType),
            1000);

        var reader = new MultipartReader(boundary, HttpContext.Request.Body);
        reader.BodyLengthLimit = long.MaxValue;
        var section = await reader.ReadNextSectionAsync();

        result = await _service.SaveLargeFile(section);

        return ResponseBase<FileUploadResult>.Result(result);
    }
}