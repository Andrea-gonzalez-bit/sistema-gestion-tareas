using ApiGenerico.Application.Services;
using ApiGenerico.Domain.Models;
using ApiGenerico.Infrastructure.Context;
using ApiGenerico.Utils.Security;
using ApiGenerico.WebAPI.Middleware;
using ApiGenerico.WebAPI.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
IEncryptionService Encrypt = new EncryptionService();
IConfigurationSection seccionConfiguracion = builder.Configuration.GetSection("SectionConfiguration");
IConfigurationSection seccionConnectionStrings = builder.Configuration.GetSection("ConnectionStrings");

builder.Host.UseSerilog((context, services, configuration) =>
{
    configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext();
});

builder.Services.Configure<SectionConfiguration>(seccionConfiguracion);
builder.Services.Configure<ConnectionStrings>(seccionConnectionStrings);
var configuracionAppSettings = seccionConfiguracion.Get<SectionConfiguration>();
var configuracionConnectionStrings = seccionConnectionStrings.Get<ConnectionStrings>();

string DecryptConnectionString(string encryptedConnectionString)
{
    return string.IsNullOrEmpty(encryptedConnectionString) ? null : Encrypt.Decrypt(encryptedConnectionString);
}

if (builder.Configuration.GetSection("ConnectionStrings:ConnetionToken").Exists())
{
    string GetConnetionToken = DecryptConnectionString(configuracionConnectionStrings.ConnetionToken);
}

if (builder.Configuration.GetSection("ConnectionStrings:ConnetionGenerico").Exists())
{
    string ConnetionGenerico = DecryptConnectionString(configuracionConnectionStrings.ConnetionGenerico);
    if (!string.IsNullOrEmpty(ConnetionGenerico))
    {
        builder.Services.AddDbContext<ContextSql>(opt => opt.UseSqlServer(ConnetionGenerico));
    }
}

builder.Services.AddScoped<IEncryptionService, EncryptionService>();

#region Registro dinamico de servicios (Dynamic Services Injection)
var generalServices = typeof(_Service).Assembly.GetTypes()
    .Where(type => !type.Name.StartsWith("_") && type.Name.EndsWith("Service"))
    .ToList();

var serviceInterfaces = generalServices.Where(type => type.IsInterface);
var serviceImplementations = generalServices.Where(type => type.IsClass);

foreach (var implementation in serviceImplementations)
{
    var interfaceName = $"I{implementation.Name}";
    var serviceInterface = serviceInterfaces.FirstOrDefault(i => i.Name == interfaceName);
    if (serviceInterface != null)
    {
        builder.Services.AddScoped(serviceInterface, implementation);
    }
}
#endregion

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Authentication:Issuer"],
        ValidAudience = builder.Configuration["Authentication:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Authentication:SecretKey"]))
    };
});

builder.Services.AddAuthorization();

builder.Services.AddSwaggerGen(opt =>
{
    opt.SwaggerDoc("V1", new OpenApiInfo { Title = "Task Management API", Version = "V1" });
    opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        In = ParameterLocation.Header,
        Description = "Enter JWT with bearer format like 'Bearer [Token]'"
    });
    opt.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
    opt.CustomSchemaIds(type => type.FullName);
    opt.DocInclusionPredicate((docName, apiDesc) =>
    {
        return apiDesc.GroupName == null || !apiDesc.GroupName.Equals("Hidden", StringComparison.OrdinalIgnoreCase);
    });
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowPolicySecureDomains", policy =>
    {
        policy.WithOrigins(configuracionAppSettings.SecureDomains)
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
    options.JsonSerializerOptions.PropertyNamingPolicy = null;
});

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var errors = new Dictionary<string, string[]>();

        foreach (var entry in context.ModelState.Where(e => e.Value?.Errors.Count > 0))
        {
            var key = NormalizeKey(entry.Key);
            var messages = entry.Value!.Errors
                .Select(error => TranslateValidationMessage(error.ErrorMessage, key))
                .ToArray();

            if (!errors.ContainsKey(key))
                errors[key] = messages;
        }

        var response = new ApiErrorResponse
        {
            Message = "La solicitud contiene errores de validación.",
            TraceId = context.HttpContext.TraceIdentifier,
            Errors = errors
        };

        return new BadRequestObjectResult(response);
    };

    static string NormalizeKey(string key)
    {
        if (key.StartsWith("$."))
            return key[2..];
        return key;
    }

    static string TranslateValidationMessage(string message, string normalizedKey)
    {
        if (string.IsNullOrWhiteSpace(message))
            return GetFormatErrorByField(normalizedKey);

        if (message.Contains("field is required", StringComparison.OrdinalIgnoreCase))
            return "El cuerpo de la solicitud no puede estar vacío.";

        if (message.Contains("is not valid", StringComparison.OrdinalIgnoreCase) ||
            message.Contains("invalid start of a value", StringComparison.OrdinalIgnoreCase) ||
            message.Contains("could not be converted", StringComparison.OrdinalIgnoreCase))
            return GetFormatErrorByField(normalizedKey);

        return message;
    }

    static string GetFormatErrorByField(string key) => key switch
    {
        "StateId" => "El identificador del estado es obligatorio y debe ser un número entero válido.",
        "DueDate" => "La fecha de vencimiento no tiene un formato válido.",
        _ => "El valor enviado no tiene el formato esperado."
    };
});

builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseSerilogRequestLogging();
app.UseCors("AllowPolicySecureDomains");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger(opt => opt.RouteTemplate = "swagger/{documentName}/swagger.json");
    app.UseSwaggerUI(opt => opt.SwaggerEndpoint("V1/swagger.json", "Task Management API V1"));
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
