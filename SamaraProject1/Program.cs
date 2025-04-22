using Microsoft.EntityFrameworkCore;
using SamaraProject1.Servicios.Contrato;
using SamaraProject1.Servicios.Implementacion;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using SamaraProject1.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using SamaraProject1.Servicios;
using Microsoft.Net.Http.Headers;
using System.IO.Compression;
using Microsoft.AspNetCore.ResponseCompression;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("/etc/secrets/appsettings.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<SamaraMarketContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("SamaraMarketDatabase"));
});

builder.Services.AddControllersWithViews();
builder.Services.AddScoped<IAdministradorService, AdministradorService>();
builder.Services.AddScoped<IEmprendedorService, EmprendedorService>();
builder.Services.AddScoped<IProductoService, ProductoService>();
builder.Services.AddScoped<IStandService, StandService>();
builder.Services.AddScoped<IEventoService, EventoService>();
builder.Services.AddScoped<ICarouselService, CarouselService>();
builder.Services.AddScoped<IImageService, ImageService>();

// Agregar HttpClient para el controlador de video
builder.Services.AddHttpClient();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(option =>
    {
        option.LoginPath = "/Inicio/IniciarSesion";
        option.ExpireTimeSpan = TimeSpan.FromMinutes(20);
    });

builder.Services.AddScoped<IAdministradorService, AdministradorService>();
builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("SmtpSettings"));
builder.Services.AddLogging();

// Agregar compresión de respuesta
builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
    options.Providers.Add<BrotliCompressionProvider>();
    options.Providers.Add<GzipCompressionProvider>();
    options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
        new[] { "image/svg+xml", "application/javascript", "text/css" });
});

builder.Services.Configure<BrotliCompressionProviderOptions>(options =>
{
    options.Level = CompressionLevel.Fastest;
});

builder.Services.Configure<GzipCompressionProviderOptions>(options =>
{
    options.Level = CompressionLevel.Fastest;
});

builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add(
        new ResponseCacheAttribute
        {
            NoStore = true,
            Location = ResponseCacheLocation.None,
        });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

// Usar compresión de respuesta
app.UseResponseCompression();

app.UseHttpsRedirection();

// Configuración mejorada de archivos estáticos con caché diferenciada por tipo de archivo
app.UseStaticFiles(new StaticFileOptions
{
    OnPrepareResponse = ctx =>
    {
        string path = ctx.File.PhysicalPath;

        // Caché más larga para imágenes y otros recursos estáticos
        if (path.EndsWith(".jpg") || path.EndsWith(".jpeg") || path.EndsWith(".png") ||
            path.EndsWith(".gif") || path.EndsWith(".webp") || path.EndsWith(".svg") ||
            path.EndsWith(".ico") || path.EndsWith(".woff") || path.EndsWith(".woff2"))
        {
            // 7 días para imágenes y fuentes
            ctx.Context.Response.Headers[HeaderNames.CacheControl] = "public,max-age=604800";
        }
        else if (path.EndsWith(".css") || path.EndsWith(".js"))
        {
            // 1 día para CSS y JavaScript
            ctx.Context.Response.Headers[HeaderNames.CacheControl] = "public,max-age=86400";
        }
        else
        {
            // 10 minutos para otros recursos
            ctx.Context.Response.Headers[HeaderNames.CacheControl] = "public,max-age=600";
        }
    }
});

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
