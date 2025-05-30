using Microsoft.EntityFrameworkCore;
using theburycode.Models;
using theburycode.Services;
using theburycode.Services.Shared;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Configurar Entity Framework
builder.Services.AddDbContext<TheBuryCodeContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ERPConnection")));

// Servicios
builder.Services.AddScoped<IClienteService, ClienteService>();
builder.Services.AddScoped<IProductoService, ProductoService>();
builder.Services.AddScoped<IVentaService, VentaService>();

// Servicios compartidos
builder.Services.AddScoped<IStockService, StockService>();
builder.Services.AddScoped<IPrecioCalculatorService, PrecioCalculatorService>();
builder.Services.AddScoped<IComprobanteService, ComprobanteService>();
builder.Services.AddScoped<IFormaPagoService, FormaPagoService>();
builder.Services.AddScoped<ISearchService, SearchService>();

// Agregar controllers de API
builder.Services.AddControllers();

var app = builder.Build(); // Aseg�rate de que esta l�nea est� aqu�

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Para APIs
app.MapControllers();

app.Run();