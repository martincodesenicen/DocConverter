using DocConverter.Domain.Entities;

namespace DocConverter.Domain.Interfaces;

public interface IJwtTokenGenerator
{
    string GenerateToken(User user);
}