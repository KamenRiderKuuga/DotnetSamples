namespace Samples.StaticFiles.AppConfigs;

/// <summary>
/// 文件上传相关配置
/// </summary>
public class FileConfigs
{
    /// <summary>
    /// 图片，简历等小文件的大小限制
    /// </summary>
    public int ImageMaxSize { get; set; }

    /// <summary>
    /// 视频文件的大小限制
    /// </summary>
    public int VideoMaxSize { get; set; }
}