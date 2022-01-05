using Samples.StaticFiles.Enums;
using Samples.StaticFiles.Utils;

namespace Samples.StaticFiles.Models;

/// <summary>
/// HTTP请求响应基类
/// </summary>
public class ResponseBase
{
    /// <summary>
    /// 状态码
    /// </summary>
    public ResponseStatusEnums Status { get; set; }

    /// <summary>
    /// 操作是否成功
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// 返回信息
    /// </summary>
    public string Msg { get; set; } = string.Empty;


    /// <summary>
    /// 响应失败，并返回错误码
    /// </summary>
    /// <returns></returns>
    public static ResponseBase Result(ResponseStatusEnums status)
    {
        bool success = status == ResponseStatusEnums.Success;

        return new ResponseBase()
        {
            Success = success,
            Status = status,
            Msg = status.GetRemark()
        };
    }
}


/// <summary>
/// HTTP请求响应基类（泛型）
/// </summary>
public class ResponseBase<T> : ResponseBase where T : ResultBase
{
    /// <summary>
    /// 返回数据集合
    /// </summary>
    public T? Data { get; set; }

    /// <summary>
    /// 响应成功，默认值
    /// </summary>
    /// <returns></returns>
    public static ResponseBase<T> Result(T data)
    {
        bool success = data.Result == ResponseStatusEnums.Success;

        return new ResponseBase<T>()
        {
            Success = success,
            Status = data.Result,
            Msg = data.Result.GetRemark(),
            Data = success ? data : default
        };
    }
}

