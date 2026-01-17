using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add OpenAPI support
builder.Services.AddOpenApi();

const string CorsPolicy = "DefaultCors";

// Configure CORS for webapp dev origin
builder.Services.AddCors(options =>
{
    options.AddPolicy(CorsPolicy, policy =>
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyMethod()
              .AllowAnyHeader());
});

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

// Map OpenAPI endpoint
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// Apply CORS
app.UseCors(CorsPolicy);

// Apply EF Core migrations at startup
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await dbContext.Database.MigrateAsync();
}

app.MapGet("/", () => "Hello World!")
    .WithName("GetRoot")
    .WithOpenApi();

app.MapGet("/health", () => Results.Ok(new { status = "healthy" }))
    .WithName("GetHealth")
    .WithOpenApi();

app.MapGet("/api/accessories", GetAllAccessories)
    .WithName("GetAllAccessories")
    .WithOpenApi();

async Task<IResult> GetAllAccessories(AppDbContext dbContext)
{
    var accessories = await dbContext.Accessories.ToListAsync();
    return Results.Ok(accessories);
}

app.Run();
