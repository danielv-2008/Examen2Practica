using Examen2Practica.Data;
using Microsoft.EntityFrameworkCore;
using Examen2Practica.Interfaces;
using Examen2Practica.Services;
using Examen2Practica.Factories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var db = builder.Configuration.GetConnectionString("ConexionDB");
builder.Services.AddDbContext<BaseDatos>(options => options.UseSqlServer(db));
builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// ===== Swagger con JWT: botón "Authorize" para pegar el token =====
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.OpenApiInfo
    {
        Title = "API Tesis y Tutores",
        Version = "v1",
        Description = "Endpoints protegidos con JWT. Usá el botón **Authorize** para pegar el token."
    });

    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "Pegá el token que devuelve /api/auth/login, SIN el prefijo 'Bearer'",
        Type = Microsoft.OpenApi.SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.ParameterLocation.Header
    });

    options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        [new OpenApiSecuritySchemeReference("Bearer", document)] = []
    });

    // Candado solo en los endpoints que tienen [Authorize]
    options.OperationFilter<Examen2Practica.Swagger.SecurityRequirementsOperationFilter>();
});

// Inyección de dependencias de servicios
builder.Services.AddScoped<ITutor, TutorServices>();
builder.Services.AddScoped<ITesis, TesisServices>();
builder.Services.AddScoped<IAuthInterface, AuthServices>();

// ===== Autenticación JWT =====
var secretaJwt = builder.Configuration["Jwt:Secret"]
    ?? throw new InvalidOperationException("Falta Jwt:Secret en appsettings.json");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidateAudience = true,
            ValidAudience = builder.Configuration["Jwt:Audience"],
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretaJwt))
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

//Aplicar migraciones y sembrar datos
using (var scope = app.Services.CreateAsyncScope())
{
    var context = scope.ServiceProvider.GetRequiredService<BaseDatos>();
    context.Database.Migrate();
    TutoresFactories.Seed(context);
    TesisFactories.Seed(context);
    UsuariosFactories.Seed(context);
}

app.UseHttpsRedirection();

//Servir imágenes subidas (wwwroot/imagenes)
app.UseStaticFiles();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
