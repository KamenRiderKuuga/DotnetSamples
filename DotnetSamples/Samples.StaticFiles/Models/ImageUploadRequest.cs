namespace Samples.StaticFiles.Models;

/// <summary>
/// 图片上传请求类
/// </summary>
public class ImageUploadRequest : FileUploadRequest
{
    /// <summary>
    /// 文件扩展名，可以根据需求进行传递，例：.jpg
    /// </summary>
    public string? FileExtension { get; set; }

    /// <summary>
    /// 是否调整图片大小（上传的文件为非图片格式时请忽略）
    /// </summary>
    public bool Resize { get; set; }

    /// <summary>
    /// 调整图片的高度为，（上传的文件为非图片格式时请忽略，单位：像素）
    /// </summary>
    public int Height { get; set; }

    /// <summary>
    /// 调整图片的宽度为（上传的文件为非图片格式时请忽略，单位：像素）
    /// </summary>
    public int Weight { get; set; }
}