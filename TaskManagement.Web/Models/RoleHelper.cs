namespace TaskManagement.Web.Models
{
    public static class RoleHelper
    {
        public static bool HasDashboardAccess(string role)
        {
            var allowedRoles = new[] { "Admin", "Manager" };
            return allowedRoles.Contains(role);
        }
    }
}
