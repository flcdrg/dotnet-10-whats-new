using Mediator;
using Microsoft.EntityFrameworkCore;
using webapp.Application.Queries;
using webapp.Data;
using webapp.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddMediator(
    (MediatorOptions options) =>
    {
        options.Assemblies = [typeof(GetPetsQuery).Assembly];
        options.ServiceLifetime = ServiceLifetime.Scoped;
    }
);

// Register EF Core with SQLite
builder.Services.AddDbContext<PetstoreContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection") ?? "Data Source=petstore.db"));

builder.Services.AddHttpContextAccessor();

// Register custom services
builder.Services.AddScoped<ITimeProvider, SystemTimeProvider>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IGstCalculationService, GstCalculationService>();
builder.Services.AddScoped<IShippingCalculationService, ShippingCalculationService>();
builder.Services.AddScoped<ICountryService, CountryService>();

// Register session
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Create database if it doesn't exist
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<PetstoreContext>();
    if (app.Environment.IsDevelopment())
    {
        // In development, reset the DB so seed data changes take effect
        context.Database.EnsureDeleted();
    }
    context.Database.EnsureCreated();
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseSession();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
