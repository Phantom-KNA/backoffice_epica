using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Epica.Web.Operacion.Config;
using Epica.Web.Operacion.Services.UserResolver;
using Epica.Web.Operacion.Services.Authentication;
using Epica.Web.Operacion.Services;
using Epica.Web.Operacion.Services.Transaccion;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.ConfigureKestrel(options =>
{
    options.Limits.KeepAliveTimeout = TimeSpan.FromMinutes(10);
});

// Configuración de autenticación JWT
builder.Services.AddAuthentication(jtw =>
{
    jtw.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    jtw.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(jtw =>
{
    jtw.SaveToken = true;
    jtw.TokenValidationParameters = new TokenValidationParameters
    {
        // Parámetros de validación del token JWT

        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        RequireExpirationTime = true,
        ValidIssuer = builder.Configuration["JWT:Issuer"],
        ValidAudience = builder.Configuration["JWT:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:ClaveSecreta"]))
    };
});

// Configuración de servicios
builder.Services.AddRazorPages();
builder.Services.AddSignalR();
builder.Services.AddScoped<IUserResolver, UserResolver>();
builder.Services.AddScoped<IServiceAuth, ServiceAuth>();
builder.Services.AddScoped<ITransaccionesApiClient, TransaccionesApiClient>();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(Convert.ToUInt32(builder.Configuration["TiempoExpiracionSesion"]));
});

// Configuración de HttpClient para servicios de API

#region CanalProxy
builder.Services.AddHttpClient("serviciosAPI", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["UrlAPI"]);
    client.Timeout = TimeSpan.FromMinutes(10);
})
.ConfigureHttpMessageHandlerBuilder((action) =>
{
    new HttpClientHandler()
    {
        ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
    };
});

#endregion

builder.Services.AddOptions();
builder.Services.Configure<UrlsConfig>(builder.Configuration.GetSection("urls"));

builder.Services.AddControllersWithViews()
                .AddRazorRuntimeCompilation();
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// Configuración de manejo de errores y redirección HTTPS

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();

//app.Use(async (context, next) =>
//{
//    try
//    {
//        var JWToken = context.Session.GetString("WebApp");
//        if (!string.IsNullOrEmpty(JWToken))
//        {
//            context.Request.Headers.Add("Authorization", "Bearer " + JWToken);
//        }
//        await next();
//    }
//    catch (Exception ex)
//    {

//    }
//});

//app.Use(async (context, next) =>
//{
//    try
//    {
//        var apiKey = context.Session.GetString("WebApp");
//        if (!string.IsNullOrEmpty(apiKey))
//        {
//            context.Request.Headers.Add("x-api-key", apiKey);
//        }
//        await next();
//    }
//    catch (Exception ex)
//    {
//app.Use(async (context, next) =>
//{
//    try
//    {
//        var JWToken = context.Session.GetString("WebApp");
//        if (!string.IsNullOrEmpty(JWToken))
//        {
//            context.Request.Headers.Add("Authorization", "Bearer " + JWToken);
//        }
//        await next();
//    }
//    catch (Exception ex)
//    {
//         //Manejo de excepciones

//            }
// });


        app.UseAuthentication();
        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
        });

        app.Run();