using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;
using Samples.StaticFiles.AppConfigs;
using Samples.StaticFiles.Enums;

namespace Samples.StaticFiles.Filters;

public class UploadFileSizeLimitAttribute : TypeFilterAttribute
{
    /// <summary>
    /// 用于约束Action的请求体大小
    /// </summary>
    /// <param name="actionType">Action类型，对应不同的类型，有不同的请求大小限制</param>
    public UploadFileSizeLimitAttribute(ActionTypeEnums actionType) : base(typeof(UploadFileSizeLimitFilter))
    {
        Arguments = new object[] { actionType };
    }
}

/// <summary>
/// 验签过滤器
/// </summary>
public class UploadFileSizeLimitFilter : IAuthorizationFilter
{
    readonly ActionTypeEnums _actionType;
    private readonly FileConfigs _fileConfigs;
    private readonly ILogger<UploadFileSizeLimitFilter> _logger;

    public UploadFileSizeLimitFilter(ActionTypeEnums actionType, IOptions<FileConfigs> fileConfigs, ILogger<UploadFileSizeLimitFilter> logger)
    {
        _actionType = actionType;
        _fileConfigs = fileConfigs.Value;
        _logger = logger;
    }

    /// <summary>
    /// 权限认证函数
    /// </summary>
    /// <param name="context"></param>
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        if (context == null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        var effectivePolicy = context.FindEffectivePolicy<IRequestSizePolicy>();
        if (effectivePolicy != null && effectivePolicy != this)
        {
            _logger.LogError("当前过滤器不是最有效的过滤器");
            return;
        }

        var requestFormLimitsPolicy = context.FindEffectivePolicy<IRequestFormLimitsPolicy>();
        if (requestFormLimitsPolicy != null && requestFormLimitsPolicy != this)
        {
            _logger.LogError("当前过滤器不是最有效的过滤器");
            return;
        }

        var maxRequestBodySizeFeature = context.HttpContext.Features.Get<IHttpMaxRequestBodySizeFeature>();
        var formFeature = context.HttpContext.Features.Get<IFormFeature>();

        if (maxRequestBodySizeFeature == null)
        {
            _logger.LogError($"没有找到{nameof(maxRequestBodySizeFeature)}");
            return;
        }
        else if (maxRequestBodySizeFeature.IsReadOnly)
        {
            _logger.LogError($"{nameof(maxRequestBodySizeFeature)}是只读的");
            return;
        }
        else
        {
            var maxSize = 0L;
            switch (_actionType)
            {
                case ActionTypeEnums.UploadImage:
                    maxSize = 1024 * 1024 * _fileConfigs.ImageMaxSize;
                    maxRequestBodySizeFeature.MaxRequestBodySize = maxSize;
                    break;
                case ActionTypeEnums.UploadVideo:
                    maxSize = 1024 * 1024 * _fileConfigs.VideoMaxSize;
                    maxRequestBodySizeFeature.MaxRequestBodySize = maxSize;
                    break;
                default:
                    break;
            }

            if ((formFeature == null || formFeature.Form == null) && maxSize != 0L)
            {
                context.HttpContext.Features.Set<IFormFeature>(new FormFeature(context.HttpContext.Request, new FormOptions() { MultipartBodyLengthLimit = maxSize }));
            }

            _logger.LogInformation($"当前的请求类型是{_actionType}请求体大小限制被设置为{ maxRequestBodySizeFeature.MaxRequestBodySize}");
        }
    }
}
