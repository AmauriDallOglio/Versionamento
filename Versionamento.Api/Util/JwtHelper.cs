using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Versionamento.Servico.DTO;

namespace Versionamento.Api.Util
{
    public static class JwtHelper
    {
        public static UsuarioToken DecodificarToken(string token)
        {
            if (string.IsNullOrEmpty(token))
                return null;

            var handler = new JwtSecurityTokenHandler();
            JwtSecurityToken jwtToken;

            try
            {
                jwtToken = handler.ReadJwtToken(token);
            }
            catch
            {
                // Token inválido
                return null;
            }

            // Verifica expiração
            if (jwtToken.ValidTo < DateTime.UtcNow)
                return null; // Token expirado

            var nome = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value ?? "";
            var roles = jwtToken.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).ToArray();
            var versoes = jwtToken.Claims.Where(c => c.Type == "AcessoApi").Select(c => c.Value).ToArray();

            return new UsuarioToken
            {
                Nome = nome,
                Roles = roles,
                Versoes = versoes,
                Expiracao = jwtToken.ValidTo.ToLocalTime() // opcional: hora local
            };
        }
    }
}
