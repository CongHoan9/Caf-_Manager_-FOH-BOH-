using API;
using BLL;
using DAL;
using DAL.Helper;
using Helper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console() // log ra console
    .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day) // log ra file
    .CreateLogger();

// thay logging
builder.Host.UseSerilog();

builder.Services.AddMemoryCache();
builder.Services.AddCors(options =>
{
  options.AddPolicy("AllowAll", builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});
// Add services to the container.
builder.Services.AddTransient<IDatabaseHelper, DatabaseHelper>();
builder.Services.AddTransient<IItemGroupRepository, ItemGroupRepository>();
builder.Services.AddTransient<IItemGroupBusiness, ItemGroupBusiness>();
builder.Services.AddTransient<IItemRepository, ItemRepository>();
builder.Services.AddTransient<IItemBusiness, ItemBusiness>();
builder.Services.AddTransient<ICustomerRepository, CustomerRepository>();
builder.Services.AddTransient<ICustomerBusiness, CustomerBusiness>();
builder.Services.AddTransient<IHoaDonRepository, HoaDonRepository>();
builder.Services.AddTransient<IHoaDonBusiness, HoaDonBusiness>();
builder.Services.AddTransient<IUserBusiness, UserBusiness>();
builder.Services.AddTransient<IUserRepository, UserRepository>();
builder.Services.AddTransient<INewsBusiness, NewsBusiness>();
builder.Services.AddTransient<INewsRepository, NewsRepository>();
builder.Services.AddTransient<IBanBusiness, BanBusiness>();
builder.Services.AddTransient<IBanRepository, BanRepository>();
builder.Services.AddTransient<INguyenLieuBusiness, NguyenLieuBusiness>();
builder.Services.AddTransient<INguyenLieuRepository, NguyenLieuRepository>();
builder.Services.AddTransient<ICaLamBusiness, CaLamBusiness>();
builder.Services.AddTransient<ICaLamRepository, CaLamRepository>();
builder.Services.AddTransient<ILichLamViecBusiness, LichLamViecBusiness>();
builder.Services.AddTransient<ILichLamViecRepository, LichLamViecRepository>();
builder.Services.AddTransient<IKhuyenMaiBusiness, KhuyenMaiBusiness>();
builder.Services.AddSingleton<IOrderBusiness, OrderBusiness>();
builder.Services.AddTransient<IKhuyenMaiRepository, KhuyenMaiRepository>();

// configure strongly typed settings objects
var appSettingsSection = builder.Configuration.GetSection("AppSettings");
builder.Services.Configure<AppSettings>(appSettingsSection);

// configure jwt authentication
var appSettings = appSettingsSection.Get<AppSettings>();
var key = Encoding.ASCII.GetBytes(appSettings.Secret);
builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(x =>
{
    x.RequireHttpsMetadata = false;
    x.SaveToken = true;
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
var app = builder.Build();
//app.UseApiKeyMiddleware();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseDefaultFiles();
app.UseStaticFiles();
app.UseExceptionHandler(c => c.Run(async context =>
{
    var exception = context.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerPathFeature>()?.Error;
    context.Response.StatusCode = 400; // Return 400 Bad Request for standard database/business errors
    context.Response.ContentType = "application/json";
    
    string errorMsg = exception?.Message ?? "Co loi xay ra tai may chu";
    
    if (errorMsg.Contains("DELETE statement conflicted with the REFERENCE constraint") || errorMsg.Contains("FK_"))
    {
        errorMsg = "Co loi xay ra tai may chu";
    }

    var result = System.Text.Json.JsonSerializer.Serialize(new { message = errorMsg });
    await context.Response.WriteAsync(result);
}));

app.UseRouting();
app.UseCors(x => x
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader());
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();



