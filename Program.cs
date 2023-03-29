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
                //�˴�����Ϊ��ֹ.Net CoreĬ�ϵķ������ͺ����ݽ�����������ҪŶ������
                //context.HandleResponse();

                //�Զ����Լ���Ҫ���ص����ݽ����������Ҫ���ص���Json����ͨ������Newtonsoft.Json�����ת��

                //�Զ��巵�ص���������
                //context.Response.ContentType = "text/plain";
                ////�Զ��巵��״̬�룬Ĭ��Ϊ401 ������ĳ� 200
                ////context.Response.StatusCode = StatusCodes.Status200OK;
                //context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                ////���Json���ݽ��
                //context.Response.WriteAsync("expired");
                return Task.FromResult(0);
            },
            //403
            OnForbidden = context =>
            {
                //context.Response.ContentType = "text/plain";
                ////�Զ��巵��״̬�룬Ĭ��Ϊ401 ������ĳ� 200
                ////context.Response.StatusCode = StatusCodes.Status200OK;
                //context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                ////���Json���ݽ��
                //context.Response.WriteAsync("expired");
                return Task.FromResult(0);
            }

        };
    });
#endregion
builder.Services.AddSqlsugarSetup(builder.Configuration);
builder.Services.AddAutoMapper(typeof(MapperProfiles));
//����
builder.Services.AddMemoryCache();
builder.Services.AddSwaggerGen(options =>
{
    
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Description = "���¿�����������ͷ����Ҫ���Jwt��ȨToken��Bearer Token",
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

    // api�汾
    typeof(APiVersion).GetEnumNames().ToList().ForEach(Version =>
    {
        options.SwaggerDoc(Version, new OpenApiInfo
        {
            Version = Version,
            Title = "API����",
            Description = $"API����+{Version}"
        });
    });
});


    
// ע��ִ�
builder.Services.AddScoped<StudentRepository>();
//builder.Services.AddScoped<StudentService>();
// ʹ�ýӿ�ע��
builder.Services.AddScoped<IStudentService,StudentService>();
// ����У��ע��
builder.Services.AddFluentValidation(opt =>
{
    opt.RegisterValidatorsFromAssembly(Assembly.GetExecutingAssembly());
});
// ����
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
            options.SwaggerEndpoint($"/swagger/{Version}/swagger.json",$"�汾ѡ��{Version}");
        });
    });
}


app.UseCors("Cors");
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
