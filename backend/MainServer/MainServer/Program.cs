using MainServer.Data;
using MainServer.Interfaces;
using MainServer.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using SharedLib.Enums;
using SharedLib.GlobalClasses;
using SharedLib.GlobalInterfaces;
using System.Text;

var builder = WebApplication.CreateBuilder(args); //Это экземляр класса-настройщика сервера

var jwdSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>();
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");


//Настройка портов которые будет прослушивать сервер
builder.WebHost.UseUrls("https://localhost:8080");


//builder.Services.AddScoped<какой тип просят, что вернуть>
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString)
    .LogTo(Console.WriteLine, LogLevel.Information));

builder.Services.AddSingleton(jwdSettings);
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IVideoService, VideoService>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(option =>
{
    option.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwdSettings.Issuer,
        ValidAudience = jwdSettings.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwdSettings.SecretKey))
    };
});
builder.Services.AddAuthorization();

//Эти команды записывают отладочную онформацию, необходимую для тестирования
//Отладка расположена по этому адресу https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var allowedOrigins = builder.Configuration.GetSection("CorsOrigins").Get<string[]>();
builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendPolicy", policy =>
    {
        policy.WithOrigins(allowedOrigins ?? new[] { "http://localhost:3000" })
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

builder.Services.AddHttpContextAccessor();

builder.Services.Configure<FormOptions>(options =>
{
    options.ValueLengthLimit = int.MaxValue;
    options.MultipartBodyLengthLimit = 2L * 1024L * 1024L * 1024L; // 2GB
    options.MemoryBufferThreshold = int.MaxValue;
});//Указываю что сервер должен поддерживать файлы до 2ух гигобайт
builder.WebHost.ConfigureKestrel(options =>
{
    options.Limits.MaxRequestBodySize = 2L * 1024L * 1024L * 1024L; // 2GB
});

//Регистрация сервисов
builder.Services.AddControllers();



var app = builder.Build();

app.UseExceptionHandler(new ExceptionHandlerOptions { ExceptionHandler = HandleException });

async Task HandleException(HttpContext context)
{
    var exceptonHandlerFeature = context.Features.Get<IExceptionHandlerFeature>();
    var exception = exceptonHandlerFeature?.Error;

    int statusCode = 500;
    string errorType = ErrorType.ServerError.ToString();
    string message = "Internal Server Error";


    if (exception is ApiException apiException)
    {
        statusCode = ApiException.GetStatusCode(apiException.ErrorType);
        errorType = apiException.ErrorType.ToString();
        message = apiException.Message;
    }

    context.Response.StatusCode = statusCode;
    context.Response.ContentType = "application/json";

    var response = new
    {
        ErrorType = errorType,
        Message = message,
        Timestamp = DateTime.UtcNow
    };

    await context.Response.WriteAsJsonAsync(response);
}

//Настройка сервера
app.UseHttpsRedirection();//Перенаправляет запросы полученные по http на порт с https
app.UseCors("FrontendPolicy");
app.UseAuthentication();//Распознает пользователя, запускает контроллер с атрибутом [Authorize]  
app.UseAuthorization();
app.MapControllers(); 


app.Run();