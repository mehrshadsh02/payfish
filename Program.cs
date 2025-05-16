using payfish.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Builder;


var builder = WebApplication.CreateBuilder(args);

// اتصال به دیتابیس SQLite (مناسب برای شروع ساده)
builder.Services.AddDbContext<PayfishDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllersWithViews();

//احراز هویت کوکی (Cookie Authentication)

builder.Services.AddAuthentication("MyCookie")
    .AddCookie("MyCookie", options =>
    {
        options.LoginPath = "/Paystub/Login";  // آدرس لاگین شما
        options.AccessDeniedPath = "/Paystub/AccessDenied"; // اختیاری
    });

builder.Services.AddAuthorization();

builder.Services.AddSession(options =>
{
    options.Cookie.SameSite = SameSiteMode.None;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
});


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
app.UseAuthentication();
app.UseAuthorization();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Paystub}/{action=Login}/{id?}"); // ✅ استفاده صحیح

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<PayfishDbContext>();
    db.Database.Migrate(); // ← این خط دیتابیس رو با مایگریشن ایجاد می‌کنه
}

app.Run();
