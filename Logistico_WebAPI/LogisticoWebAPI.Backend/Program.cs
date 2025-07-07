using LogisticoWebAPI.Backend.Data;
using LogisticoWebAPI.Backend.Helpers;
using LogisticoWebAPI.Backend.Repositories.Implementations;
using LogisticoWebAPI.Backend.Repositories.Interfaces;
using LogisticoWebAPI.Backend.UnitsOfWork.Implementations;
using LogisticoWebAPI.Backend.UnitsOfWork.Interfaces;
using LogisticoWebAPI.Shared.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using System.Text.Json.Serialization;

namespace LogisticoWebAPI.Backend
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Configurar las variables desde GitHub Secrets o variables de entorno
            var connectionString = Environment.GetEnvironmentVariable("LOGISTICO_DATABASE_CONNECTION")
                ?? builder.Configuration.GetConnectionString("LogisticoDatabase")
                ?? throw new InvalidOperationException("Database connection string not found.");

            var azureStorageConnection = Environment.GetEnvironmentVariable("AZURE_STORAGE_CONNECTION")
                ?? builder.Configuration.GetConnectionString("AzureStorage")
                ?? throw new InvalidOperationException("Azure Storage connection string not found.");

            var jwtKey = Environment.GetEnvironmentVariable("JWT_SECRET_KEY")
                ?? builder.Configuration["jwtKey"]
                ?? throw new InvalidOperationException("JWT key not found.");

            var mailPassword = Environment.GetEnvironmentVariable("MAIL_SECRET")
                ?? builder.Configuration["Mail:Password"]
                ?? throw new InvalidOperationException("Mail password not found.");

            // Agregar las configuraciones al contenedor de dependencias
            builder.Configuration["ConnectionStrings:LogisticoDatabase"] = connectionString;
            builder.Configuration["ConnectionStrings:AzureStorage"] = azureStorageConnection;
            builder.Configuration["jwtKey"] = jwtKey;
            builder.Configuration["Mail:Password"] = mailPassword;

            // Add services to the container.
            builder.Services.AddControllers().AddJsonOptions(x =>
                x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);
            builder.Services.AddEndpointsApiExplorer();

            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Logistico API",
                    Version = "v1"
                });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = @"JWT Authorization header using the Bearer scheme. <br /> <br />
                          Enter 'Bearer' [space] and then your token in the text input below.<br /> <br />
                          Example: 'Bearer 12345abcdef'<br /> <br />",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {
                        {
                            new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "Bearer"
                                },
                        Scheme = "oauth2",
                        Name = "Bearer",
                        In = ParameterLocation.Header,
                            },
                            new List<string>()
                        }
                });
            });

            builder.Services.AddDbContext<DataContext>(options =>
                options.UseSqlServer(connectionString));
            builder.Services.AddTransient<SeedDb>();
            builder.Services.AddScoped<IFileStorage, FileStorage>();
            builder.Services.AddScoped<IMailHelper, MailHelper>();

            builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            builder.Services.AddScoped(typeof(IGenericUnitOfWork<>), typeof(GenericUnitOfWork<>));

            builder.Services.AddScoped<IStatesRepository, StatesRepository>();
            builder.Services.AddScoped<IStatesUnitOfWork, StatesUnitOfWork>();

            builder.Services.AddScoped<ICitiesRepository, CitiesRepository>();
            builder.Services.AddScoped<ICitiesUnitOfWork, CitiesUnitOfWork>();

            builder.Services.AddScoped<IUsersRepository, UsersRepository>();
            builder.Services.AddScoped<IUsersUnitOfWork, UsersUnitOfWork>();

            builder.Services.AddIdentity<User, IdentityRole>(x =>
            {
                x.Tokens.AuthenticatorTokenProvider = TokenOptions.DefaultAuthenticatorProvider;
                x.SignIn.RequireConfirmedEmail = true;
                x.User.RequireUniqueEmail = true;
                x.Password.RequireDigit = false;
                x.Password.RequiredUniqueChars = 0;
                x.Password.RequireLowercase = false;
                x.Password.RequireNonAlphanumeric = false;
                x.Password.RequireUppercase = false;
                x.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                x.Lockout.MaxFailedAccessAttempts = 3;
                x.Lockout.AllowedForNewUsers = true;

            })
                .AddEntityFrameworkStores<DataContext>()
                .AddDefaultTokenProviders();

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(x => x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
                    ClockSkew = TimeSpan.Zero
                });

            var app = builder.Build();

            SeedData(app);

            void SeedData(WebApplication app)
            {
                var scopedFactory = app.Services.GetService<IServiceScopeFactory>();

                using var scope = scopedFactory!.CreateScope();
                var service = scope.ServiceProvider.GetService<SeedDb>();
                service!.SeedAsync().Wait();
            }

            app.UseCors(c => c
                .AllowAnyMethod()
                .AllowAnyHeader()
                .SetIsOriginAllowed(origin => true)
                .AllowCredentials());

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger(c =>
                    {
                        c.OpenApiVersion = Microsoft.OpenApi.OpenApiSpecVersion.OpenApi3_0;
                    });
                app.UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint("/swagger/v1/swagger.json", "LogisticoWebAPI.Backend v1");
                    options.RoutePrefix = "swagger";
                });
            }

            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();

            app.Run();
        }
    }
}