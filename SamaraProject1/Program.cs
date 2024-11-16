using Microsoft.EntityFrameworkCore;
using SamaraProject1.Servicios.Contrato;
using SamaraProject1.Servicios.Implementacion;

using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

using SamaraProject1.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<SamaraMarketContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("SamaraMarketDatabase"));
});


builder.Services.AddScoped<IAdministradorService, AdministradorService>();
builder.Services.AddScoped<IEmprendedorService, EmprendedorService>();
builder.Services.AddScoped<IProductoService, ProductoService>();
builder.Services.AddScoped<IStandService, StandService>();
builder.Services.AddScoped<IEventoService, EventoService>();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(option =>
    {
        option.LoginPath = "/Inicio/IniciarSesion";
        option.ExpireTimeSpan = TimeSpan.FromMinutes(20);

    });

//builder.Services.AddCors(options =>
//{
 //   options.AddPolicy("AllowAll", policy =>
   // {
     //   policy.AllowAnyOrigin()  // Permite solicitudes desde cualquier origen
       //       .AllowAnyHeader()  // Permite cualquier cabecera
         //     .AllowAnyMethod();  // Permite cualquier método HTTP (GET, POST, etc.)
    //});
//});

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

app.UseHttpsRedirection();

app.UseStaticFiles(new StaticFileOptions
{
    OnPrepareResponse = ctx =>
    {
        ctx.Context.Response.Headers.Add("Cache-Control", "public,max-age=600");
    }
});

app.UseRouting();

//app.UseCors("AllowAll");

app.UseAuthentication();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
    

app.Run();
