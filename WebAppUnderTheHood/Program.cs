using Microsoft.AspNetCore.Authorization;
using WebAppUnderTheHood.Authorization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

builder.Services.AddAuthentication("MyCookieAuth").AddCookie("MyCookieAuth", options =>
{
    options.Cookie.Name = "MyCookieAuth";
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireClaim("Admin"));
    options.AddPolicy("MustBelongToHRDepartment", policy => { policy.RequireClaim("Department", "HR"); });
    options.AddPolicy("HRManagerOnly", policy =>
    {
        policy
            .RequireClaim("Department", "HR")
            .RequireClaim("Manager")
            .Requirements.Add(new HrManagerProbationRequirement(3));
    });
});

builder.Services.AddSingleton<IAuthorizationHandler, HrManagerProbationRequirementHandler>();

builder.Services.AddHttpClient("OurWebAPI", client =>
{
    client.BaseAddress = new Uri("https://localhost:7011/");
});

builder.Services.AddSession(options =>
{
    options.Cookie.HttpOnly = true; // To avoid javascript accessing our cookie
    options.IdleTimeout = TimeSpan.FromMinutes(20); // If the user is idle for 20 minutes, the session will timeout
    options.Cookie.IsEssential = true; // Since the session affects the proper functionality of the application, we can set IsEssential to True.
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseSession();

app.MapRazorPages();

app.Run();