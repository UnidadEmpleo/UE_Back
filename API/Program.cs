using API.Persistence;
using API.Seguridad.Application.Core;
using API.Seguridad.Application.Core.Audit;
using API.Seguridad.Application.Seguridad.Grupos.Queries;
using API.Seguridad.Application.Seguridad.Grupos.Validators;
using API.Seguridad.Domain.Seguridad;
using API.Seguridad.Infrastructure;
using API.Seguridad.Infrastructure.Authorization;
using API.Seguridad.Infrastructure.Authorization;
using API.Seguridad.Middleware;
using API.UnidadEmpleo.DTO;
using API.UnidadEmpleo.Persistence;
using API.UnidadEmpleo.Services;
using Azure.Identity;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Graph;
using Microsoft.Identity.Web;
using Microsoft.IdentityModel.Tokens;
using QuestPDF.Infrastructure;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.MSSqlServer;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using System.Text.Json.Serialization;


var builder = WebApplication.CreateBuilder(args);

// Configura Microsoft Graph
builder.Services.AddSingleton(provider =>
{
    var clientId = builder.Configuration["AzureAd:ClientId"];
    var tenantId = builder.Configuration["AzureAd:TenantId"];
    var clientSecret = builder.Configuration["AzureAd:ClientSecret"];

    var options = new TokenCredentialOptions
    {
        AuthorityHost = AzureAuthorityHosts.AzurePublicCloud
    };

    var clientSecretCredential = new ClientSecretCredential(
        tenantId, clientId, clientSecret, options);

    // Define los scopes necesarios
    var scopes = new[] { "https://graph.microsoft.com/.default" };

    return new GraphServiceClient(clientSecretCredential, scopes);
});

//Este servicio quita referencias circulares
builder.Services.AddControllers().AddJsonOptions(x =>
                x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

builder.Services.AddSingleton<IGraphManager, GraphManager>();

builder.Services.AddScoped<ICorporacionContextAccessor, CorporacionContextAccessor>();

// ***LINEA QUE SE AGREGA
builder.Services.AddScoped<UnidadEmpleoDBContextFactoryInterface, UnidadEmpleoDbContextFactory>();

// Add DbContext
// ***CONTEXTO AGREGADO
builder.Services.AddDbContext<UnidadEmpleoDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("UnidadEmpleoConnection"),
        sqlOptions =>
        {
            sqlOptions.EnableRetryOnFailure(
                maxRetryCount: 3,
                maxRetryDelay: TimeSpan.FromSeconds(15),
                errorNumbersToAdd: null);
        })
    //.EnableDetailedErrors()
    //.EnableSensitiveDataLogging()
    //.LogTo(Console.WriteLine, LogLevel.Debug)
    );

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlOptions =>
        {
            sqlOptions.EnableRetryOnFailure(
                maxRetryCount: 3,
                maxRetryDelay: TimeSpan.FromSeconds(15),
                errorNumbersToAdd: null);
        })
    //.EnableDetailedErrors()
    //.EnableSensitiveDataLogging()
    //.LogTo(Console.WriteLine, LogLevel.Debug)
    );

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day) // Log en archivo diario
    .WriteTo.MSSqlServer(
    connectionString: builder.Configuration.GetConnectionString("DefaultConnection"),
    sinkOptions: new MSSqlServerSinkOptions
    {
        TableName = "Logs",
        AutoCreateSqlTable = false // Crea autom�ticamente la tabla si no existe
    },
    restrictedToMinimumLevel: LogEventLevel.Information, // Nivel m�nimo de log
    columnOptions: new ColumnOptions
    {
        AdditionalColumns = new List<SqlColumn>
        {
            new("UserId", System.Data.SqlDbType.NVarChar),
            new("UserName", System.Data.SqlDbType.NVarChar),
            new("RequestPath", System.Data.SqlDbType.NVarChar),
            new("RequestMethod", System.Data.SqlDbType.NVarChar),
            new("StatusCode", System.Data.SqlDbType.Int),
            new("Elapsed", System.Data.SqlDbType.Float)
        }
    }, formatProvider: null) // Usa el formato JSON por defecto
    .CreateLogger();

builder.Host.UseSerilog(); // Reemplaza el logger por defecto con Serilog

builder.Services.AddIdentity<AppUserIdentity, AppIdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

// Autenticaci�n combinada (JWT + Azure AD)
builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = "BearerOrAzureAD";
        options.DefaultChallengeScheme = "BearerOrAzureAD";
    })
    .AddPolicyScheme("BearerOrAzureAD", "Bearer or AzureAD", options =>
    {
        options.ForwardDefaultSelector = context =>
        {
            var authHeader = context.Request.Headers.Authorization.ToString();
            if (authHeader?.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase) == true)
            {
                var token = authHeader["Bearer ".Length..].Trim();
                var handler = new JwtSecurityTokenHandler();
                if (handler.CanReadToken(token))
                {
                    var jwt = handler.ReadJwtToken(token);
                    var iss = jwt.Issuer ?? string.Empty;
                    if (iss.Contains("login.microsoftonline.com", StringComparison.OrdinalIgnoreCase) ||
                        iss.Contains("sts.windows.net", StringComparison.OrdinalIgnoreCase))
                    {
                        return "AzureAD";
                    }
                }
            }
            return JwtBearerDefaults.AuthenticationScheme; // "Bearer" (local)
        };
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SecretKey"]!)
            ),
        };

        options.Events = new JwtBearerEvents
        {
            OnTokenValidated = context =>
            {
                var identity = (ClaimsIdentity)context.Principal!.Identity!;
                identity.AddClaim(new Claim("auth_source", "local_jwt"));
                return Task.CompletedTask;
            }
        };
    })
    .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd"), jwtBearerScheme: "AzureAD")
    .EnableTokenAcquisitionToCallDownstreamApi()
    .AddInMemoryTokenCaches();

