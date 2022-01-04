namespace Samples.StaticFiles.Utils;

/// <summary>
/// 文件相关工具类
/// </summary>
public class FileUtils
{
    /// <summary>
    /// 获取指定路径在当前程序运行目录的绝对路径
    /// </summary>
    /// <returns></returns>
    public static string GetFilePathWithWorkPath(string path)
    {
        return Path.Combine(AppContext.BaseDirectory, path);
    }


    /// <summary>
    /// 判断是否为图片后缀
    /// </summary>
    /// <param name="fileExtension"></param>
    /// <returns></returns>
    public static bool IsImageExtension(string fileExtension)
    {
        // 把扩展名转换成统一格式
        fileExtension = fileExtension.TrimStart('.').ToLower();

        if (Constants.ImageExtensions.Contains(fileExtension))
        {
            return true;
        }

        return false;
    }
}
