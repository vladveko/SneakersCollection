using System.Security.Claims;
using Infrastructure.Authentication;

namespace Web.Services;

public class CurrentUserService: ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor) => _httpContextAccessor = httpContextAccessor;

    public Guid UserId =>
        Guid.TryParse(UserClaims?.SingleOrDefault(c => c.Type == Claims.UserId)?.Value, out var userId)
            ? userId
            : Guid.Empty;
    
    private IEnumerable<Claim>? UserClaims => _httpContextAccessor?.HttpContext?.User?.Claims;
}