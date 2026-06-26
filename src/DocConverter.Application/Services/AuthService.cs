using DocConverter.Application.DTOs;
using DocConverter.Application.Interfaces;
using DocConverter.Domain.Entities;
using DocConverter.Domain.Interfaces;
using DocConverter.Domain.Exceptions;
using FluentValidation;

namespace DocConverter.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _users;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenGenerator _tokenGenerator;
    private readonly IValidator<RegisterRequest> _registerValidator;
    private readonly IValidator<LoginRequest> _loginValidator;

    public AuthService(
        IUserRepository users,
        IPasswordHasher passwordHasher,
        IJwtTokenGenerator tokenGenerator,
        IValidator<RegisterRequest> registerValidator,
        IValidator<LoginRequest> loginValidator)
    {
        _users = users;
        _passwordHasher = passwordHasher;
        _tokenGenerator = tokenGenerator;
        _registerValidator = registerValidator;
        _loginValidator = loginValidator;
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
    {
        // 1. Validar la estructura del DTO usando FluentValidation
        var validationResult = await _registerValidator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            // Se toma el primer error y se lo lanza como BadRequestException
            var firstError = validationResult.Errors.First().ErrorMessage;
            throw new BadRequestException(firstError);
        }

        // 2. Validar si el email ya está registrado
        var emailExists = await _users.ExistsByEmailAsync(request.Email);
        if (emailExists)
        {
            throw new BadRequestException("El email ya está registrado."); 
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
        var validationResult = await _loginValidator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            var firstError = validationResult.Errors.First().ErrorMessage;
            throw new BadRequestException(firstError);
        }

        // Buscar al usuario por Email
        var user = await _users.GetByEmailAsync(request.Email);
        if (user == null || !_passwordHasher.VerifyPassword(request.Password, user.PasswordHash))
        {
            throw new UnauthorizedException("Email o contraseña inválidos.");
        }

        // Generar Token y responder
        var token = _tokenGenerator.GenerateToken(user);
        return new AuthResponse(user.Id, user.Email, token);
    }
}