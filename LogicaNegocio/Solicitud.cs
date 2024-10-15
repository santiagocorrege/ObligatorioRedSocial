using LogicaNegocio.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicaNegocio
{
    public enum Estado
    {
        PENDIENTE_APROBACION,
        APROBADA,
        RECHAZADA,
    }
    public class Solicitud : IEquatable<Solicitud>
    {
        private int _id;
        private static int s_IdSecuencial;
        private Estado _estado = Estado.PENDIENTE_APROBACION;
        private DateTime _fechaSolicitud = DateTime.Now;
        private Miembro _miembroSolicitante;

        public Solicitud(Miembro miembroSolicitante)
        {
            _id = s_IdSecuencial++;
            _miembroSolicitante = miembroSolicitante;
        }

        public int Id { get { return _id; } }
        public Miembro Solicitante { get => _miembroSolicitante; }

        public Estado Estado
        {
            get => _estado;
            set => _estado = value;
        }



        //Si el estado de la solicitud es Rechazada entonces se puede realizar una nueva solicitud. _estado != Estado.RECHAZADA (Esto no lo exige la letra se podria agregar
        //Verifica que el miembro solicitante no haya procesado una solicitud anteriormente
        public bool Equals(Solicitud? other)
        {
            return (_miembroSolicitante.Equals(other._miembroSolicitante) && _estado == Estado.PENDIENTE_APROBACION) || (_miembroSolicitante.Equals(other._miembroSolicitante) && _estado == Estado.APROBADA); // Usa el Equals o tengo que hacer .Equals?
        }

        public override string ToString()
        {
            return $"\n\n--SOLICITUD {_id}--\nEstado: {_estado}\nFecha: {_fechaSolicitud}\nSolicitante:{_miembroSolicitante.Email}";
        }

    }
}
