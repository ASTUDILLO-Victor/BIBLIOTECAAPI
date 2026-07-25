using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using BibliotecaApp.Application.Services;
using BibliotecaApp.Application.Services.Interfaces;
using BibliotecaApp.Domain.Interfaces;
using BibliotecaApp.Infrastructure.Data;
using BibliotecaApp.Infrastructure.Repositories;
using BibliotecaApp.Infrastructure.Services;
using Microsoft.AspNetCore.DataProtection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// PostgreSQL
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// JWT
var jwtKey = builder.Configuration["Jwt:Key"]!;
var jwtIssuer = builder.Configuration["Jwt:Issuer"]!;
var jwtAudience = builder.Configuration["Jwt:Audience"]!;

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultSignInScheme = "GoogleTemp";
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtIssuer,
        ValidAudience = jwtAudience,
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(jwtKey)),
        ClockSkew = TimeSpan.Zero
    };
})
.AddCookie("GoogleTemp", options =>
{
    options.Cookie.SameSite = SameSiteMode.Lax;
    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
    options.Cookie.Path = "/";
})
.AddGoogle(options =>
{
    options.ClientId = builder.Configuration["Google:ClientId"]!;
    options.ClientSecret = builder.Configuration["Google:ClientSecret"]!;
    options.CallbackPath = "/api/auth/google-callback";
    options.SignInScheme = "GoogleTemp";
    options.CorrelationCookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
    options.CorrelationCookie.SameSite = SameSiteMode.Lax;
    options.CorrelationCookie.Path = "/";
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("gestionar_autores", policy =>
        policy.RequireClaim("permiso", "gestionar_autores"));
    options.AddPolicy("gestionar_categorias", policy =>
        policy.RequireClaim("permiso", "gestionar_categorias"));
    options.AddPolicy("gestionar_libros", policy =>
        policy.RequireClaim("permiso", "gestionar_libros"));
    options.AddPolicy("ver_prestamos", policy =>
        policy.RequireClaim("permiso", "ver_prestamos"));
    options.AddPolicy("gestionar_prestamos", policy =>
        policy.RequireClaim("permiso", "gestionar_prestamos"));
});
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration["Redis:ConnectionString"];
    options.InstanceName = "BibliotecaApp_";
});
builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo("/app/keys"))
    .SetApplicationName("BibliotecaApp");

// Repositorios Auth
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IRolRepository, RolRepository>();
builder.Services.AddScoped<IUsuarioRolRepository, UsuarioRolRepository>();
builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
builder.Services.AddScoped<ITokenBlacklistRepository, TokenBlacklistRepository>();

// Repositorios Negocio
builder.Services.AddScoped<IAutorRepository, AutorRepository>();
builder.Services.AddScoped<ICategoriaRepository, CategoriaRepository>();
builder.Services.AddScoped<ILibroRepository, LibroRepository>();
builder.Services.AddScoped<ILibroAutorRepository, LibroAutorRepository>();
builder.Services.AddScoped<IPrestamoRepository, PrestamoRepository>();
builder.Services.AddScoped<IQrCodeGenerator, QrCodeGenerator>();

// Services Auth
builder.Services.AddScoped<IAuthService, AuthService>();

// Services Negocio
builder.Services.AddScoped<IAutorService, AutorService>();
builder.Services.AddScoped<ICategoriaService, CategoriaService>();
builder.Services.AddScoped<ILibroService, LibroService>();
builder.Services.AddScoped<IPrestamoService, PrestamoService>();

builder.Services.AddSignalR();
builder.Services.AddScoped<INotificadorService, BibliotecaApp.API.Hubs.NotificadorService>();

// Unit of Work
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("PermitirTodo", policy =>
    {
        policy.SetIsOriginAllowed(_ => true)
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});
builder.Services.AddScoped<ICacheService, RedisCacheService>();
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

app.UseMiddleware<BibliotecaApp.API.Middleware.ErrorHandlerMiddleware>();
app.UseHttpsRedirection();
app.UseCors("PermitirTodo");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapHub<BibliotecaApp.API.Hubs.NotificacionesHub>("/hubs/notificaciones");
app.Run();