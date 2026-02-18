using Microsoft.AspNetCore.DataProtection;

namespace EmployeeApi.Services;

public class UrlEncryptionService : IUrlEncryptionService
{
    private readonly IDataProtector _protector;

    public UrlEncryptionService(IDataProtectionProvider dataProtectionProvider)
    {
        // "UrlEncryption" is a purpose string that isolates this protector
        _protector = dataProtectionProvider.CreateProtector("UrlEncryption");
    }

    public string Encrypt(int id)
    {
        return _protector.Protect(id.ToString());
    }

    public int Decrypt(string encryptedId)
    {
        var decrypted = _protector.Unprotect(encryptedId);
        return int.Parse(decrypted);
    }
}
