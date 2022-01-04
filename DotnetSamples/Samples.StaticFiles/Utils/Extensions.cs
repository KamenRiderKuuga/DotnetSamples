using System.ComponentModel;
using System.Reflection;

namespace Samples.StaticFiles.Utils;

/// <summary>
/// 一些扩展方法
/// </summary>
public static class Extensions
{
    /// <summary>
    /// 判断字符串是否为空
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static bool IsEmpty(this string value)
    {
        return string.IsNullOrEmpty(value);
    }

    /// <summary>
    /// 获取枚举的备注信息
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static string GetRemark(this Enum value)
    {
        FieldInfo? field = value.GetType().GetField(value.ToString());
        if (field == null)
        {
            return value.ToString();
        }

        object[] customAttributes = field.GetCustomAttributes(typeof(DescriptionAttribute), inherit: false);
        if (customAttributes.Length != 0)
        {
            return ((DescriptionAttribute)customAttributes[0]).Description;
        }

        return value.ToString();
    }
}
