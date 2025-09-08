using TaskManagementApi.Models;

namespace TaskManagementApi.Services;

public interface IJwtTokenService
{
    (string token, DateTime expiresAt) CreateToken(User user);
}
