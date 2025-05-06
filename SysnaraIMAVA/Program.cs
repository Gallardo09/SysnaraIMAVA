using FastReport.DataVisualization.Charting;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using OfficeOpenXml;
using SysnaraIMAVA.Models;


var builder = WebApplication.CreateBuilder(args);

// Configurar EPPlus para uso no comercial
//ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

// Add services to the container.
builder.Services.AddControllersWithViews();

//****************************** Conexion a la base de datos DBSMILE ***************************
builder.Services.AddDbContext<DbimavaContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("conexion"))
);

builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 10 * 1024 * 1024; // Límite de 10 MB para subir una foto en MATRICULA
});

// Habilitar sesiones (LOGIN)
builder.Services.AddSession(options =>
{
    // Configura el tiempo de expiración de la sesión si es necesario
    // options.IdleTimeout = TimeSpan.FromMinutes(60);  // Tiempo de expiración de la sesión (opcional)
    options.Cookie.HttpOnly = true;  // Cookies solo accesibles por el servidor
    options.Cookie.IsEssential = true;  // Hacer que la cookie de sesión sea esencial
});

// FastReport (asegúrate de tener instalado el paquete)
builder.Services.AddFastReport();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

// ** Asegúrate de añadir esto aquí para habilitar el uso de sesiones **
app.UseSession();  // Aquí agregas el middleware de sesión (LOGIN)

app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
