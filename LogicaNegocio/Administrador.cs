using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicaNegocio
{
    public class Administrador : Usuario
    {
        public Administrador(string email, string password) : base(email, password)
        {
        }

        public override string Rol()
        {
            return "ADMINISTRADOR";
        }
    }
}
