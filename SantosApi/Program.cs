using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SantosApi.Data;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ==============================================================================
// 1. CONFIGURAÇÃO DO BANCO DE DADOS (PostgreSQL - Render)
// ==============================================================================
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// ==============================================================================
// 2. CONFIGURAÇÃO DO IDENTITY (Sistema de Login)
// ==============================================================================
builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

// ==============================================================================
// 3. CONFIGURAÇÃO DA AUTENTICAÇÃO (JWT)
// ==============================================================================
var key = Encoding.ASCII.GetBytes(
    builder.Configuration["JwtSettings:Key"]
    ?? "chave_super_secreta_padrao_para_nao_quebrar_se_faltar_config"
);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false; // Render usa proxy SSL
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});

// ==============================================================================
// 4. CONFIGURAÇÃO DO CORS (Netlify + Render)
// ==============================================================================
var netlifyOrigin = "https://seu-site.netlify.app";

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowNetlify", policy =>
    {
        policy.WithOrigins(netlifyOrigin)
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });

    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// ==============================================================================
// 5. CONTROLLERS + SWAGGER (COM JWT ATIVADO)
// ==============================================================================
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "SantosApi", Version = "v1" });

    // Definição correta para Bearer JWT (use Http + scheme "bearer")
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Description = "Autenticação JWT usando o esquema Bearer.\n\nDigite: Bearer {seu_token_jwt}",
        Name = "Authorization",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http, // <- importante
        Scheme = "bearer",
        BearerFormat = "JWT"
    });

    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "bearer",
                Name = "Bearer",
                In = Microsoft.OpenApi.Models.ParameterLocation.Header,
            },
            new string[] {}
        }
    });
});


var app = builder.Build();

// ==============================================================================
// 6. PIPELINE
// ==============================================================================
app.UseSwagger();
app.Use(async (context, next) =>
{
    if (context.Request.Headers.ContainsKey("X-Forwarded-Proto") &&
        context.Request.Headers["X-Forwarded-Proto"] == "http")
    {
        var httpsUrl = "https://" + context.Request.Host + context.Request.Path + context.Request.QueryString;
        context.Response.Redirect(httpsUrl);
        return;
    }

    await next();
});

app.UseSwaggerUI();

app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// ==============================================================================
// 7. MIGRAÇÃO AUTOMÁTICA + SEED (ROLES)
// ==============================================================================
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();

    try
    {
        logger.LogInformation("Iniciando Migração do Banco de Dados...");

        var context = services.GetRequiredService<AppDbContext>();
        context.Database.Migrate();

        logger.LogInformation("Migração concluída com sucesso!");

        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        string[] roleNames = { "Admin", "User" };

        foreach (var roleName in roleNames)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                await roleManager.CreateAsync(new IdentityRole(roleName));
                logger.LogInformation($"Role '{roleName}' criada.");
            }
        }
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "ERRO durante a migração ou seed.");
    }
}

// ==============================================================================
// 8. CONFIGURA A PORTA DINÂMICA DO RENDER (OBRIGATÓRIO)
// ==============================================================================
var port = Environment.GetEnvironmentVariable("PORT") ?? "5101";
app.Urls.Add($"http://0.0.0.0:{port}");

app.Run();
