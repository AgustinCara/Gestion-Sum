using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using GestionSUM.Services;
using GestionSUM.Models;

namespace GestionSUM.Filters
{
    public class AdminOnlyAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (UsuarioActual.Rol != RolUsuario.Administrador)
            {
                context.Result = new RedirectToActionResult("Index", "Home", null);
            }
        }
    }
}
