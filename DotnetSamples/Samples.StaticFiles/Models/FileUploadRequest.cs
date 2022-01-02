using System.ComponentModel.DataAnnotations;

namespace Samples.StaticFiles.Models;

/// <summary>
/// 文件上传请求
/// </summary>
public class FileUploadRequest
{
    /// <summary>
    /// 上传的文件内容
    /// </summary>
    [Required]
    public IFormFile? File { get; set; }

    /// <summary>
    /// 项目名
    /// </summary>
    [Required]
    public string? ProjectName { get; set; }
}
