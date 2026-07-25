namespace BibliotecaApp.Domain.Interfaces;

public interface IQrCodeGenerator
{
    string GenerarQrBase64(string texto);
}