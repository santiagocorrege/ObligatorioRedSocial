using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LogicaNegocio.Interface;

namespace LogicaNegocio
{
    public class Miembro : Usuario, IValidate, IComparable<Miembro>
    {
        private string _nombre;
        private string _apellido;
        private DateTime _fechaNacimiento;
        private List<Miembro> _amigos = new List<Miembro>();
        private List<Solicitud> _solicitudes = new List<Solicitud>();
        private bool _bloqueado = false;

        public Miembro (string email, string password, string nombre, string apellido, DateTime fechaNacimiento) : base (email, password)
        {
            _nombre = nombre;
            _apellido = apellido;
            _fechaNacimiento = fechaNacimiento;
        }

        public Miembro() { }
        #region Properties

        public DateTime FechaNacimiento { get => _fechaNacimiento; set => _fechaNacimiento = value; }
        public string Nombre { get => _nombre; set => _nombre = value; }

        public string Apellido { get => _apellido;  set => _apellido = value; }
        
        public List<Solicitud> Solicitudes { get => _solicitudes;}

        public List<Miembro> Amigos { get => _amigos; }

        public bool Bloqueado { get => _bloqueado; set => _bloqueado = value; }

        #endregion

        #region Methods
        public List<Solicitud> DevolverSolicitudesPendientesMiembro()
        {
            List<Solicitud> solicitudes = new();
            foreach (Solicitud solicitud in _solicitudes)
            {
                if (solicitud.Estado == Estado.PENDIENTE_APROBACION)
                {
                    solicitudes.Add(solicitud);
                }
            }
            return solicitudes;
        }
        public Solicitud BuscarSolicitud(int id)
        {
            Solicitud solicitudBuscada = null;
            int i = 0;
            while (i < _solicitudes.Count && solicitudBuscada == null)
            {
                if (_solicitudes[i].Id == id)
                {
                    solicitudBuscada = _solicitudes[i];
                }
                i++;
            }
            return solicitudBuscada;
        }

        public void AltaSolicitud(Solicitud solicitud)
        {
            //Este contains utiliza el metodo equals de solicitud que compara el solicitante de la solicitud.
            if(solicitud.Solicitante == null) 
            {
                throw new Exception("El usuario no existe");
            }
            if (!_solicitudes.Contains(solicitud))
            {
                _solicitudes.Add(solicitud);
            }
            else
            {
                throw new Exception("Ya existe la solicitud");
            }
        }
        public void AceptarSolicitud(Solicitud solicitud)
        {
            if (_solicitudes.Contains(solicitud) && solicitud.Estado == Estado.PENDIENTE_APROBACION && _bloqueado == false)
            {
                solicitud.Estado = Estado.APROBADA;
                _amigos.Add(solicitud.Solicitante);
                solicitud.Solicitante._amigos.Add(this);
            } 
            else
            {
                throw new Exception("Esa solicitud no existe o ya fue procesada");
            }
        }

        public void RechazarSolicitud(Solicitud solicitud)
        {
            if (_solicitudes.Contains(solicitud) && solicitud.Estado == Estado.PENDIENTE_APROBACION && _bloqueado == false)
            {
                solicitud.Estado = Estado.RECHAZADA;
            }
            else
            {
                throw new Exception("Esa solicitud no existe o ya fue procesada");
            }
        }
        //Devuelve una lista con los miembros que no son amigos, o que no hayan enviado ya una solicitud
        public List<Miembro> MiembrosNoAmigos(List<Miembro> miembros)
        {
            List<Miembro> miembrosNoAmigos = new List<Miembro>();
            foreach(Miembro miembro in miembros)
            {
                if (!_amigos.Contains(miembro) && !miembro.Equals(this) && !ExisteSolicitudPendiente(miembro))
                {
                    miembrosNoAmigos.Add(miembro);
                }
            }
            return miembrosNoAmigos;
        }
        //Verifica que no exista una solicitud pendiente de ninguna de las partes para asi evitar mostrar el usuario al intentar agregar un amigo
        public bool ExisteSolicitudPendiente(Miembro miembroSolicitando)
        {
            bool existe = false;
            Solicitud solicitudUsuarioLogueado = new Solicitud(this);
            Solicitud solicitudMiembroNoAmigo = new Solicitud(miembroSolicitando);
            if (this._solicitudes.Contains(solicitudMiembroNoAmigo) || miembroSolicitando.Solicitudes.Contains(solicitudUsuarioLogueado))
            {
                existe = true;

            }
            return existe;
        }
        public bool EsAmigo(Miembro miembro)
        {
            bool esAmigo = false;
            int contador = 0;
            while(contador < _amigos.Count && esAmigo == false)
            {
                if (_amigos[contador].Equals(miembro))
                {
                    esAmigo = true;
                }
                contador++;
            }
                return esAmigo;
            }

        public override string ToString()
        {
            return base.ToString() + $"Nombre: {_nombre}\nApellido: {_apellido}\nFecha Nacimiento: {_fechaNacimiento}";
        }

        public void Validate()
        {
            base.Validate();

            if (_nombre == null || _nombre.Trim().Length == 0)
            {
                throw new Exception("El nombre no puede estar vacio");
            }
            if (_apellido == null || _apellido.Trim().Length == 0)
            {
                throw new Exception("El apellido no puede estar vacio");
            }
            if(_fechaNacimiento > DateTime.Now ||  _fechaNacimiento == DateTime.MinValue)
            {
                throw new Exception("La fecha de nacimiento no es valida");
            }
        }

        public int CompareTo(Miembro other)
        {
            int criterio = 0;
            criterio = other._nombre.CompareTo(_nombre) * -1;
            if(criterio == 0)
            {
                criterio = other._apellido.CompareTo(_apellido) * -1;
            }
            return criterio;
        }



        public override string Rol()
        {
            return "MIEMBRO";
        }

        #endregion
    }
}
