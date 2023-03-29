using System.Reflection;
using System.Text;
using Common;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Repository;
using Service;
using SqlSugar;
using WebApi;
using WebApi.Db;
using WebApi.Profiles;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers()
    .AddNewtonsoftJson(
    options =>options.SerializerSettings.DateFormatString = "yyyy-MM-dd");
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

// jwt
#region Jwt
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var tokenModel = builder.Configuration.GetSection("Jwt").Get<TokenModelJwt>();
        var secretByte = Encoding.UTF8.GetBytes(tokenModel.Secret);
        options.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidateIssuer = true,
            ValidIssuer = tokenModel.Issuer,

            ValidateAudience = true,
            ValidAudience = tokenModel.Audience,

            ValidateLifetime = true,

            IssuerSigningKey = new SymmetricSecurityKey(secretByte)
        };
        options.Events = new JwtBearerEvents
        {
            OnChallenge = context =>
            {
                //此处代码为终止.Net Core默认的返回类型和数据结果，这个很重要哦，必须
                //context.HandleResponse();

                //自定义自己想要返回的数据结果，我这里要返回的是Json对象，通过引用Newtonsoft.Json库进行转换

                //自定义返回的数据类型
                //context.Response.ContentType = "text/plain";
                ////自定义返回状态码，默认为401 我这里改成 200
                ////context.Response.StatusCode = StatusCodes.Status200OK;
                //context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                ////输出Json数据结果
                //context.Response.WriteAsync("expired");
                return Task.FromResult(0);
            },
            //403
            OnForbidden = context =>
            {
                //context.Response.ContentType = "text/plain";
                ////自定义返回状态码，默认为401 我这里改成 200
                ////context.Response.StatusCode = StatusCodes.Status200OK;
                //context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                ////输出Json数据结果
                //context.Response.WriteAsync("expired");
                return Task.FromResult(0);
            }

        };
    });
#endregion
builder.Services.AddSqlsugarSetup(builder.Configuration);
builder.Services.AddAutoMapper(typeof(MapperProfiles));
//缓存
builder.Services.AddMemoryCache();
builder.Services.AddSwaggerGen(options =>
{
    
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Description = "在下框中输入请求头中需要添加Jwt授权Token：Bearer Token",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] { }
        }
    });

    // api版本
    typeof(APiVersion).GetEnumNames().ToList().ForEach(Version =>
    {
        options.SwaggerDoc(Version, new OpenApiInfo
        {
            Version = Version,
            Title = "API标题",
            Description = $"API描述+{Version}"
        });
    });
});


    
// 注入仓储
builder.Services.AddScoped<StudentRepository>();
//builder.Services.AddScoped<StudentService>();
// 使用接口注入
builder.Services.AddScoped<IStudentService,StudentService>();
// 数据校验注入
builder.Services.AddFluentValidation(opt =>
{
    opt.RegisterValidatorsFromAssembly(Assembly.GetExecutingAssembly());
});
// 跨域
builder.Services.AddCors(c =>
{
    c.AddPolicy("Cors", policy =>
    {
        policy
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        typeof(APiVersion).GetEnumNames().ToList().ForEach(Version =>
        {
            options.SwaggerEndpoint($"/swagger/{Version}/swagger.json",$"版本选择：{Version}");
        });
    });
}


app.UseCors("Cors");
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
