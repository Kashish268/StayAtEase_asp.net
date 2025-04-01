var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

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

// Route for Admin controller
app.MapControllerRoute(
    name: "admin",
    pattern: "Account/{action=Dashboard}/{id?}",
    defaults: new { controller = "Account", action = "Dashboard" }
);
app.MapControllerRoute(
    name: "super_admin",
    pattern: "Account/{action=Super_AdminDashboard}/{id?}",
    defaults: new { controller = "Account", action = "Super_AdminDashboard" }
);

app.Run();
