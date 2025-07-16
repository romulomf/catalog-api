using CatalogApiClient.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

builder.Services.AddHttpClient();

var categoriesApiUri = builder.Configuration["ServiceUri:CategoriesApi"] ?? "http://localhost:5000";
builder.Services.AddHttpClient("CategoriesApi", c => c.BaseAddress = new Uri(categoriesApiUri));

builder.Services.AddScoped<CategoryService>();

var app = builder.Build();

app.UseStaticFiles();
app.MapControllers();
app.MapRazorPages();

app.MapControllerRoute("Default", "{controller=Home}/{action=Index}");

app.Run();