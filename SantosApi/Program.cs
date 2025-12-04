using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SantosApi.Data;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ==============================================================================
// 1. CONFIGURAÇÃO DO BANCO DE DADOS (PostgreSQL)
// ==============================================================================
// O Render injeta a Connection String automaticamente na variável de ambiente
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
var key = Encoding.ASCII.GetBytes(builder.Configuration["JwtSettings:Key"] ?? "chave_super_secreta_padrao_para_nao_quebrar_se_faltar_config"); 

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false; // Em produção no Render, o SSL é gerido pelo load balancer, mas pode deixar false se necessário
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false, // Em produção, considere setar para true e definir o Issuer
        ValidateAudience = false
    };
});

// ==============================================================================
// 4. CONFIGURAÇÃO DO CORS E OUTROS SERVIÇOS
// ==============================================================================
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder => builder
        .AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader());
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// ==============================================================================
// 5. PIPELINE DE EXECUÇÃO (Middleware)
// ==============================================================================

// Swagger
// Dica: Mantive fora do "if Development" para você conseguir testar no Render
app.UseSwagger();
app.UseSwaggerUI();

app.UseCors("AllowAll"); 

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// ==============================================================================
// 6. BLOCO DE INICIALIZAÇÃO DO BANCO (A CORREÇÃO ESTÁ AQUI)
// ==============================================================================
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();

    try 
    {
        logger.LogInformation("Iniciando Migração do Banco de Dados...");

        // PASSO CRUCIAL: Aplica as Migrations
        // Isso cria as tabelas (AspNetUsers, AspNetRoles) se elas não existirem.
        var context = services.GetRequiredService<AppDbContext>();
        context.Database.Migrate(); 
        
        logger.LogInformation("Migração concluída com sucesso!");

        // PASSO DE SEED: Cria perfis iniciais
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        string[] roleNames = { "Admin", "User" };
        
        foreach (var roleName in roleNames)
        {
            // Agora é seguro chamar RoleExistsAsync porque o Migrate() acima garantiu que a tabela existe
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                await roleManager.CreateAsync(new IdentityRole(roleName));
                logger.LogInformation($"Role '{roleName}' criada com sucesso.");
            }
        }
    }
    catch (Exception ex)
    {
        // Log detalhado para aparecer no console do Render se der erro
        logger.LogError(ex, "ERRO CRÍTICO durante a migração ou seed do banco de dados.");
    }
}

app.Run();