using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Net.Http.Headers;
using Samples.StaticFiles.Enums;
using Samples.StaticFiles.Models;
using Samples.StaticFiles.Utils;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using System.Net;

namespace Samples.StaticFiles.Services;

public class FileUploadService
{
    private readonly ILogger<FileUploadService> _logger;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public FileUploadService(ILogger<FileUploadService> logger, IHttpContextAccessor httpContextAccessor)
    {
        _logger = logger;
        _httpContextAccessor = httpContextAccessor;
    }

    /// <summary>
    /// 保存文件并返回保存结果
    /// </summary>
    /// <param name="file">文件实体</param>
    ///<param name="projectName">项目名</param>
    /// <returns></returns>
    public async Task<FileUploadResult> SaveFile(ImageUploadRequest fileSaveInfo)
    {
        var result = new FileUploadResult();

        string folderName;
        string filePath;
        string fileName;

        try
        {
            if (fileSaveInfo.File?.Length > 0)
            {
                // 若没有指定文件扩展名，通过文件名获取
                if (fileSaveInfo.FileExtension.IsEmpty())
                {
                    fileSaveInfo.FileExtension = Path.GetExtension(fileSaveInfo.File.FileName);
                }

                bool isImage = FileUtils.IsImageExtension(fileSaveInfo.FileExtension);

                folderName = FileUtils.GetFilePathWithWorkPath(Path.Combine(Constants.FileFolderName, fileSaveInfo.ProjectName));
                fileName = Guid.NewGuid().ToString("N") + fileSaveInfo.FileExtension;
                filePath = Path.Combine(folderName, fileName);

                if (!Directory.Exists(folderName))
                {
                    Directory.CreateDirectory(folderName);
                }

                if (isImage)
                {
                    using (var stream = fileSaveInfo.File.OpenReadStream())
                    {
                        using (Image image = Image.Load(stream))
                        {
                            if (fileSaveInfo.Resize)
                            {
                                image.Mutate(x => x.Resize(fileSaveInfo.Weight, fileSaveInfo.Height));
                            }

                            await image.SaveAsync(filePath);
                        }
                    }
                }
                else
                {
                    using (var stream = File.Create(filePath))
                    {
                        await fileSaveInfo.File.CopyToAsync(stream);
                    }
                }
            }
            else
            {
                result.Result = ResponseStatusEnums.FileContentEmpty;
                return result;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"文件保存失败，错误信息：{ex}");
            result.Result = ResponseStatusEnums.SaveFileFail;
            return result;
        }

        result.DomainName = $"{_httpContextAccessor.HttpContext.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host.Value}";
        result.RelativeFilePath = $"/files/{fileSaveInfo.ProjectName}/{fileName}";
        result.FilePath = result.DomainName + result.RelativeFilePath;

        return result;
    }

    /// <summary>
    /// 保存大文件
    /// </summary>
    /// <param name="section">请求报文中的文件section实体</param>
    /// <returns></returns>
    public async Task<FileUploadResult> SaveLargeFile(MultipartSection section)
    {
        var result = new FileUploadResult();

        var hasContentDispositionHeader =
            ContentDispositionHeaderValue.TryParse(
                section.ContentDisposition, out var contentDisposition);

        if (hasContentDispositionHeader)
        {
            // 验证是否含有非文件数据
            if (!MultipartRequestHelper.HasFileViaContentDisposition(contentDisposition))
            {
                result.Result = ResponseStatusEnums.HasNotFileContentInRequest;
                return result;
            }

            // 验证是否含有项目名
            if (!MultipartRequestHelper.GetNameFromContentDisposition(contentDisposition, out var projectName))
            {
                result.Result = ResponseStatusEnums.ProjectNameIsEmpty;
                return result;
            }

            var folderName = FileUtils.GetFilePathWithWorkPath(Path.Combine(Constants.FileFolderName, projectName));
            var fileName = Guid.NewGuid().ToString("N") + Path.GetExtension(WebUtility.HtmlEncode(
                    contentDisposition.FileName.Value));

            var filePath = Path.Combine(folderName, fileName);

            if (!Directory.Exists(folderName))
            {
                Directory.CreateDirectory(folderName);
            }

            using (var targetStream = File.Create(filePath))
            {
                await section.Body.CopyToAsync(targetStream);
            }

            result.DomainName = $"{_httpContextAccessor.HttpContext.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host.Value}";
            result.RelativeFilePath = $"/files/{projectName}/{fileName}";
            result.FilePath = result.DomainName + result.RelativeFilePath;
        }
        else
        {
            result.Result = ResponseStatusEnums.FileContentEmpty;
        }

        return result;
    }

    /// <summary>
    /// 保存视频文件
    /// </summary>
    /// <param name="fileSaveInfo">保存文件请求</param>
    /// <returns></returns>
    public async Task<FileUploadResult> SaveVideo(FileUploadRequest fileSaveInfo)
    {
        var result = new FileUploadResult();

        string folderName;
        string filePath;
        string fileName;

        try
        {
            if (fileSaveInfo.File?.Length > 0)
            {

                var fileExtension = Path.GetExtension(fileSaveInfo.File.FileName);

                folderName = FileUtils.GetFilePathWithWorkPath(Path.Combine(Constants.FileFolderName, fileSaveInfo.ProjectName));
                fileName = Guid.NewGuid().ToString("N") + fileExtension;
                filePath = Path.Combine(folderName, fileName);

                if (!Directory.Exists(folderName))
                {
                    Directory.CreateDirectory(folderName);
                }

                using (var stream = File.Create(filePath))
                {
                    await fileSaveInfo.File.CopyToAsync(stream);
                }
            }
            else
            {
                result.Result = ResponseStatusEnums.FileContentEmpty;
                return result;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"文件保存失败，错误信息：{ex}");
            result.Result = ResponseStatusEnums.SaveFileFail;
            return result;
        }

        result.DomainName = $"{_httpContextAccessor.HttpContext.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host.Value}";
        result.RelativeFilePath = $"/files/{fileSaveInfo.ProjectName}/{fileName}";
        result.FilePath = result.DomainName + result.RelativeFilePath;
        return result;
    }
}

