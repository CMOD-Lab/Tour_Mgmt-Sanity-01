using Serilog;
using Tour_Management.Application.Extensions;
using Tour_Management.Infrastructure.Extensions;

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/tour-management-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    // Use Serilog
    builder.Host.UseSerilog();

    // Add Razor Pages
    builder.Services.AddRazorPages();

    // Add session support
    builder.Services.AddDistributedMemoryCache();
    builder.Services.AddSession(options =>
    {
        options.IdleTimeout = TimeSpan.FromMinutes(30);
        options.Cookie.HttpOnly = true;
        options.Cookie.IsEssential = true;
    });

    // Add HttpContextAccessor
    builder.Services.AddHttpContextAccessor();

    // Register Application services
    builder.Services.AddApplicationServices();

    // Register Infrastructure services
    builder.Services.AddInfrastructureServices(builder.Configuration);

    var app = builder.Build();

    // Configure the HTTP request pipeline
    if (!app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler("/Error");
        app.UseHsts();
    }
    else
    {
        app.UseDeveloperExceptionPage();
    }

    app.UseHttpsRedirection();
    app.UseStaticFiles();

    app.UseRouting();

    app.UseSession();

    app.UseAuthorization();

    app.MapRazorPages();

    // Default route to login page
    app.MapGet("/", context =>
    {
        context.Response.Redirect("/Users/Login");
        return Task.CompletedTask;
    });

    Log.Information("Tour Management application starting up");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application start-up failed");
}
finally
{
    Log.CloseAndFlush();
}
