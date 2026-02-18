namespace EmployeeApi.Services;

public interface IUrlEncryptionService
{
    string Encrypt(int id);
    int Decrypt(string encryptedId);
}
