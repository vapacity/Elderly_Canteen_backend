using Elderly_Canteen.Data.Repos.Elderly_Canteen.Data.Repos;
using Elderly_Canteen.Data.Repos;
using Elderly_Canteen.Data.Entities;
using Elderly_Canteen.Services.Implements;
using Elderly_Canteen.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using Microsoft.AspNetCore.HttpOverrides;
using System.Net;
using Microsoft.Extensions.FileProviders;
using System.IO;
using Elderly_Canteen;
using Microsoft.AspNetCore.Cors.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// 设置 Kestrel 服务器选项，包括 MaxRequestBodySize
builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.Limits.MaxRequestBodySize = 52428800; // 50 MB
});

// 添加 CORS 服务，允许所有来源
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyHeader()
                   .AllowAnyMethod();
        });
});

// 添加控制器服务
builder.Services.AddControllers();

// 添加数据库上下文
builder.Services.AddDbContext<ModelContext>(options =>
    options.UseOracle(builder.Configuration.GetConnectionString("DefaultConnection")));
Console.WriteLine("Database Connection String: " + builder.Configuration.GetConnectionString("DefaultConnection"));
// 加载 OssConfig
var ossConfig = builder.Configuration.GetSection("OssConfig");
// 注册OSSService
builder.Services.AddSingleton<IOssService>(sp => new OSSService(
    ossConfig["Endpoint"],
    ossConfig["AccessKeyId"],
    ossConfig["AccessKeySecret"],
    ossConfig["BucketName"]
));
// 注册通用仓储服务
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

// 注册其他服务
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<IDonateService, DonateService>();
builder.Services.AddScoped<IEmployeeManagement, EmployeeManagement>();
builder.Services.AddScoped<IUsersService, UsersService>();
builder.Services.AddScoped<IVolunteerService, VolunteerService>();
builder.Services.AddMemoryCache();//配置内存缓存
builder.Services.AddScoped<IHomePageService, HomePageService>();
builder.Services.AddScoped<IIngreService, IngreService>();
builder.Services.AddScoped<IRepoService, RepoService>();
builder.Services.AddScoped<ICateService, CateService>();
builder.Services.AddScoped<IDishService, DishService>();
builder.Services.AddScoped<IWeekMenuService, WeekMenuService>();

// JWT 身份验证
var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = Encoding.ASCII.GetBytes(jwtSettings["Key"]);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
});

// 添加 Swagger
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Elderly Canteen API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter into field the word 'Bearer' followed by a space and the JWT value",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement {
    {
        new OpenApiSecurityScheme
        {
            Reference = new OpenApiReference
            {
                Type = ReferenceType.SecurityScheme,
                Id = "Bearer"
            },
        },
        new string[] { }
    }
    });
    c.OperationFilter<FormDataOperationFilter>();

    c.DocInclusionPredicate((docName, apiDesc) =>
    {
        // 检查是否有 FromForm 参数
        var isFormApi = apiDesc.ParameterDescriptions.Any(p => p.Source.Id == "FormFile" || p.Source.Id == "Form");

        // 如果是 form-data 接口，应用 FormDataOperationFilter
        if (isFormApi)
        {
            return true;
        }

        // 其他 API 返回 true 即可
        return !isFormApi;
    });
});

var app = builder.Build();

// 配置请求管道
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Elderly Canteen API V1");
    });
}

// 使用转发头中间件
app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

// 使用 CORS 中间件，允许所有来源
app.UseCors("AllowAllOrigins");

// 使用静态文件中间件
// 这里将自定义的 uploads 目录配置为静态文件目录
var uploadsPath = Path.Combine(builder.Environment.ContentRootPath, "uploads");
if (!Directory.Exists(uploadsPath))
{
    Directory.CreateDirectory(uploadsPath);
}

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(uploadsPath),
    RequestPath = "/uploads"
});

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
