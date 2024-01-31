using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using VSIMS.WebApi;
using VSIMS.WebApi.Data;
using VSIMS.WebApi.Services.Student;
using System.Configuration;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<ImsDataContext>(
    //options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
    options =>
    {
        var conStr = builder.Configuration.GetConnectionString("DefaultConnection");
        options.UseMySql(conStr, ServerVersion.AutoDetect(conStr));
    });
// 以下是autofac依赖注入
builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
builder.Host.ConfigureContainer<ContainerBuilder>(builder =>
{
    // 注入Service程序集
    string assemblyName = Assembly.GetExecutingAssembly().GetName().Name;
    builder.RegisterAssemblyTypes(Assembly.Load(assemblyName))
        .AsImplementedInterfaces()
        .InstancePerDependency();
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();