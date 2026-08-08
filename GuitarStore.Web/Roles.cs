namespace GuitarStore.Web;

/// <summary>
/// Role names used for authorization. Backed by a Cognito group of the same name, mapped
/// onto a role claim at sign-in.
/// </summary>
public static class Roles
{
    public const string Admin = "Admin";
}
