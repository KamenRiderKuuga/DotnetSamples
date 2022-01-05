using Microsoft.Net.Http.Headers;

namespace Samples.StaticFiles.Utils;

/// <summary>
/// multipart/form-data请求的帮助类
/// </summary>
public static class MultipartRequestHelper
{
    /// <summary>
    /// 获取到请求头中Content-Type的boundary
    /// </summary>
    /// <param name="contentType">代表请求头中Content-Type的实体对象</param>
    /// <param name="lengthLimit">大小限制</param>
    /// <remarks>
    /// Content-Type: multipart/form-data; boundary="----WebKitFormBoundarymx2fSWqWSd0OxQqq"
    /// </remarks>
    /// <returns></returns>
    public static string GetBoundary(MediaTypeHeaderValue contentType, int lengthLimit)
    {
        var boundary = HeaderUtilities.RemoveQuotes(contentType.Boundary).Value;

        if (string.IsNullOrWhiteSpace(boundary))
        {
            throw new InvalidDataException("请求头中缺少boundary");
        }

        if (boundary.Length > lengthLimit)
        {
            throw new InvalidDataException(
                $"boundary长度超过限制");
        }

        return boundary;
    }

    /// <summary>
    /// 根据请求头判断请求的类型是否是multipart类型
    /// </summary>
    /// <param name="contentType"></param>
    /// <returns></returns>
    public static bool IsMultipartContentType(string? contentType)
    {
        return !string.IsNullOrEmpty(contentType)
               && contentType.IndexOf("multipart/", StringComparison.OrdinalIgnoreCase) >= 0;
    }

    /// <summary>
    /// 通过Content-Disposition检查文件名是否为空
    /// </summary>
    /// <param name="contentDisposition">代表请求中Content-Disposition的实体</param>
    /// <returns></returns>
    public static bool HasFileViaContentDisposition(ContentDispositionHeaderValue contentDisposition)
    {
        // Content-Disposition: form-data; name="myfile1"; filename="Misc 002.jpg"
        return contentDisposition != null
            && contentDisposition.DispositionType.Equals("form-data")
            && (!string.IsNullOrEmpty(contentDisposition.FileName.Value)
                || !string.IsNullOrEmpty(contentDisposition.FileNameStar.Value));
    }

    /// <summary>
    /// 通过Content-Disposition检查名称
    /// </summary>
    /// <param name="contentDisposition">代表请求中Content-Disposition的实体</param>
    /// <returns></returns>
    public static bool GetNameFromContentDisposition(ContentDispositionHeaderValue contentDisposition, out string name)
    {
        var result = false;
        name = "";

        if (contentDisposition != null
            && contentDisposition.DispositionType.Equals("form-data")
            && (!string.IsNullOrEmpty(contentDisposition.Name.Value)
                || !string.IsNullOrEmpty(contentDisposition.Name.Value)))
        {
            result = true;
            name = contentDisposition.Name.Value;
        }

        return result;
    }
}