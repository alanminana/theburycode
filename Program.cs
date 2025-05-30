using Microsoft.EntityFrameworkCore;
using theburycode.Models;
using theburycode.Services;
using theburycode.Services.Shared;

var builder = WebApplication.CreateBuilder(args);

// 1. MVC
builder.Services.AddControllersWithViews();

// 2. EF Core
builder.Services.AddDbContext<TheBuryCodeContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ERPConnection")));

// 3. Servicios de dominio
builder.Services.AddScoped<IClienteService, ClienteService>();
builder.Services.AddScoped<IProductoService, ProductoService>();
builder.Services.AddScoped<IVentaService, VentaService>();

// 4. Servicios compartidos
builder.Services.AddScoped<IStockService, StockService>();
builder.Services.AddScoped<IPrecioCalculatorService, PrecioCalculatorService>();
builder.Services.AddScoped<IComprobanteService, ComprobanteService>();
builder.Services.AddScoped<IFormaPagoService, FormaPagoService>();
builder.Services.AddScoped<ISearchService, SearchService>();

var app = builder.Build();

// 5. Pipeline
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();      // detalle de errores en dev
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

// 6. Ruta por defecto (ajústala a tu acción inicial)
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=VueTest}/{id?}");

app.Run();
