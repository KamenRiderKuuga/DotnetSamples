using System.ComponentModel;

namespace Samples.StaticFiles.Enums;

/// <summary>
/// 业务状态码枚举
/// </summary>
/// <remarks>
/// 部分在HTTP状态码内有定义的内容会和HTTP状态码相同
/// </remarks>
public enum ResponseStatusEnums
{
    /// <summary>
    /// 业务执行成功
    /// </summary>
    [Description("业务执行成功")]
    Success = 0,

    /// <summary>
    /// 权限认证失败
    /// </summary>
    [Description("权限认证失败")]
    Forbidden = 403,

    /// <summary>
    /// 保存文件失败
    /// </summary>
    [Description("保存文件失败")]
    SaveFileFail = 10001,

    /// <summary>
    /// 文件内容为空
    /// </summary>
    [Description("文件内容为空")]
    FileContentEmpty = 10002,

    /// <summary>
    /// 文件扩展名为空
    /// </summary>
    [Description("文件扩展名为空")]
    FileExtensionEmpty = 10003,

    /// <summary>
    /// 请使用multipart类型的请求
    /// </summary>
    [Description("请使用multipart类型的请求")]
    UploadFileRequestError = 10004,

    /// <summary>
    /// 请求中含有非文件内容
    /// </summary>
    [Description("请求中含有非文件内容")]
    HasNotFileContentInRequest = 10005,

    /// <summary>
    /// 请求中含有非文件内容
    /// </summary>
    [Description("项目名为空")]
    ProjectNameIsEmpty = 10007,
}
