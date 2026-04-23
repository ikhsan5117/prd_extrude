using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using VelastoProductionSystem.Data;
using VelastoProductionSystem.WebSockets;
using VelastoProductionSystem.Hubs;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSignalR();

// Configure Entity Framework Core with SQL Server or SQLite
var defaultConnection = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    if (defaultConnection != null && (defaultConnection.Contains(".db") || defaultConnection.Contains("Data Source")))
        options.UseSqlite(defaultConnection);
    else
        options.UseSqlServer(defaultConnection);
});

// Configure second DB context for ELWP_PRD (read-only, for planning sync)
var elwpConnection = builder.Configuration.GetConnectionString("ElwpConnection");
builder.Services.AddDbContext<ElwpDbContext>(options =>
{
    if (elwpConnection != null && (elwpConnection.Contains(".db") || elwpConnection.Contains("Data Source")))
        options.UseSqlite(elwpConnection);
    else
        options.UseSqlServer(elwpConnection);
});

// Add session support for production tracking
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(8);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Ensure databases are created and seed initial data
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        var elwpContext = services.GetRequiredService<ElwpDbContext>();
        
        // Ensure schemas exist if using SQLite
        if (context.Database.ProviderName?.Contains("Sqlite") == true)
        {
            context.Database.EnsureCreated();
        }
        
        if (elwpContext.Database.ProviderName?.Contains("Sqlite") == true)
        {
            elwpContext.Database.EnsureCreated();
        }

        DataSeeder.SeedData(context, elwpContext);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while initializing the database.");
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
    app.UseHttpsRedirection();
}
app.UseStaticFiles();

app.UseRouting();
app.UseWebSockets();

app.UseSession();
app.UseAuthorization();

app.Map("/ws/dashboard", async context =>
{
    if (context.WebSockets.IsWebSocketRequest)
    {
        using var webSocket = await context.WebSockets.AcceptWebSocketAsync();
        await DashboardWebSocketHandler.HandleAsync(webSocket, context.RequestServices);
    }
    else
    {
        context.Response.StatusCode = StatusCodes.Status400BadRequest;
    }
});

app.MapHub<DashboardHub>("/hub/dashboard");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
