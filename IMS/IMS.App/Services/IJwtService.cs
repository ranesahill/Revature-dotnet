using IMS.Core.Entities;

namespace IMS.App.Services;

public interface IJwtService
{
    string GenerateToken(User user);
}
