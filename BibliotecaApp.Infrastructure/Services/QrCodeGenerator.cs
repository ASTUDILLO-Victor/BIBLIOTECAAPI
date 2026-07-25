using BibliotecaApp.Domain.Interfaces;
using QRCoder;
using System;

namespace BibliotecaApp.Infrastructure.Services;

public class QrCodeGenerator : IQrCodeGenerator
{
    public string GenerarQrBase64(string texto)
    {
        using var qrGenerator = new QRCodeGenerator();
        using var qrCodeData = qrGenerator.CreateQrCode(texto, QRCodeGenerator.ECCLevel.Q);
        using var qrCode = new PngByteQRCode(qrCodeData);
        byte[] qrCodeBytes = qrCode.GetGraphic(20);

        return Convert.ToBase64String(qrCodeBytes);
    }
}