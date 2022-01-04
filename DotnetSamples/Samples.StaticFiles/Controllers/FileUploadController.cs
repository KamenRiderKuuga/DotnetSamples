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
/// ����ͼƬ�ϴ������Ŀ�����
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
    /// �ļ��ϴ��ӿ�
    /// </summary>
    /// <param name="request">��Ҫ�ϴ����ļ���Ϣʵ��</param>
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
    /// ��Ƶ�ϴ��ӿ�
    /// </summary>
    /// <param name="request">��Ҫ�ϴ�����Ƶ�ļ���Ϣʵ��</param>
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
    /// ���ļ��ϴ��ӿڣ������ƣ���Ҫ��ǩ��
    /// </summary>
    /// <returns></returns>
    /// <remarks>
    /// ʹ��multipart/form-data���������ͣ�form-data��ֻ��һ�
    /// keyʹ����Ŀ���ƣ�value��Ҫ�ϴ����ļ�������ʹ��Postman����ģ������
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