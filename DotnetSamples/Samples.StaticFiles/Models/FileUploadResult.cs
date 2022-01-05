namespace Samples.StaticFiles.Models
{
    /// <summary>
    /// 文件上传结果响应实体
    /// </summary>
    public class FileUploadResult : ResultBase
    {
        /// <summary>
        /// 文件相对路径
        /// </summary>
        public string FilePath { get; set; } = string.Empty;
    }
}
