using System.Text;
using System.Text.Json.Serialization;
using APICatalogo.Context;
using APICatalogo.DTOs.Mappings;
using APICatalogo.Filters;
using APICatalogo.Models;
using APICatalogo.Repositories;
using APICatalogo.Repositories.Interfaces;
using APICatalogo.Services;
using APICatalogo.Services.Auth;
using APICatalogo.Services.Auth.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace CrossCutting;

public static class IndepencyInjection
{
    public static IServiceCollection ConfigureIoc(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddEndpointsApiExplorer();
        services.AddScoped();
        services.AddJwt(configuration);
        services.AddAutoMapper();
        services.AddAuthorization();
        services.AddContext(configuration);
        services.AddControllers();
        services.AddEntity();
        services.AddSwaggerGen();
        
        return services;
    }
    public static void AddScoped(this IServiceCollection services)
    {
        services.AddScoped<ApiLogginFilter>();
        services.AddScoped<ICategoriaRepository, CategoriaRepository>();
        services.AddScoped<IProdutoRepository, ProdutoRepository>();
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<IUnitOfWork, UnityOfWork>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<ILogger, Logger<Program>>();
        services.AddScoped<IAuth, Auth>();
    }
    public static void AddJwt(this IServiceCollection services, IConfiguration configuration)
    {
        var secretKey = configuration["JWT:SecretKey"] ?? throw new ArgumentException("Invalid secret key!!");
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options =>
        {
            options.SaveToken = true;
            options.RequireHttpsMetadata = false;
            options.TokenValidationParameters = new TokenValidationParameters()
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ClockSkew = TimeSpan.Zero,
                ValidAudience = configuration["JWT:ValidAudience"],
                ValidIssuer = configuration["JWT:ValidIssuer"],
                IssuerSigningKey = new SymmetricSecurityKey(
                                   Encoding.UTF8.GetBytes(secretKey))
            };
        });
    }
    public static void AddAutoMapper(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(ProdutoDTOMappingProfile));
    }
    public static void AddAuthorization(this IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
            options.AddPolicy("SuperAdminOnly", policy => policy.RequireRole("Admin").RequireClaim("id", "LucasCamilo"));
            options.AddPolicy("UserOnly", policy => policy.RequireRole("User"));
            options.AddPolicy("ExclusiveOnly", policy => policy.RequireAssertion(context => context.User.HasClaim(claim =>
            claim.Type == "id" && claim.Value == "LucasCamilo") || context.User.IsInRole("SuperAdmin")));
        });
    }
    public static void AddContext(this IServiceCollection services, IConfiguration configuration)
    {
        string mySqlConnection = configuration.GetConnectionString("DefaultConnection")!;
        services.AddDbContext<AppDbContext>(options => options.UseMySql(mySqlConnection, ServerVersion.AutoDetect(mySqlConnection)));
    }
    public static void AddControllers(this IServiceCollection services)
    {
        services.AddControllers(options => { options.Filters.Add(typeof(ApiExceptionFilter)); }).AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        }).AddNewtonsoftJson();
    }
    public static void AddEntity(this IServiceCollection services)
    {
        services.AddIdentity<ApplicationUser, IdentityRole>()
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();
    }
    public static void AddSwaggerGen(this IServiceCollection services)
    {
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "apicatalogo", Version = "v1" });
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
            {
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Bearer JWT ",
            });
            c.AddSecurityRequirement(new OpenApiSecurityRequirement { { new OpenApiSecurityScheme { Reference = new OpenApiReference
            { Type = ReferenceType.SecurityScheme, Id = "Bearer" } }, new string[] {} } });
        });
    }
}
