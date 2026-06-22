using DocConverter.Application.DTOs;

namespace DocConverter.Application.Interfaces;

public interface IAuthService
{
    Task<AuthResponse> RegisterAsync(RegisterRequest request);
    Task<AuthResponse> LoginAsync(LoginRequest request);
}