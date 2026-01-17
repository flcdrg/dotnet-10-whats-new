using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add EF Core with environment-specific database configuration
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
var environment = builder.Environment;

if (environment.IsDevelopment())
{
    // Use SQLite for local development
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseSqlite(
            "Data Source=petstore.db;",
            sqliteOptions => sqliteOptions.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery)
        )
    );
}
else
{
    // Use SQL Server for production
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseSqlServer(
            connectionString ?? throw new InvalidOperationException("DefaultConnection string not found"),
            sqlServerOptions => sqlServerOptions.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery)
        )
    );
}

var app = builder.Build();

// Apply EF Core migrations at startup
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await dbContext.Database.MigrateAsync();
}

app.MapGet("/", () => "Hello World!");
app.MapGet("/health", () => Results.Ok(new { status = "healthy" }));

app.Run();
