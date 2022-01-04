namespace Samples.StaticFiles;

/// <summary>
/// 在这里定义一些常量
/// </summary>
public class Constants
{
    /// <summary>
    /// 文件保存路径名
    /// </summary>
    public const string FileFolderName = "files";

    /// <summary>
    /// 所有的图片后缀名
    /// </summary>
    public static readonly HashSet<string> ImageExtensions = new HashSet<string> { "bmp", "jpg", "gif", "png", "jpeg" };
}
