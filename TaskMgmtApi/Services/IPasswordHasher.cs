namespace TaskManagementApi.Services;

public interface IPasswordHasher
{
    (byte[] hash, byte[] salt) HashPassword(string password);
    bool Verify(string password, byte[] hash, byte[] salt);
}
