var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

builder.Services.AddHttpClient();

var app = builder.Build();

app.UseStaticFiles();
app.MapControllers();
app.MapRazorPages();

app.MapControllerRoute("Default", "{controller=Home}/{action=Index}");

app.Run();