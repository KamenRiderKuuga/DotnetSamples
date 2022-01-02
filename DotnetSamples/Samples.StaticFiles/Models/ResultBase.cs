using Samples.StaticFiles.Enums;
using System.Text.Json.Serialization;

namespace Samples.StaticFiles.Models;

/// <summary>
/// 执行结果基类
/// </summary>
public class ResultBase
{
    /// <summary>
    /// 执行结果
    /// </summary>
    [JsonIgnore]
    public ResponseStatusEnums Result { get; set; } = ResponseStatusEnums.Success;
}
