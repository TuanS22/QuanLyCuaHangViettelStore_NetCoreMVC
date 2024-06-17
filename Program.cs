var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();

// Add biến session
builder.Services.AddSession();

var app = builder.Build();


//app.MapGet("/", () => "Hello World!");

// Khai báo biến session để dùng các url khác nhau
app.UseSession();

// khai báo thư mục wwwroot là thư mục ảo
app.UseStaticFiles();

app.MapControllerRoute(
    name: "admin",
    pattern: "{area}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
