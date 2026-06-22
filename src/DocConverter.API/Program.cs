using System.Text;
using DocConverter.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer; // Nuevo
using Microsoft.IdentityModel.Tokens; // Nuevo

var builder = WebApplication.CreateBuilder(args);

// --- 1. CONFIGURACIÓN DE SERVICIOS (Dependency Injection) ---
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Capas de Clean Architecture
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication();

// Configuración de Autenticación JWT
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["Secret"] ?? throw new InvalidOperationException("JWT Secret is missing.");

builder.Services.AddAuthentication(options =>
{
    // Forzamos a que por defecto todo el sistema intente validar tokens en formato JWT Bearer
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true, // Verifica que el token no haya expirado
        ValidateIssuerSigningKey = true, // Valida la firma del token usando nuestra clave secreta
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
    };
});

var app = builder.Build();

// --- 2. PIPELINE DE PETICIONES (Middlewares) ---
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// CRÍTICO: El orden de estos dos middlewares importa. 
// Primero el sistema detecta quién es el usuario (Authenticate) y luego si tiene permiso (Authorize)
app.UseAuthentication(); 
app.UseAuthorization();

app.MapControllers();

app.Run();