namespace ApiClinica.Models;

// Perfis de usuário usados na autorização ([Authorize(Roles = Roles.Admin)]).
public static class Roles
{
    public const string Comum = "Comum";
    public const string Admin = "Admin";
}
