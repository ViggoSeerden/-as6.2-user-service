using Microsoft.AspNetCore.Authorization;

namespace UserService;

public class HasScopeRequirement(string scope, string? issuer) : IAuthorizationRequirement
{
    public string Issuer { get; } = issuer ?? throw new ArgumentNullException(nameof(issuer));
    public string Scope { get; } = scope ?? throw new ArgumentNullException(nameof(scope));
}