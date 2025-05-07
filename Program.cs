using payfish.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Builder;


var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSession();

// اتصال به دیتابیس SQLite (مناسب برای شروع ساده)
builder.Services.AddDbContext<PayfishDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

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
app.UseSession(); // باید قبل از UseEndpoints باشه
app.UseAuthorization();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Paystub}/{action=Login}/{id?}"); // ✅ استفاده صحیح


app.Run();
