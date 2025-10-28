using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Versionamento.Servico.DTO; // Importa a classe LoginRequest e UsuariosTeste


namespace Versionamento.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [ApiVersion("1.0")]
    public class TokenController : ControllerBase
    {
        private readonly IConfiguration _config;

        public TokenController(IConfiguration config)
        {
            _config = config;
        }

        [HttpPost]
        public IActionResult GerarToken([FromBody] LoginRequest login)
        {
            // Busca usuário na lista estática
            var usuario = Usuario.Lista().FirstOrDefault(u =>
                u.Nome.Equals(login.Usuario, StringComparison.OrdinalIgnoreCase) &&
                u.Senha == login.Senha);

            if (usuario == null)
                return Unauthorized(new { mensagem = "Usuário ou senha inválidos" });

            // Chave secreta usada para assinar o token
            var key = Encoding.UTF8.GetBytes("minha_chave_secreta_super_segura");

            // Criação da lista de claims
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, usuario.Nome) // Nome do usuário
            };

            // Adiciona claims de roles (papéis do usuário)
            foreach (var role in usuario.Roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            // Adiciona claims de versões de API que o usuário pode acessar
            foreach (var versao in usuario.Versoes)
            {
                claims.Add(new Claim("AcessoApi", versao));
            }


            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddMinutes(60), // O token expira em 60 minutos a partir de agora (UTC)
                NotBefore = DateTime.Now, // Define quando o token começa a valer (agora, em UTC)
                IssuedAt = DateTime.Now, // Define quando o token foi emitido (agora, em UTC)

                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature
                )
            };

         

            var tokenHandler = new JwtSecurityTokenHandler();
            var securityToken = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(securityToken);

            // Retorna token e data de expiração convertida para hora local
            return Ok(new
            {
                token = tokenString,
                expiracao = securityToken.ValidTo.ToLocalTime() // Mostra em hora local
            });
        }
    }
}
