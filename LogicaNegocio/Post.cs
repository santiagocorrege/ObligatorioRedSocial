using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LogicaNegocio.Interface;

namespace LogicaNegocio
{

    public class Post : Publicacion, IValidate, IComparable<Post>
    {
        private string _imagen;
        private List<Comentario> _comentarios = new List<Comentario>();
        private bool _censurado = false;
        private static decimal s_ValorPrivacidad = 10;

        public Post(Miembro autor, string titulo, string contenido, string imagen, Privacidad privacidad) : base(autor, titulo, contenido, privacidad)
        {
            _imagen = imagen;
        }

        public string Imagen { get => _imagen; }
        public bool Censurado { get => _censurado; set => _censurado = value; }
        public List<Comentario> Comentarios { get => _comentarios; }
        #region Methods

        public override decimal CalcularAceptacion()
        {
            decimal valorAceptacion = base.CalcularAceptacion();
            if (Privacidad == Privacidad.PUBLICO)
            {
                valorAceptacion += s_ValorPrivacidad;
            }
            return valorAceptacion;
        }

        public List<Comentario> ComentariosMiembro(Miembro miembro)
        {
            List<Comentario> comentariosMiembro = new List<Comentario>();
            for(int i = 0; i < _comentarios.Count; i++)
            {
                if (_comentarios[i].Autor.Equals(miembro))
                {
                    comentariosMiembro.Add(_comentarios[i]);
                }
            }
            return comentariosMiembro;
        }
        public void AltaComentario(Comentario comentario)
        {
            comentario.Validate();
            //Verifica que la publicacion no este censurada y que el Autor del comentario pueda participar en el post (Metodo Publicacion)
            if (_censurado == false && ValidateParticipacion(comentario.Autor))
            {
                //Le asigna con el accesor set the Privacidad del comentario la Privacidad del Post (this.Privacidad => Privacidad)
                comentario.Privacidad = Privacidad;
                _comentarios.Add(comentario);
            }
            else
            {
                throw new Exception("La publicacion se encuentra censurada o usted no puede participar");
            }
        }

        public void Validate()
        {
            //Verifica las reglas de negocio de Publicacion (Padre)
            base.Validate();
            //Debe ser mayor o igual a 5 porque la extencion consta de 4 caracteres .jpg/.png
            if (_imagen.Trim().Length < 5)
            {
                throw new Exception("El nombre de la imagen tiene que tener más de cuatro caractares.");
            }
            
            if(ValidateFormato() == false)
            {
                throw new Exception("El formato de la imagen no es correcto.");
            }
        }

        public bool ValidateVisualizacion(Miembro visualizador)
        {
            return base.ValidateVisualizacion(visualizador) && !_censurado;
        }
            private bool ValidateFormato()
        {
            string formato = "";
            for(int i = _imagen.Length - 4; i < _imagen.Length; i++)
            {
                formato += _imagen[i];
            }
            return formato == ".jpg" || formato == ".png";
        }

        public int CompareTo(Post? other)
        {
            return other.Titulo.CompareTo(Titulo);
        }

        public override string ToString()
        {
            return base.ToString() + $"\nImagen: {_imagen}\nPrivacidad: {Privacidad}\nValor Aceptacion:{CalcularAceptacion()}";
        }

        #endregion
    }


}
