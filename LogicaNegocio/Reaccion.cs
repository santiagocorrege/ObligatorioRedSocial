using LogicaNegocio.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicaNegocio
{
    public enum TipoReaccion
    {
        ME_GUSTA,
        NO_ME_GUSTA,
    }
    public class Reaccion:IEquatable<Reaccion>
    {
        private Miembro _autor;
        private TipoReaccion _tipoReaccion;

        public Reaccion(Miembro autor, TipoReaccion tipoReaccion)
        {
            _autor = autor;
            _tipoReaccion = tipoReaccion;
        }

        public Miembro Autor { get { return _autor; } }

        public TipoReaccion TipoReaccion {  get => _tipoReaccion; }
        public bool Equals(Reaccion? other)
        {
            return _autor.Equals(other.Autor) && _tipoReaccion.Equals(other.TipoReaccion);
        }

        public override string ToString()
        {
            return $"\n====REACCION====\n\nAutor: {_autor.Email}\nReaccion: {_tipoReaccion}";
        }
    }
}
