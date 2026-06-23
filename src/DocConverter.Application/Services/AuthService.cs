using DocConverter.Application.DTOs;
using DocConverter.Application.Interfaces;
using DocConverter.Domain.Entities;
using DocConverter.Domain.Interfaces;
using DocConverter.Domain.Exceptions;

namespace DocConverter.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _users;
    private readonly IPasswordHasher _passwordHasher;
private readonly IJwtTokenGenerator _tokenGenerator;

    public AuthService(
        IUserRepository users,
        IPasswordHasher passwordHasher,
        IJwtTokenGenerator tokenGenerator)
    {
        _users = users;
        _passwordHasher = passwordHasher;
        _tokenGenerator = tokenGenerator;
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
    {
        // 1. Validar si el email ya está registrado
        var emailExists = await _users.ExistsByEmailAsync(request.Email);
        if (emailExists)
        {
            throw new BadRequestException("The email is already registered.");
            // Nota: Luego cambiaremos esto por una excepción personalizada controlada por un Middleware global
        }

        // 2. Crear la entidad e inyectar el Hash seguro
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = request.Email,
            PasswordHash = _passwordHasher.HashPassword(request.Password)
        };

        // 3. Guardar en BD
        await _users.AddAsync(user);
        await _users.SaveChangesAsync();

        // 4. Generar el Token y responder
        var token = _tokenGenerator.GenerateToken(user);
        return new AuthResponse(user.Id, user.Email, token);
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        // Buscar al usuario por Email
        var user = await _users.GetByEmailAsync(request.Email);
        if (user == null || !_passwordHasher.VerifyPassword(request.Password, user.PasswordHash))
        {
            throw new UnauthorizedException("Invalid email or password.");
        }

        // Generar Token y responder
        var token = _tokenGenerator.GenerateToken(user);
        return new AuthResponse(user.Id, user.Email, token);
    }
}