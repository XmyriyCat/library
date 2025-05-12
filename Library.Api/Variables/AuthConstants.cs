namespace Library.Api.Variables;

public static class AuthConstants
{
    public const string RoleClaimName = System.Security.Claims.ClaimTypes.Role;
    
    public const string AdminPolicyName = "Admin";
    public const string AdminClaimValue = "admin";

    public const string ManagerPolicyName = "Manager";
    public const string ManagerClaimValue = "manager";
}