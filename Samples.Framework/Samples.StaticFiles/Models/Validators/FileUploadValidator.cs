using FluentValidation;

namespace Samples.StaticFiles.Models.Validators;

/// <summary>
/// 用于图片上传的模型验证器
/// </summary>
public class FileUploadValidator : AbstractValidator<ImageUploadRequest>
{
    /// <summary>
    /// 构造函数
    /// </summary>
    public FileUploadValidator()
    {
        RuleFor(x => x.File).Must(x => x?.Length > 0).WithMessage("上传的文件内容为空");
        RuleFor(x => x.Height).NotEmpty().When(x => x.Resize).WithMessage("当需要调整图片大小时，高不能为空");
        RuleFor(x => x.Weight).NotEmpty().When(x => x.Resize).WithMessage("当需要调整图片大小时，宽不能为空");
    }
}
