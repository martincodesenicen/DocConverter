namespace DocConverter.Application.Interfaces;

public interface ICurrentUserService
{
    // Guid del usuario autenticado en la petición actual
    Guid? UserId { get; }
}