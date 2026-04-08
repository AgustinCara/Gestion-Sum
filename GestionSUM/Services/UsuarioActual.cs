using System.Security.Claims;
using GestionSUM.Models;
using Microsoft.AspNetCore.Identity;

namespace GestionSUM.Services
{
    public static class UsuarioActual
    {
        private static IHttpContextAccessor _accessor;

        public static void Configure(IHttpContextAccessor accessor)
        {
            _accessor = accessor;
        }

        public static int UsuarioId
        {
            get
            {
                var claim = _accessor?.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
                return int.TryParse(claim, out int id) ? id : 0;
            }
        }

        public static RolUsuario Rol
        {
            get
            {
                var context = _accessor?.HttpContext;
                if (context == null || !context.User.Identity.IsAuthenticated)
                    return RolUsuario.Vecino;

                // Buscamos el UserManager a través de los servicios del request
                var userManager = context.RequestServices.GetService<UserManager<Usuario>>();
                var user = userManager?.GetUserAsync(context.User).Result;

                return user?.Rol ?? RolUsuario.Vecino;
            }
        }
    }
}