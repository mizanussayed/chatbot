using Microsoft.AspNetCore.Authorization;

namespace SignarlRChat.Permission
{
    public record PermissionRequirement(string Permission) : IAuthorizationRequirement;
}
