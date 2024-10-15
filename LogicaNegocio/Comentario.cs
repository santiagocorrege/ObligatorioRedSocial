using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicaNegocio
{
    public class Comentario : Publicacion
    {
        private Privacidad _privacidad;

        //Se establece al setter que luego cuando se llama el constructor en Post se establece la misma privacidad que el post posee
        public Privacidad Privacidad
        {
            set => _privacidad = value;
        }
        public Comentario (Miembro autor, string titulo, string contenido, Privacidad privacidad) : base(autor, titulo, contenido, privacidad)
        {
        }

    }
}
