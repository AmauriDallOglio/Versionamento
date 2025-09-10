namespace Versionamento.Servico.DTO
{
    // Request para login
    public record LoginRequest(string Usuario, string Senha);

    // Usuário do sistema
    public class Usuario
    {
        public string Nome { get; set; } = "";
        public string Senha { get; set; } = "";
        public string[] Roles { get; set; } = Array.Empty<string>();
        public string[] Versoes { get; set; } = Array.Empty<string>();
    }

    // Lista de usuários para teste
    public static class UsuariosTeste
    {
        public static List<Usuario> Lista { get; } = new List<Usuario>
        {
            new Usuario { Nome = "usuario1", Senha = "1234", Roles = new[] { "User" }, Versoes = new[] { "v1" } },
            new Usuario { Nome = "usuario2", Senha = "1234", Roles = new[] { "User" }, Versoes = new[] { "v1", "v2" } },
            new Usuario { Nome = "usuario3", Senha = "1234", Roles = new[] { "Admin" }, Versoes = new[] { "v1", "v2", "v3" } }
        };
    }

}
