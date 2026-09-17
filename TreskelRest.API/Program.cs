using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json.Serialization;
using TreskelRest.API.Exceptions;
using TreskelRest.Biz;
using TreskelRest.Biz.Interfaces;
using TreskelRest.Biz.Procesos;
using TreskelRest.Biz.Servicios;
using TreskelRest.Data;

var builder = WebApplication.CreateBuilder(args);

// 1. Definir la política
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        });
});

// ==========================================
// NUEVO: CONFIGURACIÓN DEL SERVICIO JWT
// ==========================================
var jwtKey = builder.Configuration["Jwt:Key"]!;
builder.Services.AddAuthentication(options => {
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options => {
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
    };
});

// Add services to the container.

builder.Services
    .AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(
            new JsonStringEnumConverter());
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ===================================================
// ✅ NUEVO: REGISTRO DE MANEJO DE EXCEPCIONES GLOBAL
// ===================================================
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails(
    options => options.CustomizeProblemDetails = context =>
    {
        context.ProblemDetails.Extensions.Remove("exception");
        context.ProblemDetails.Extensions.Remove("headers");
        context.ProblemDetails.Extensions.Remove("routeValues");
        context.ProblemDetails.Extensions.Remove("endpoint");
    }    
);


builder.Services.AddScoped<ISriWebService, SriWebService>();
builder.Services.AddScoped<ISendEmail, SendEmail>();
builder.Services.AddScoped<IWhatsapp, TreskelRest.Biz.Models.Responses.Whatsapp>();
builder.Services.AddScoped<ISriComprobanteService, SriComprobanteService>();

// Agrego singleton para mongodb
builder.Services.AddSingleton<TreskelFilesContext>();
builder.Services.AddScoped<ITreskelFilesRepository, TreskelFilesRepository>();

// Agrega singleton para acceso a base de datos
builder.Services.AddSingleton<TreskelContext>();
builder.Services.AddScoped<ITreskelRepository, TreskelRepository>();


// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Para usar MediatorR
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(typeof(BizAssemblyMarker).Assembly));

// ===================================================
// AutoMapper
// ===================================================
builder.Services.AddAutoMapper(cfg =>
{
    // Esto buscará automáticamente todas las clases que hereden de 'Profile'
    // en el ensamblado donde se encuentra la clase Program (tu API)
    cfg.AddMaps(typeof(Program).Assembly);

    // Si tus perfiles están en la capa BIZ, añade también ese ensamblado:
    cfg.AddMaps(typeof(TreskelRest.Biz.Models.Requests.MappingProfile).Assembly);
});

builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.MapOpenApi();
}
else {
    app.UseExceptionHandler("/error-global-ficticio");
}

// Habilitar CORS en el pipeline si tu app de Angular lo requiere
app.UseCors("AllowAll");


// ===================================================
// NUEVO: UBICACIÓN DE LA AUTENTICACIÓN
// ===================================================
app.UseAuthentication();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
