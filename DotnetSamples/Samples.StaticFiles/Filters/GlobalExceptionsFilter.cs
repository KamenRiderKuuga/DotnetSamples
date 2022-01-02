using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Samples.StaticFiles.Filters;

public class GlobalExceptionsFilter : IExceptionFilter
{
    private readonly ILogger<GlobalExceptionsFilter> _loggerHelper;

    public GlobalExceptionsFilter(ILogger<GlobalExceptionsFilter> loggerHelper)
    {
        _loggerHelper = loggerHelper;
    }

    public void OnException(ExceptionContext context)
    {
        var json = new JsonErrorResponse();

        json.Message = context.Exception.Message;//错误信息

        json.DevelopmentMessage = context.Exception.StackTrace;//堆栈信息

        context.Result = new InternalServerErrorObjectResult(json);

        //采用log4net 进行错误日志记录
        _loggerHelper.LogError(json.Message + WriteLog(json.Message, context.Exception));
    }

    /// <summary>
    /// 自定义返回格式
    /// </summary>
    /// <param name="throwMsg"></param>
    /// <param name="ex"></param>
    /// <returns></returns>
    public string WriteLog(string throwMsg, Exception ex)
    {
        return $"\r\n【自定义错误】：{throwMsg} \r\n【异常类型】：{ex.GetType().Name} \r\n【异常信息】：{ex.Message} \r\n【堆栈调用】：{ex.StackTrace}";
    }
}

/// <summary>
/// 服务器内部错误实体
/// </summary>
public class InternalServerErrorObjectResult : ObjectResult
{
    public InternalServerErrorObjectResult(object value) : base(value)
    {
        StatusCode = StatusCodes.Status500InternalServerError;
    }
}


/// <summary>
/// 返回的错误信息实体
/// </summary>
public class JsonErrorResponse
{
    /// <summary>
    /// 生产环境的消息
    /// </summary>
    public string? Message { get; set; }
    /// <summary>
    /// 开发环境的消息
    /// </summary>
    public string? DevelopmentMessage { get; set; }
}
