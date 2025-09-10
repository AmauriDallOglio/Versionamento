using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Versionamento.Servico.DTO
{
    public class UsuarioToken
    {
        public string Nome { get; set; } = "";
        public string[] Roles { get; set; } = Array.Empty<string>();
        public string[] Versoes { get; set; } = Array.Empty<string>();
        public DateTime? Expiracao { get; set; }
    }
}
