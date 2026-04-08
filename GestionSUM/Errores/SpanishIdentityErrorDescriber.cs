using Microsoft.AspNetCore.Identity;

namespace GestionSUM.Helpers
{
    public class SpanishIdentityErrorDescriber : IdentityErrorDescriber
    {
        public override IdentityError DuplicateUserName(string userName)
        {
            return new IdentityError { Code = nameof(DuplicateUserName), Description = $"El nombre de usuario '{userName}' ya está en uso." };
        }

        public override IdentityError DuplicateEmail(string email)
        {
            return new IdentityError { Code = nameof(DuplicateEmail), Description = $"El correo electrónico '{email}' ya está registrado." };
        }

        public override IdentityError PasswordTooShort(int length)
        {
            return new IdentityError { Code = nameof(PasswordTooShort), Description = $"La contraseña es demasiado corta (mínimo {length} caracteres)." };
        }

        public override IdentityError InvalidEmail(string email)
        {
            return new IdentityError { Code = nameof(InvalidEmail), Description = $"La dirección de correo electrónico '{email}' no es válida." };
        }

        // Podés agregar más overrides aquí si necesitás traducir otros mensajes (PasswordRequiresDigit, etc)
    }
}