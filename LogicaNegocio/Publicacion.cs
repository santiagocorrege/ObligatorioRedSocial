using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LogicaNegocio.Interface;

namespace LogicaNegocio
{
    public enum Privacidad
    {
        PRIVADO,
        PUBLICO
    }
    public class Publicacion: IValidate
    {
        private int _id;
        private static int s_IdSecuencial = 1;
        private Privacidad _privacidad;
        private Miembro _autor;
        private string _titulo;
        private string _contenido;
        private DateTime _fechaPublicacion = DateTime.Today;
        private static int s_fvaMG = 5;
        private static int s_fvaNG = -2;
        private List<Reaccion> _reacciones = new List<Reaccion>();


        public Publicacion(Miembro autor, string titulo, string contenido, Privacidad privacidad)
        {
            _id = s_IdSecuencial++;
            _titulo = titulo;
            _autor = autor;
            _contenido = contenido;
            _privacidad = privacidad;
        }

        public List <Reaccion> Reacciones { get =>  _reacciones; }

        public Miembro Autor { get => _autor; }
        public int Id { get => _id; }
        public string Titulo { get => _titulo; }
        public string Contenido { get => _contenido; }  
        public DateTime FechaPublicacion { get => _fechaPublicacion; }
        public Privacidad Privacidad { get => _privacidad; }


        #region Methods
        //Verifica si el Miembro puede participar en la publicacion.

        public virtual decimal CalcularAceptacion()
        {
            decimal valorAceptacion = 0;
            for(int i=0; i< Reacciones.Count; i++)
            {
                if (Reacciones[i].TipoReaccion == TipoReaccion.ME_GUSTA)
                {
                    valorAceptacion += s_fvaMG;
                } else
                {
                    valorAceptacion += s_fvaNG;
                }
            }
            return valorAceptacion;
        }

        public int DevolverMeGusta()
        {
            int mg = 0;
            foreach(Reaccion reaccion in _reacciones)
            {
                if(reaccion.TipoReaccion == TipoReaccion.ME_GUSTA)
                {
                    mg++;
                }
            }
            return mg;
        }

        public int DevolverNoMeGusta()
        {
            int nmg = 0;
            foreach (Reaccion reaccion in _reacciones)
            {
                if (reaccion.TipoReaccion == TipoReaccion.NO_ME_GUSTA)
                {
                    nmg++;
                }
            }
            return nmg;
        }

        
        //Sirve para validar la participacion/visibilidad de un usuario en el post de otro usuario
        public bool ValidateParticipacion(Miembro participante)
        {
            bool puedeParticipar = false;
            //El usuario no podra participar (visualizar o responder) publicaciones si este esta bloqueado
            if (!participante.Bloqueado)
            {
                //Chequeo privacidad publicacion primero si es Publica entonces puede participar.
                if (_privacidad == Privacidad.PRIVADO)
                {
                    //Chequeo si el participante es Amigo del Autor o si el autor del comentario es el autor de la publicacion.
                    if (Autor.EsAmigo(participante) || Autor.Equals(participante))
                    {
                        puedeParticipar = true;
                    }
                }
                else
                {
                    puedeParticipar = true;
                }
            }
            return puedeParticipar;
        }

        public bool ValidateVisualizacion(Miembro visualizador)
        {
            bool puedeVisualizar = false;
            //Chequeo privacidad publicacion primero si es Publica entonces puede participar.
            if (_privacidad == Privacidad.PRIVADO)
            {
                //Chequeo si el visualizador es Amigo del Autor o si el autor del comentario es el autor de la publicacion.
                if (Autor.EsAmigo(visualizador) || Autor.Equals(visualizador))
                    {
                    puedeVisualizar = true;
                    }
            } 
            else
            {
                puedeVisualizar = true;
            }
            return puedeVisualizar;
        }

        public void AltaReaccion(Reaccion reaccion)
        {
            //Verifica que el miembro no haya ya reaccionado a la publicacion y Que puede participar en la misma.
            if (ValidateParticipacion(reaccion.Autor))
            {
                if (DevolverReaccionOpuesta(reaccion) != null)
                {
                    _reacciones.Add(reaccion);
                    _reacciones.Remove(DevolverReaccionOpuesta(reaccion));
                }
                else if (!_reacciones.Contains(reaccion))
                {
                    _reacciones.Add(reaccion);
                }
                else if(_reacciones.Contains(reaccion))
                {
                    _reacciones.Remove(reaccion);
                }                 
            } 
            else
            {
                throw new Exception("Usted no puede participar");
            }
        }

        public Reaccion DevolverReaccionOpuesta(Reaccion reaccionPorAgregar)
        {
            Reaccion reaccionOpuesta = null;
            int i = 0;
            while(i < _reacciones.Count && reaccionOpuesta == null)
            {
                if(reaccionPorAgregar.Autor == _reacciones[i].Autor && _reacciones[i].TipoReaccion != reaccionPorAgregar.TipoReaccion)
                {
                    reaccionOpuesta = _reacciones[i];
                }
                i++;
            }
            return reaccionOpuesta;
        }
        public void Validate()
        {
            if (_titulo.Length < 3 || _titulo.Trim().Length == 0)
            {
                throw new Exception(" El titulo no puede tener menos de tres caracteres.");
            }
            if (_autor == null)
            {
                throw new Exception("El Autor no existe");
            }
            if(_autor.Bloqueado)
            {
                throw new Exception("Usted esta bloqueado y no puede comentar o realizar publicaciones");
            }
        }
        public override string ToString()
        {
            string contenido = _contenido;
            if(contenido.Length > 50)
            {
                contenido = contenido.Substring(0, 50);
            }
            return $"\nID:{_id}\nAutor: {_autor.Email}\nTitulo: {_titulo}\nContenido:{contenido}\nFecha Publicacion: {_fechaPublicacion}";
        }

            public bool MiembroReaccionoMG(string email)
        {
            bool participo = false;
            int i = 0;
            while(i < _reacciones.Count && participo == false)
            {
                if (_reacciones[i].TipoReaccion == TipoReaccion.ME_GUSTA && _reacciones[i].Autor.Email == email)
                {
                    participo = true;
                }
                i++;
            }
            return participo;
        }

        public bool MiembroReaccionoNMG(string email)
        {
            bool participo = false;
            int i = 0;
            while (i < _reacciones.Count && participo == false)
            {
                if (_reacciones[i].TipoReaccion == TipoReaccion.NO_ME_GUSTA && _reacciones[i].Autor.Email == email)
                {
                    participo = true;
                }
                i++;
            }
            return participo;
        }

        #endregion
    }
}


