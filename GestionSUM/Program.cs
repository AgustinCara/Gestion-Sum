using GestionSUM.Data;
using GestionSUM.Helpers;
using GestionSUM.Models;
using GestionSUM.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 1. Configuración de la Base de Datos (MySQL)
var connectionString = "server=localhost;database=sum_reservas;user=root;password=1999;";
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(
        connectionString,
        ServerVersion.AutoDetect(connectionString)
    )
);

// 2. Configuración de Identity
builder.Services.AddIdentity<Usuario, IdentityRole<int>>(options => {
    options.Password.RequireDigit = false;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
    options.SignIn.RequireConfirmedAccount = false;
    options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders()
.AddErrorDescriber<SpanishIdentityErrorDescriber>();

// IMPORTANTE: Para que UsuarioActual pueda leer la sesión
builder.Services.AddHttpContextAccessor();

builder.Services.AddControllersWithViews();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";
});

var app = builder.Build();

// Inicializar la clase estática UsuarioActual con el proveedor de servicios
UsuarioActual.Configure(app.Services.GetRequiredService<IHttpContextAccessor>());

// 3. SEEDING: Crear Administrador y Roles
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole<int>>>();
    var userManager = services.GetRequiredService<UserManager<Usuario>>();

    // Crear rol Administrador si no existe
    if (!await roleManager.RoleExistsAsync("Administrador"))
    {
        await roleManager.CreateAsync(new IdentityRole<int>("Administrador"));
    }

    // Crear rol Vecino si no existe
    if (!await roleManager.RoleExistsAsync("Vecino"))
    {
        await roleManager.CreateAsync(new IdentityRole<int>("Vecino"));
    }

    // Asignar rango Admin a tu cuenta
    var user = await userManager.FindByEmailAsync("agustin.hcarabajal@gmail.com");
    if (user != null)
    {
        if (!await userManager.IsInRoleAsync(user, "Administrador"))
        {
            await userManager.AddToRoleAsync(user, "Administrador");
        }

        if (user.Rol != RolUsuario.Administrador)
        {
            user.Rol = RolUsuario.Administrador;
            await userManager.UpdateAsync(user);
        }
    }
}

// 4. Pipeline de HTTP
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();