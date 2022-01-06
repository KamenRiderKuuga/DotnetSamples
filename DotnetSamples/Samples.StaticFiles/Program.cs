using FluentValidation.AspNetCore;
using Microsoft.Extensions.FileProviders;
using Samples.StaticFiles;
using Samples.StaticFiles.AppConfigs;
using Samples.StaticFiles.Filters;
using Samples.StaticFiles.Models.Validators;
using Samples.StaticFiles.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpContextAccessor();

builder.Services.AddControllers(option =>
{
    // ȫ���쳣����
    option.Filters.Add(typeof(GlobalExceptionsFilter));
}).AddFluentValidation(conf =>
{
    conf.RegisterValidatorsFromAssemblyContaining<FileUploadValidator>();
    conf.DisableDataAnnotationsValidation = false;
});

// ��������
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        builder =>
        {
            builder.AllowAnyOrigin() // ��������վ���������
            .AllowAnyMethod() // �����������󷽷�
            .AllowAnyHeader(); // ������������ͷ
        });
});

builder.Services.AddScoped<FileUploadService>();

builder.Services.Configure<SecretKeyConfigs>(builder.Configuration.GetSection(nameof(SecretKeyConfigs)));
builder.Services.Configure<FileConfigs>(builder.Configuration.GetSection(nameof(FileConfigs)));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

var staticFileFolder = Path.Combine(AppContext.BaseDirectory, Constants.FileFolderName);

if (!Directory.Exists(staticFileFolder))
{
    Directory.CreateDirectory(staticFileFolder);
}

app.UseStaticFiles(new StaticFileOptions()
{
    FileProvider = new PhysicalFileProvider(staticFileFolder),
    RequestPath = new PathString("/files"),
    ServeUnknownFileTypes = true
});

app.MapControllers();

app.Run();
