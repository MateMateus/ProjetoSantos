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
    options.RequireHttpsMetadata = false; // OK no Render (SSL via load balancer)
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

// ⚠️ ALTERE este domínio para o seu site no Netlify!
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

    // Política total para testes (opcional)
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// ==============================================================================
// 5. PIPELINE
// ==============================================================================
app.UseSwagger();
app.UseSwaggerUI();

// ⚠️ EM PRODUÇÃO USE: app.UseCors("AllowNetlify");
// Durante testes pode usar AllowAll:
app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// ==============================================================================
// 6. MIGRAÇÃO AUTOMÁTICA + SEED (ROLES)
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
// 7. CONFIGURA A PORTA DINÂMICA DO RENDER (OBRIGATÓRIO)
// ==============================================================================
var port = Environment.GetEnvironmentVariable("PORT") ?? "5101";
app.Urls.Add($"http://0.0.0.0:{port}");

app.Run();
