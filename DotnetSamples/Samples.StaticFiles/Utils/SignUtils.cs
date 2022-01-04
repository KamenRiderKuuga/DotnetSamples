using System.Security.Cryptography;
using System.Text;

namespace Samples.StaticFiles.Utils;

/// <summary>
/// 用于签名，验签的类
/// </summary>
public class SignUtils
{
    /// <summary>
    /// 使用指定的参数计算出的SHA256值，与提供的签名进行校验
    /// </summary>
    /// <param name="content">要用来验签的内容，通常由几个参数拼接生成</param>
    /// <param name="secretKey">密钥</param>
    /// <param name="sign">提供的已有的签名</param>
    /// <returns></returns>
    public static bool VerifyForSHA256(string content, string secretKey, string sign)
    {
        return HmacSHA256(secretKey, content) == sign;
    }

    /// <summary>
    /// 计算字符串的SHA256值
    /// </summary>
    /// <param name="secretKey">密钥</param>
    /// <param name="plain">字符串内容</param>
    /// <returns></returns>
    public static string HmacSHA256(string secretKey, string plain)
    {
        var keyBytes = Encoding.UTF8.GetBytes(secretKey);
        var plainBytes = Encoding.UTF8.GetBytes(plain);

        using (var hmacsha256 = new HMACSHA256(keyBytes))
        {
            var sb = new StringBuilder();
            var hashValue = hmacsha256.ComputeHash(plainBytes);
            foreach (byte x in hashValue)
            {
                sb.Append(string.Format("{0:x2}", x));
            }
            return sb.ToString();
        }
    }
}
