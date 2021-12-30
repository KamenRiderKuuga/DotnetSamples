using FluentValidation.AspNetCore;
using Samples.StaticFiles.Filters;
using Samples.StaticFiles.Models.Validators;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpContextAccessor();

builder.Services.AddControllers(option =>
{
    // 全局异常过滤
    option.Filters.Add(typeof(GlobalExceptionsFilter));
}).AddFluentValidation(conf =>
{
    conf.RegisterValidatorsFromAssemblyContaining<FileUploadValidator>();
    conf.DisableDataAnnotationsValidation = false;
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
