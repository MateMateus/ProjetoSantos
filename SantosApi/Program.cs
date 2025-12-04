using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SantosApi.Data;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// 1. Configura o Banco de Dados (PostgreSQL)
// O Render injeta a Connection String automaticamente na variável de ambiente
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// 2. Configura o sistema de Login (Identity)
builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

// 3. Configura a Autenticação (JWT)
// Nota: Em produção, garanta que "JwtSettings:Key" esteja nas variáveis de ambiente do Render também
var key = Encoding.ASCII.GetBytes(builder.Configuration["JwtSettings:Key"] ?? "chave_super_secreta_padrao_para_nao_quebrar_se_faltar_config"); 
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});

// 4. Configura o CORS (Permite acesso externo)
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

// --- ÁREA DE EXECUÇÃO ---

// Swagger (Pode manter no if Development ou tirar o if para ver no Render)
if (app.Environment.IsDevelopment()) 
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
// Se quiser ver o Swagger no Render (Produção), descomente as linhas abaixo e comente o bloco if acima:
// app.UseSwagger();
// app.UseSwaggerUI();

app.UseCors("AllowAll"); 

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// ** BLOCO DE INICIALIZAÇÃO DO BANCO **
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    try 
    {
        // PASSO 1: Aplica as Migrations (Cria as tabelas AspNetRoles, AspNetUsers, etc.)
        // Isso resolve o erro "relation does not exist"
        var context = services.GetRequiredService<AppDbContext>();
        context.Database.Migrate(); 

        // PASSO 2: Seed Automático (Cria os perfis Admin e User)
        // Agora seguro de rodar porque as tabelas já existem
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        string[] roleNames = { "Admin", "User" };
        foreach (var roleName in roleNames)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                await roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }
    }
    catch (Exception ex)
    {
        // Log de erro caso algo falhe na migração (útil para debug no Render)
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Ocorreu um erro ao migrar ou semear o banco de dados.");
    }
}

app.Run();