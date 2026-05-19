using System.Security.Claims;
using PousadaApi.Application.Interfaces;

namespace PousadaApi.Api.Services;

public sealed class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public int UserId
    {
        get
        {
            var user = _httpContextAccessor.HttpContext?.User;
            var idClaim = user?.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? user?.FindFirstValue("id");

            if (string.IsNullOrEmpty(idClaim) || !int.TryParse(idClaim, out var userId))
                throw new UnauthorizedAccessException("Usuário não autenticado.");

            return userId;
        }
    }
}
