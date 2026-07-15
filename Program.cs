using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using DAShopTech.Models;
using System;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.Facebook;
using Newtonsoft.Json;
using DAShopTech.Services;
using System.Data.SqlClient;

var builder = WebApplication.CreateBuilder(args);

// Cấu hình dịch vụ DbContext và kết nối cơ sở dữ liệu
builder.Services.AddDbContext<ShopTechDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
// Program.cs
builder.Services.AddHttpClient<ChatGPTService>(client =>
{
    client.BaseAddress = new Uri("https://api.openai.com/");
});

// Đăng ký dịch vụ SmsService
builder.Services.AddSingleton<SmsService>(serviceProvider =>
{
    var configuration = serviceProvider.GetRequiredService<IConfiguration>();
    return new SmsService(configuration);
});

builder.Services.AddHttpClient<ChatGPTService>();
builder.Services.AddSingleton(new ChatGPTService(new HttpClient(), "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"));


// Đăng ký dịch vụ PaymentService
builder.Services.AddScoped<IPaymentService, PaymentService>(); 

// Thêm dịch vụ Authentication và Cookie
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
})
.AddCookie() // Sử dụng Cookie Authentication
.AddGoogle(googleOptions =>
{
    googleOptions.ClientId = "YOUR_GOOGLE_CLIENT_ID";
    googleOptions.ClientSecret = "YOUR_GOOGLE_CLIENT_SECRET";
})
.AddFacebook(facebookOptions =>
{
    facebookOptions.AppId = "YOUR_FACEBOOK_APP_ID";
    facebookOptions.AppSecret = "YOUR_FACEBOOK_APP_SECRET";
});

// Đăng ký dịch vụ session
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Thời gian timeout session
    options.Cookie.HttpOnly = true; // Bảo mật cookie session chỉ được truy cập qua HTTP
    options.Cookie.IsEssential = true; // Bắt buộc cookie cần thiết cho session hoạt động
});

// Thêm các dịch vụ MVC (Controllers với Views)
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Cấu hình middleware cho ứng dụng
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error"); // Sử dụng trang lỗi tùy chỉnh
    app.UseHsts(); // Kích hoạt HSTS để bảo vệ chống lại tấn công MITM
}

app.UseHttpsRedirection(); // Chuyển hướng các yêu cầu HTTP sang HTTPS
app.UseStaticFiles(); // Cho phép phục vụ các tệp tĩnh từ thư mục wwwroot

app.UseRouting(); // Định tuyến các yêu cầu đến các endpoint của controller

app.UseSession(); // Kích hoạt sử dụng session trước khi phân quyền
app.UseAuthentication(); // Middleware Authentication
app.UseAuthorization(); // Phân quyền cho các yêu cầu dựa trên role

app.MapControllers(); // Định tuyến các controller

// Cấu hình route mặc định cho ứng dụng
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Khởi tạo dữ liệu khi ứng dụng khởi động (đảm bảo cơ sở dữ liệu được tạo nếu chưa có)
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ShopTechDbContext>();
        // Gọi phương thức khởi tạo dữ liệu hoặc migration nếu cần
        context.Database.Migrate(); // Áp dụng các migration nếu có (nếu sử dụng)
        DataInitializer.Initialize(context); // Khởi tạo dữ liệu
    }
    catch (Exception ex)
    {
        // Ghi lại log lỗi khi khởi tạo dữ liệu không thành công
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Đã xảy ra lỗi khi khởi tạo cơ sở dữ liệu.");
    }
}

using (var connection = new SqlConnection(builder.Configuration.GetConnectionString("DefaultConnection")))
{
    try
    {
        connection.Open();
        Console.WriteLine("Kết nối SQL Server thành công!");
    }
    catch (Exception ex)
    {
        Console.WriteLine("Kết nối SQL Server thất bại: " + ex.Message);
    }
}

// Chạy ứng dụng
app.Run();
