using Microsoft.AspNetCore.Authorization;

namespace SignalRChat.Permission
{
    public record PermissionRequirement(string Permission) : IAuthorizationRequirement;
}
