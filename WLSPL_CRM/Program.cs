using System.Data;
using Microsoft.Data.SqlClient;
using WLSPL_CRM_2.repository;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddSingleton<IDbConnection>(sp =>
    new SqlConnection(builder.Configuration.GetConnectionString("Conn_String"))
);

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.Name = "MySession";
});


builder.Services.AddScoped<IUserRegistrationRepo, UserRegistration>();
builder.Services.AddScoped<IleadRepo, LeadRepo>();
builder.Services.AddScoped<IcomapnymasterRepo, CompanymasterRepo>();
builder.Services.AddScoped<Iprospectrepo, prospectrepo>();
builder.Services.AddScoped<IQuotationRepo, Quotationrepo>();
builder.Services.AddScoped<IServicesRepo, ServicesRepo>();
builder.Services.AddScoped<IDealRepo, DealRepo>();
builder.Services.AddScoped<IProformaInvoiceRepo, ProformaInvoiceRepo>();
builder.Services.AddScoped<IWorkorderRepo, WorkorderRepo>();

builder.Services.AddControllersWithViews();

builder.Services.AddHttpContextAccessor();

builder.Services.AddAuthentication("Cookies")
    .AddCookie("Cookies", options =>
    {
        options.LoginPath = "/Login/Index";
        options.AccessDeniedPath = "/Login/AccessDenied";
    });

builder.Services.AddAuthorization();
builder.Services.AddAuthentication();


var app = builder.Build();
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseSession();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Login}/{action=Index}/{id?}");


app.Run();
