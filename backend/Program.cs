using Demo.Api.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add OpenAPI support
builder.Services.AddOpenApi();

const string CorsPolicy = "DefaultCors";

// Configure CORS for webapp and adminapp dev origins
builder.Services.AddCors(options =>
{
    options.AddPolicy(CorsPolicy, policy =>
        policy.WithOrigins(
                "http://localhost:5173", // webapp
                "http://localhost:5196", // adminapp http
                "https://localhost:7041" // adminapp https
            )
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

var accessories = app.MapGroup("/api/accessories");

accessories.MapGet("/", async (AppDbContext dbContext) =>
{
    var items = await dbContext.Accessories.ToListAsync();
    return Results.Ok(items);
})
.WithName("GetAllAccessories")
.WithOpenApi();

accessories.MapGet("/{id:int}", async (int id, AppDbContext dbContext) =>
{
    var item = await dbContext.Accessories.FindAsync(id);
    return item is null ? Results.NotFound() : Results.Ok(item);
})
.WithName("GetAccessoryById")
.WithOpenApi();

accessories.MapPost("/", async (AccessoryRequest request, AppDbContext dbContext) =>
{
    var validationError = ValidateAccessory(request);
    if (validationError is not null)
    {
        return Results.BadRequest(new { error = validationError });
    }

    var accessory = new Accessory
    {
        Name = request.Name.Trim(),
        Category = request.Category.Trim(),
        Price = request.Price,
        StockQuantity = request.StockQuantity,
        Description = request.Description?.Trim(),
        ImageUrl = request.ImageUrl?.Trim()
    };

    dbContext.Accessories.Add(accessory);
    await dbContext.SaveChangesAsync();

    return Results.Created($"/api/accessories/{accessory.Id}", accessory);
})
.WithName("CreateAccessory")
.WithOpenApi();

accessories.MapPut("/{id:int}", async (int id, AccessoryRequest request, AppDbContext dbContext) =>
{
    var validationError = ValidateAccessory(request);
    if (validationError is not null)
    {
        return Results.BadRequest(new { error = validationError });
    }

    var accessory = await dbContext.Accessories.FindAsync(id);
    if (accessory is null)
    {
        return Results.NotFound();
    }

    accessory.Name = request.Name.Trim();
    accessory.Category = request.Category.Trim();
    accessory.Price = request.Price;
    accessory.StockQuantity = request.StockQuantity;
    accessory.Description = request.Description?.Trim();
    accessory.ImageUrl = request.ImageUrl?.Trim();

    await dbContext.SaveChangesAsync();

    return Results.Ok(accessory);
})
.WithName("UpdateAccessory")
.WithOpenApi();

accessories.MapDelete("/{id:int}", async (int id, AppDbContext dbContext) =>
{
    var accessory = await dbContext.Accessories.FindAsync(id);
    if (accessory is null)
    {
        return Results.NotFound();
    }

    dbContext.Accessories.Remove(accessory);
    await dbContext.SaveChangesAsync();

    return Results.NoContent();
})
.WithName("DeleteAccessory")
.WithOpenApi();

static string? ValidateAccessory(AccessoryRequest request)
{
    if (string.IsNullOrWhiteSpace(request.Name))
    {
        return "Name is required.";
    }

    if (string.IsNullOrWhiteSpace(request.Category))
    {
        return "Category is required.";
    }

    if (request.Price < 0)
    {
        return "Price must be zero or greater.";
    }

    if (request.StockQuantity < 0)
    {
        return "Stock quantity must be zero or greater.";
    }

    return null;
}

app.Run();

// Request contract for creating or updating accessories
public record AccessoryRequest(
    string Name,
    string Category,
    decimal Price,
    int StockQuantity,
    string? Description,
    string? ImageUrl
);
