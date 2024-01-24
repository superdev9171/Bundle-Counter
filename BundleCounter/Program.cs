using BundleCounter;
using BundleCounter.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<AppDBContext>(options =>
    options.UseSqlite(connectionString));

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddTransient<IBundleService, BundleService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");


using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<AppDBContext>();
    var logger = services.GetService<ILogger<Program>>();
    
    logger?.LogInformation("Server Starting At: " + DateTimeOffset.Now.ToString());
    
    int tries = 10;
    while (tries-- > 0)
    {
        try
        {
            if (context.Database.GetPendingMigrations().Any())
                context.Database.Migrate();
            break;
        }
        catch (Exception ex)
        {
            logger?.LogError(ex, "");
            Thread.Sleep(5000);
        }
    }
}

app.Run();
