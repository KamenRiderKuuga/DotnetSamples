using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;
using Samples.StaticFiles.AppConfigs;
using Samples.StaticFiles.Enums;
using Samples.StaticFiles.Models;
using Samples.StaticFiles.Utils;

namespace Samples.StaticFiles.Filters;

public class SignRequirementAttribute : TypeFilterAttribute
{
    /// <summary>
    /// 签名的验证类型
    /// </summary>
    /// <param name="verifyType"></param>
    public SignRequirementAttribute(SignVerifyEnums verifyType = SignVerifyEnums.None) : base(typeof(SignRequirementFilter))
    {
        Arguments = new object[] { verifyType };
    }
}

/// <summary>
/// 验签过滤器
/// </summary>
public class SignRequirementFilter : IAuthorizationFilter
{
    readonly SignVerifyEnums _verifyType;
    private readonly IOptions<SecretKeyConfigs> _secretKeyConfigs;
    private readonly ILogger<SignRequirementFilter> _logger;

    public SignRequirementFilter(SignVerifyEnums verifyType, IOptions<SecretKeyConfigs> secretKeyConfigs, ILogger<SignRequirementFilter> logger)
    {
        _verifyType = verifyType;
        _secretKeyConfigs = secretKeyConfigs;
        _logger = logger;
    }

    /// <summary>
    /// 权限认证函数
    /// </summary>
    /// <param name="context"></param>
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        // 这四个值用于验签
        context.HttpContext.Request.Headers.TryGetValue("appid", out var appIdValue);
        context.HttpContext.Request.Headers.TryGetValue("timestamp", out var timestampValue);
        context.HttpContext.Request.Headers.TryGetValue("sign", out var signValue);
        context.HttpContext.Request.Headers.TryGetValue("random", out var randomValue);

        // 因为是StringValues类型，这里需要重新定义字符串类型变量来获取，方便处理
        var appId = appIdValue.FirstOrDefault();
        var timestamp = timestampValue.FirstOrDefault();
        var sign = signValue.FirstOrDefault();
        var random = randomValue.FirstOrDefault();

        _logger.LogTrace($"开始验签，参数：appid:{appId}，timestamp：{timestamp}，sign:{sign}，random:{random}");

        string message = string.Empty;

        var setResponse = new Action(() =>
        {
            _logger.LogError($"验签失败，参数：appid:{appId}，timestamp：{timestamp}，sign:{sign}，random:{random}，原因：{message}");
            context.Result = new ObjectResult(ResponseBase.Result(ResponseStatusEnums.Forbidden));
        });

        bool verifyResult = false;

        // 验证验签参数是否为空
        if (appId.IsEmpty() || timestamp.IsEmpty() || sign.IsEmpty() || random.IsEmpty())
        {
            message = "验签参数缺失";
            setResponse();
            return;
        }

        // 验证时间戳，防止重放攻击
        if (long.TryParse(timestamp, out var requestTime))
        {
            if (Math.Abs((DateTimeOffset.FromUnixTimeSeconds(requestTime) - DateTimeOffset.UtcNow).TotalMinutes) > 5)
            {
                message = "请求已过期";
                setResponse();
                return;
            };
        }

        // 验证密钥是否存在
        if (!_secretKeyConfigs.Value.ContainsKey(appId))
        {
            message = "没有与此appid对应的密钥";
            setResponse();
            return;
        }

        switch (_verifyType)
        {
            case SignVerifyEnums.SHA256:
                var content = appId + timestamp + random;
                verifyResult = SignUtils.VerifyForSHA256(content, _secretKeyConfigs.Value[appId], sign);
                break;

            default:
                break;
        }

        if (!verifyResult)
        {
            _logger.LogError($"验签失败，参数：appid:{appId}，timestamp：{timestamp}，sign:{sign}，random:{random}，原因：{message}");
            context.Result = new ObjectResult(ResponseBase.Result(ResponseStatusEnums.Forbidden))
            {
                StatusCode = StatusCodes.Status403Forbidden
            };
        }
    }
}