builder.Services.Configure<JwtBearerOptions>("AzureAD", options =>
{
    // Accept either the App ID URI or the ClientId as audience
    options.TokenValidationParameters.ValidAudiences = new[]
    {
        builder.Configuration["AzureAd:Audience"],
        builder.Configuration["AzureAd:ClientId"]
    };

    if (options.Backchannel == null)
    {
        options.Backchannel = new HttpClient();
    }
    options.Backchannel.Timeout = TimeSpan.FromSeconds(10);

    options.Events = new JwtBearerEvents
    {
        OnTokenValidated = context =>
        {
            var identity = (ClaimsIdentity)context.Principal!.Identity!;
            identity.AddClaim(new Claim("auth_source", "azure_ad"));
            return Task.CompletedTask;
        }
    };
});

var combinedPolicy = new AuthorizationPolicyBuilder()
    .RequireAuthenticatedUser()
    .Build();

builder.Services.AddControllers(options =>
{
    options.Filters.Add(new AuthorizeFilter(combinedPolicy));
});

// Leer origenes permitidos desde configuración
var allowedOrigins = builder.Configuration.GetSection("AllowedOrigins").Get<string[]>()
    ?? ["http://localhost:8080", "http://192.168.186.1:8080"];

builder.Services.AddCors(options => {
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              //.WithOrigins(allowedOrigins); // Permite solicitudes desde este origen
              .AllowAnyHeader() // Permite cualquier encabezado
              .AllowAnyMethod() ; // Permite el uso de credenciales (cookies, encabezados de autorizaci n, etc.)
              
    });
});

builder.Services.AddMemoryCache();

builder.Services.AddMediatR(x => {
    x.RegisterServicesFromAssemblyContaining<GetGrupoList.Handler>();
    x.AddOpenBehavior(typeof(ValidationBehavior<,>));
});

builder.Services.AddHttpContextAccessor();
builder.Services.AddAutoMapper(typeof(MappingProfiles).Assembly);
//builder.Services.AddAutoMapper(typeof(MapperUnidadEmpleo).Assembly);

builder.Services.AddValidatorsFromAssemblyContaining<CreateGrupoValidator>();
builder.Services.AddTransient<ExceptionMiddleware>();
builder.Services.AddScoped<TokenService>();
builder.Services.AddScoped<IAuditService, AuditService>();
builder.Services.AddScoped<UsuarioCacheService>();

// *** Servicios agregados
builder.Services.AddScoped<ILists, ListsServices>();


builder.Services.AddAuthorization(options =>
{
    // Crear políticas dinámicas para cada tipo de rol
    var tipoRoles = Enum.GetValues<TipoRoles>();
    
    foreach (var tipoRol in tipoRoles)
    {
        options.AddPolicy($"TipoRol_{(int)tipoRol}", policy =>
            policy.Requirements.Add(new TipoRolRequirement(tipoRol)));
    }
    
    // Políticas para combinaciones de tipos de rol
    
    options.AddPolicy($"TipoRol_{(int)TipoRoles.Administrador}_{(int)TipoRoles.Administrador}", 
        policy => policy.Requirements.Add(new TipoRolRequirement(TipoRoles.Administrador, TipoRoles.Administrador)));
    
    options.AddPolicy($"TipoRol_{(int)TipoRoles.Medico}_{(int)TipoRoles.Medico}", 
        policy => policy.Requirements.Add(new TipoRolRequirement(TipoRoles.Medico, TipoRoles.Medico)));
    
    options.AddPolicy($"TipoRol_{(int)TipoRoles.AtencionRegistro}_{(int)TipoRoles.AtencionRegistro}_{(int)TipoRoles.AtencionRegistro}", 
        policy => policy.Requirements.Add(new TipoRolRequirement(TipoRoles.AtencionRegistro, TipoRoles.AtencionRegistro, TipoRoles.AtencionRegistro)));
    
});

// Registrar el handler de autorización (usando claims, no consulta BD)
builder.Services.AddSingleton<IAuthorizationHandler, TipoRolAuthorizationHandler>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));

    options.CustomSchemaIds(type => type.FullName?.Replace("+", "."));
});

builder.Services.AddSingleton<IAntivirusScanner>(sp => new ClamAntivirusScanner("localhost", 3310));

// Add QuestPDF license configuration (use Community license for non-commercial)
QuestPDF.Settings.License = LicenseType.Community;

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

// Configuraci�n del pipeline de middlewares
app.UseRouting();

app.UseCors();

app.UseSerilogRequestLogging(options =>
{
    options.MessageTemplate = "Handled {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";
});

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<ExceptionMiddleware>();
app.UseMiddleware<CorporacionMiddleware>();

app.MapControllers();

// Seed data
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    try
    {
        // ***SE AGREGO EL INITIALIZER NO FUNCIONA.
        var contextUE = services.GetRequiredService<UnidadEmpleoDbContext>();
        await contextUE.Database.MigrateAsync();
        await UnidadEmpleoDbInitializer.SeedData(contextUE);

        var context = services.GetRequiredService<AppDbContext>();
        var userManager = services.GetRequiredService<UserManager<AppUserIdentity>>();
        var roleManager = services.GetRequiredService<RoleManager<AppIdentityRole>>();
        var configuration = services.GetRequiredService<IConfiguration>();

        await context.Database.MigrateAsync();

        await DbInitializer.SeedData(context, userManager, roleManager, configuration);
    }
    catch (System.Exception ex)
    {       
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while migrating or seeding the database."); 
    }
}

app.Run();
