using System.Security.Claims;
using TrailerTrack.Application.Interfaces;

namespace TrailerTrack.Web.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor accessor)
        => _httpContextAccessor = accessor;

    public string? UserId
        => _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);

    public bool IsInRole(string role)
        => _httpContextAccessor.HttpContext?.User?.IsInRole(role) ?? false;
}