namespace GestionSUM.Services
{
    public interface IEmailService
    {
        Task EnviarConfirmacionReservaAsync(string emailDestino, string nombreUsuario, string fecha, string turno);
    }
}