namespace SignarlRChat.Areas.Admin.Models
{
    public class PermissionViewModel
    {
        public string RoleId { get; set; } = string.Empty;
        public List<RoleClaimsViewModel> RoleClaims { get; set; } = new();
    }

    public class RoleClaimsViewModel
    {
        public string ClaimType { get; set; } = string.Empty;

        public bool IsViewSelected { get; set; }
        public string ViewValue { get; set; } = string.Empty;
        public bool IsCreateSelected { get; set; }
        public string CreateValue { get; set; } = string.Empty;
        public bool IsEditSelected { get; set; }
        public string EditValue { get; set; } = string.Empty;
        public bool IsDeleteSelected { get; set; }
        public string DeleteValue { get; set; } = string.Empty;
        public string ModuleName { get; set; } = string.Empty;
    }
}
