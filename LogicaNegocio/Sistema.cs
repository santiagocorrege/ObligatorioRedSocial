using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace LogicaNegocio
{
    public class Sistema
    {
        private List<Usuario> _usuarios = new List<Usuario>();
        private List<Publicacion> _publicaciones = new List<Publicacion>();
        private static Sistema _instancia;
        private Sistema()
        {
            PreCargaDatos();
        }

        #region Properties
        public static Sistema Instancia
        {
            get
            {
                if (_instancia == null)
                {
                    _instancia = new Sistema();
                }
                return _instancia;
            }
        }

        public List<Usuario> Usuarios { get => _usuarios; }

        public List<Publicacion> Publiciones { get => _publicaciones; }

        #endregion

        #region Methods
        public void AltaUsuario(Usuario user)
        {
            user.Validate();
            if (!_usuarios.Contains(user))
            {
                _usuarios.Add(user);
            } else
            {
                throw new Exception("El usuario ya existe");
            }
        }

        public void AltaSolicitud(Miembro miembroLogueado, string emailSolicitado)
        {
            Miembro solicitado = BuscarMiembro(emailSolicitado);
            if(solicitado != null)
            {
                solicitado.AltaSolicitud(new Solicitud(miembroLogueado));
            }else
            {
                throw new Exception("No existe ese miembro");
            }
        }
        public void AltaPost(Post post)
        {
            post.Validate();
            _publicaciones.Add(post);
        }

        public void AltaComentario(Post post, Comentario comentario)
        {
            //Agrega el comentario a la publicacion y Verifica validaciones de comentario,
            //participacion etc, de lo contrario lanza una excepcion
            post.AltaComentario(comentario);
            _publicaciones.Add(comentario);
        }

        //Recorre lista de usuarios de sistema y devuelve una lista con los usuarios que son miembros
        //Para luego poder trabajar con los miembros usuarios en sistema y mostrarlos en program y trabajar en precarga (Solicitudes...)
        public List<Miembro> DevolverUsuariosMiembros()
        {
            List<Miembro> miembros = new List<Miembro>();
            for (int i = 0; i < _usuarios.Count; i++)
            {
                if (_usuarios[i] != null && _usuarios[i] is Miembro)
                {
                    Miembro miembro = (Miembro)_usuarios[i];
                    miembros.Add(miembro);
                }
            }
            return miembros;
        }

        public List<Post> DevolverPosts()
        {
            List<Post> posts = new List<Post>();
            for (int i = 0; i < _publicaciones.Count; i++)
            {
                if (_publicaciones[i] is Post)
                {
                    Post post = (Post)_publicaciones[i];
                    posts.Add(post);
                }
            }
            return posts;
        }

        public Publicacion BuscarPublicacion(int id)
        {
            Publicacion publicacionBuscado = null;
            int i = 0;
            while (i < _publicaciones.Count && publicacionBuscado == null)
            {
                if (_publicaciones[i].Id == id)
                {
                    publicacionBuscado = _publicaciones[i];
                }
                i++;
            }
            return publicacionBuscado;
        }

        public Post BuscarPost(int id)
        {
            Post postBuscado = null;
            List<Post> posts = DevolverPosts();
            int i = 0;
            while(i < posts.Count && postBuscado == null)
            {
                if (posts[i].Id  == id)
                {
                    postBuscado = posts[i];
                }
                i++;
            }
            return postBuscado;
        }

        public Miembro BuscarMiembro(string email)
        {
            List<Miembro> miembros = DevolverUsuariosMiembros();
            Miembro miembroBuscado = null;
            int i = 0;
            if(email !=null)
            {
                while (i < miembros.Count && miembroBuscado == null)
                {
                    if (miembros[i].Email.Trim().ToUpper() == email.Trim().ToUpper())
                    {
                        miembroBuscado = miembros[i];
                    }
                    i++;
                }
            }
            return miembroBuscado;
        }
        public Administrador BuscarAdministrador(string email)
        {
            Administrador administradorBuscado = null;
            int i = 0;
            if (email != null)
            {
                while (i < _usuarios.Count && administradorBuscado == null)
                {
                    if (_usuarios[i].Email.Trim().ToUpper() == email.Trim().ToUpper() && _usuarios[i] is Administrador)
                    {
                        administradorBuscado = (Administrador)_usuarios[i];
                    }
                    i++;
                }
            }
            return administradorBuscado;
        }


        public Usuario ComprobarUsuario(string nombreUsuario, string contrasena)
        {
            Usuario usuarioBuscado = null;
            int i = 0;
            while(i < _usuarios.Count && usuarioBuscado == null)
            {
                if (_usuarios[i].Password == contrasena.Trim() && _usuarios[i].Email.Trim() == nombreUsuario.Trim())
                {
                    usuarioBuscado = _usuarios[i];
                }
                i++;
            }
            return usuarioBuscado;
        }

        //REQUERIMIENTOS FUNCIONALES ADMINISTRADOR
        //RF1 Administrador:  Listar Usuarios Miembro por apellido y nombre ascendentemente
        public List<Miembro> DevolverUsuariosMiembroOrdenados()
        {
            List<Miembro> miembros = DevolverUsuariosMiembros();
            miembros.Sort();
            return miembros;
        }

        //RF2 Administador: Censurar Miembro

        public void BloquearMiembro(Miembro miembro)
        {
            if (miembro == null)
            {
                throw new Exception("El miembro no existe");
            }
            if (miembro.Bloqueado)
            {
                throw new Exception("El miembro ya esta bloqueado");
            }
            else
            {
                miembro.Bloqueado = true;
            }
        }

        //RF3 Administrador : Censurar Post

        public void CensurarPost(Post post)
        {
            if(post == null)
            {
                throw new Exception("El post no existe");
            }
            if (post.Censurado)
            {
                throw new Exception("El post ya esta baneado");
            }
            else
            {
                post.Censurado = true;
            }
        }

        //REQUERIMIENTOS FUNCIONALES MIEMBRO
        //RF01
        public List<Post> MostrarMuro(Miembro miembro)
        {
            List<Post> posts = DevolverPosts();
            List<Post> muro = new List<Post>();
            foreach(Post post in posts)
            {
                if (post.ValidateVisualizacion(miembro))
                {
                    muro.Add(post);
                }
            }
            return muro;
        }

        public List<Solicitud> DevolverSolicitudesPendientesMiembro(Miembro miembro)
        {
            return miembro.DevolverSolicitudesPendientesMiembro();
        }


        //Aceptar Solicitud

        public void AceptarSolicitudMiembro(string email, int id)
        {
            Miembro miembroLogueado = BuscarMiembro(email);
            Solicitud solicitud = miembroLogueado.BuscarSolicitud(id);
            if (solicitud != null && miembroLogueado != null)
            {
                miembroLogueado.AceptarSolicitud(solicitud);
            }
            else
            {
                //Teoricamente no va a suceder nunca la excepcion ya que nosotros listamos las solicitudes
                //Con sus correspondientes enlaces vinculados a los id de la solicitud
                throw new Exception("No existe la solicitud");
            }
        }

        public void RechazarSolicitudMiembro(string email, int id)
        {
            Miembro miembroLogueado = BuscarMiembro(email);
            Solicitud solicitud = miembroLogueado.BuscarSolicitud(id);
            if (solicitud != null && miembroLogueado != null)
            {
                miembroLogueado.RechazarSolicitud(solicitud);
            } else
            {
                //Teoricamente no va a suceder nunca la excepcion ya que nosotros listamos las solicitudes
                //Con sus correspondientes enlaces vinculados a los id de la solicitud
                throw new Exception("No existe la solicitud");
            }
        }

        public List<Miembro> DevolverMiembrosNoAmigos(Miembro miembro)
        {
            List<Miembro> miembros = DevolverUsuariosMiembros();
            List<Miembro> miembrosNoAmigos = null;
            if (miembro != null)
            {
                miembrosNoAmigos = miembro.MiembrosNoAmigos(miembros);
                //Dejar de listar aquellos que les he enviado solicitud);
            }
            return miembrosNoAmigos;
        }

        public List<Publicacion> FiltrarPublicaciones(string contenido, int valorAceptacion, Miembro miembroLogueado)
        {
            List<Publicacion> publicacionesFiltradas = new List<Publicacion>();
            List<Post> postAccesoMiembro = MostrarMuro(miembroLogueado);
            foreach(Post post in postAccesoMiembro)
            {
                if (post.Contenido.ToUpper().Contains(contenido.ToUpper()) && post.CalcularAceptacion() >= valorAceptacion)
                {
                    publicacionesFiltradas.Add(post);
                }
                foreach(Comentario comentario in post.Comentarios)
                {
                    if (comentario.Contenido.ToUpper().Contains(contenido.ToUpper()) && comentario.CalcularAceptacion() >= valorAceptacion)
                    {
                        publicacionesFiltradas.Add(comentario);
                    }
                    
                }
            }
            return publicacionesFiltradas;
        }


        //TERMINAN RF

        private void PreCargaDatos()
        {
            PrecargaUsuariosMiembros();
            //Hardcodeado de 1 Administrador
            AltaUsuario(new Administrador("admin@gmail.com", "pass1234"));
            PrecargaSolicitudes();
            PrecargaPublicacionesComentariosYReacciones();
            // FALTAN REACCIONES
        }
        private void PrecargaUsuariosMiembros()
        {
            //Hardcodeado de Usuarios miembros a la lista _usuarios de Sistema
            AltaUsuario(new Miembro("test1@gmail.com", "pass1234", "testNomJuan", "testAppPeri", new DateTime(2001, 01, 01)));
            AltaUsuario(new Miembro("test2@gmail.com", "pass1234", "testNomPedro", "testAppSomalia", new DateTime(2002, 01, 01)));
            AltaUsuario(new Miembro("test3@gmail.com", "pass1234", "testNomPedro", "testAppAlvarez", new DateTime(2003, 01, 01)));
            AltaUsuario(new Miembro("test4@gmail.com", "pass1234", "testNomMariana", "testAppJuanes", new DateTime(2004, 01, 01)));
            AltaUsuario(new Miembro("test5@gmail.com", "pass1234", "testNomHugo", "testApp", new DateTime(2005, 01, 01)));
            AltaUsuario(new Miembro("test6@gmail.com", "pass1234", "testNomLeona", "testApp", new DateTime(2006, 01, 01)));
            AltaUsuario(new Miembro("test7@gmail.com", "pass1234", "testNomMariana", "testAppFernandez", new DateTime(2007, 01, 01)));
            AltaUsuario(new Miembro("test8@gmail.com", "pass1234", "testNomJuani", "testApp", new DateTime(2008, 01, 01)));
            AltaUsuario(new Miembro("test9@gmail.com", "pass1234", "testNomElizabeth", "testApp", new DateTime(2009, 01, 01)));
            AltaUsuario(new Miembro("test10@gmail.com", "pass1234", "testNomCarolina", "testApp", new DateTime(2010, 01, 01)));
        }

        private void PrecargaSolicitudes()
        {
            List<Miembro> miembros = DevolverUsuariosMiembros();

            //Se que mi lista va a tener 10 miembros porque precargargamos 10 miembros
            //Invitaciones de todos a Miembro test1@gmail.com
            BuscarMiembro("test1@gmail.com").AltaSolicitud(new Solicitud(BuscarMiembro("test2@gmail.com")));
            BuscarMiembro("test1@gmail.com").AltaSolicitud(new Solicitud(BuscarMiembro("test3@gmail.com")));
            BuscarMiembro("test1@gmail.com").AltaSolicitud(new Solicitud(BuscarMiembro("test4@gmail.com")));
            BuscarMiembro("test1@gmail.com").AltaSolicitud(new Solicitud(BuscarMiembro("test5@gmail.com")));
            BuscarMiembro("test1@gmail.com").AltaSolicitud(new Solicitud(BuscarMiembro("test6@gmail.com")));
            BuscarMiembro("test1@gmail.com").AltaSolicitud(new Solicitud(BuscarMiembro("test7@gmail.com")));
            BuscarMiembro("test1@gmail.com").AltaSolicitud(new Solicitud(BuscarMiembro("test8@gmail.com")));
            BuscarMiembro("test1@gmail.com").AltaSolicitud(new Solicitud(BuscarMiembro("test9@gmail.com")));
            BuscarMiembro("test1@gmail.com").AltaSolicitud(new Solicitud(BuscarMiembro("test10@gmail.com")));

            //Invitaciones de todos a Miembro test2@gmail.com menos de Miembro test1@gmail.com
            BuscarMiembro("test2@gmail.com").AltaSolicitud(new Solicitud(BuscarMiembro("test3@gmail.com")));
            BuscarMiembro("test2@gmail.com").AltaSolicitud(new Solicitud(BuscarMiembro("test4@gmail.com")));
            BuscarMiembro("test2@gmail.com").AltaSolicitud(new Solicitud(BuscarMiembro("test5@gmail.com")));
            BuscarMiembro("test2@gmail.com").AltaSolicitud(new Solicitud(BuscarMiembro("test6@gmail.com")));
            BuscarMiembro("test2@gmail.com").AltaSolicitud(new Solicitud(BuscarMiembro("test7@gmail.com")));
            BuscarMiembro("test2@gmail.com").AltaSolicitud(new Solicitud(BuscarMiembro("test8@gmail.com")));
            BuscarMiembro("test2@gmail.com").AltaSolicitud(new Solicitud(BuscarMiembro("test9@gmail.com")));
            BuscarMiembro("test2@gmail.com").AltaSolicitud(new Solicitud(BuscarMiembro("test10@gmail.com")));

            //Aceptacion de solicitudes enviadas a miembro test1@gmail.com
            BuscarMiembro("test1@gmail.com").AceptarSolicitud(BuscarMiembro("test1@gmail.com").Solicitudes[0]);
            BuscarMiembro("test1@gmail.com").AceptarSolicitud(BuscarMiembro("test1@gmail.com").Solicitudes[1]);
            BuscarMiembro("test1@gmail.com").AceptarSolicitud(BuscarMiembro("test1@gmail.com").Solicitudes[2]);
            BuscarMiembro("test1@gmail.com").AceptarSolicitud(BuscarMiembro("test1@gmail.com").Solicitudes[3]);
            BuscarMiembro("test1@gmail.com").AceptarSolicitud(BuscarMiembro("test1@gmail.com").Solicitudes[4]);
            BuscarMiembro("test1@gmail.com").AceptarSolicitud(BuscarMiembro("test1@gmail.com").Solicitudes[5]);
            BuscarMiembro("test1@gmail.com").AceptarSolicitud(BuscarMiembro("test1@gmail.com").Solicitudes[6]);
            BuscarMiembro("test1@gmail.com").AceptarSolicitud(BuscarMiembro("test1@gmail.com").Solicitudes[7]);
            BuscarMiembro("test1@gmail.com").AceptarSolicitud(BuscarMiembro("test1@gmail.com").Solicitudes[8]);

            //Aceptacion de solicitudes enviadas a miembro test2@gmail.com
            BuscarMiembro("test2@gmail.com").AceptarSolicitud(BuscarMiembro("test2@gmail.com").Solicitudes[0]);
            BuscarMiembro("test2@gmail.com").AceptarSolicitud(BuscarMiembro("test2@gmail.com").Solicitudes[1]);
            BuscarMiembro("test2@gmail.com").AceptarSolicitud(BuscarMiembro("test2@gmail.com").Solicitudes[2]);
            BuscarMiembro("test2@gmail.com").AceptarSolicitud(BuscarMiembro("test2@gmail.com").Solicitudes[3]);
            BuscarMiembro("test2@gmail.com").AceptarSolicitud(BuscarMiembro("test2@gmail.com").Solicitudes[4]);
            BuscarMiembro("test2@gmail.com").AceptarSolicitud(BuscarMiembro("test2@gmail.com").Solicitudes[5]);
            BuscarMiembro("test2@gmail.com").AceptarSolicitud(BuscarMiembro("test2@gmail.com").Solicitudes[6]);
            BuscarMiembro("test2@gmail.com").AceptarSolicitud(BuscarMiembro("test2@gmail.com").Solicitudes[7]);

            //Creacion invitacion de miembro test5@gmail.com a miembro test3@gmail.com para que la rechace
            BuscarMiembro("test3@gmail.com").AltaSolicitud(new Solicitud(BuscarMiembro("test5@gmail.com")));

            //Rechazo invitacion de miembro test5@gmail.com a miembro test3@gmail.com 
            BuscarMiembro("test3@gmail.com").RechazarSolicitud(BuscarMiembro("test3@gmail.com").Solicitudes[0]);

            //Creacion invitacion de miembro test6@gmail.com a miembro test3@gmail.com para que quede pendiente
            BuscarMiembro("test3@gmail.com").AltaSolicitud(new Solicitud(BuscarMiembro("test6@gmail.com")));
            BuscarMiembro("test3@gmail.com").AltaSolicitud(new Solicitud(BuscarMiembro("test7@gmail.com")));
            BuscarMiembro("test3@gmail.com").AltaSolicitud(new Solicitud(BuscarMiembro("test8@gmail.com")));
            BuscarMiembro("test3@gmail.com").AltaSolicitud(new Solicitud(BuscarMiembro("test9@gmail.com")));


            //TESTEAR VISUALIZACION DEL POST PRIVADO DEL USUARIO 7. ALTA SOLICITUD DE USUARIO 7 A USUARIO 6
            BuscarMiembro("test7@gmail.com").AltaSolicitud(new Solicitud(BuscarMiembro("test6@gmail.com")));

            //ACEPTACION DE LA SOLICITUD 
            BuscarMiembro("test7@gmail.com").AceptarSolicitud(BuscarMiembro("test7@gmail.com").Solicitudes[0]);
        }

        private void PrecargaPublicacionesComentariosYReacciones()
        {
            //Filtracion de Usuarios a Miembros (Tengo 10 miembros)
            List<Miembro> miembros = DevolverUsuariosMiembros();

            //Precarga de Post: 1 Por cada miembro de miembro 1 a miembro 6
            AltaPost(new Post(BuscarMiembro("test1@gmail.com"), "AMi post1", "Esta es mi primer post", "1.jpg", Privacidad.PUBLICO));
            AltaPost(new Post(BuscarMiembro("test2@gmail.com"), "CMi post1", "Esta es mi primer post", "2.jpg", Privacidad.PUBLICO));
            AltaPost(new Post(BuscarMiembro("test4@gmail.com"), "BMi post1", "Esta es mi primer post", "4.jpg", Privacidad.PUBLICO));
            AltaPost(new Post(BuscarMiembro("test5@gmail.com"), "NMi post1", "101010101020202020203030303030404040404050505050506060606060", "5.jpg", Privacidad.PUBLICO));
            AltaPost(new Post(BuscarMiembro("test6@gmail.com"), "YMi post1", "Esta es mi primer post", "6.jpg", Privacidad.PUBLICO));
            AltaPost(new Post(BuscarMiembro("test6@gmail.com"), "BMi post2", "Esta es mi segundo post", "7.jpg", Privacidad.PUBLICO));
            
            //TESTEAR VISUALIZACION DE ESTE POST
            AltaPost(new Post(BuscarMiembro("test7@gmail.com"), "BMi post2", "TESTEO PRIVACIDAD", "8.jpg", Privacidad.PRIVADO));

            //Precarga de Comentarios.
            List<Post> posts = DevolverPosts();

            //Comentario hecho por Miembro test3@gmail.com hecho a Publicacion 0,1,2,3 y 4
            AltaComentario(posts[0], new Comentario(BuscarMiembro("test3@gmail.com"), "TituloComentario1A", "contenidocomentario1A", Privacidad.PUBLICO));
            AltaComentario(posts[1], new Comentario(BuscarMiembro("test3@gmail.com"), "TituloComentario1B", "contenidocomentario1B", Privacidad.PUBLICO));
            AltaComentario(posts[2], new Comentario(BuscarMiembro("test3@gmail.com"), "TituloComentario1C", "contenidocomentario1C", Privacidad.PUBLICO));
            AltaComentario(posts[3], new Comentario(BuscarMiembro("test3@gmail.com"), "TituloComentario1D", "contenidocomentario1D", Privacidad.PUBLICO));
            AltaComentario(posts[4], new Comentario(BuscarMiembro("test3@gmail.com"), "TituloComentario1E", "contenidocomentario1E", Privacidad.PUBLICO));

            //Comentario hecho por Miembro test4@gmail.com hecho a Publicacion 0,1,2,3 y 4
            AltaComentario(posts[0], new Comentario(BuscarMiembro("test4@gmail.com"), "TituloComentario2F", "contenidocomentario2F", Privacidad.PUBLICO));
            AltaComentario(posts[1], new Comentario(BuscarMiembro("test4@gmail.com"), "TituloComentario2G", "contenidocomentario2G", Privacidad.PUBLICO));
            AltaComentario(posts[2], new Comentario(BuscarMiembro("test4@gmail.com"), "TituloComentario2H", "contenidocomentario2H", Privacidad.PUBLICO));
            AltaComentario(posts[3], new Comentario(BuscarMiembro("test4@gmail.com"), "TituloComentario2I", "contenidocomentario2I", Privacidad.PUBLICO));
            AltaComentario(posts[4], new Comentario(BuscarMiembro("test4@gmail.com"), "TituloComentario2J", "contenidocomentario2J", Privacidad.PUBLICO));

            //Comentario hecho por Miembro test5@gmail.com hecho a Publicacion 0,1,2,3 y 4
            AltaComentario(posts[0], new Comentario(BuscarMiembro("test5@gmail.com"), "TituloComentario3K", "contenidocomentario3K", Privacidad.PUBLICO));
            AltaComentario(posts[1], new Comentario(BuscarMiembro("test5@gmail.com"), "TituloComentario3L", "contenidocomentario3L", Privacidad.PUBLICO));
            AltaComentario(posts[2], new Comentario(BuscarMiembro("test5@gmail.com"), "TituloComentario3M", "contenidocomentario3M", Privacidad.PUBLICO));
            AltaComentario(posts[3], new Comentario(BuscarMiembro("test5@gmail.com"), "TituloComentario3N", "contenidocomentario3N", Privacidad.PUBLICO));
            AltaComentario(posts[4], new Comentario(BuscarMiembro("test5@gmail.com"), "TituloComentario3O", "contenidocomentario3O", Privacidad.PUBLICO));

            //Reacciones de test3@gmail.com,test4@gmail.com a Post 0 de Miembro 0 Post 0 de Miembro 1
            posts[0].AltaReaccion(new Reaccion(BuscarMiembro("test3@gmail.com"), TipoReaccion.ME_GUSTA));
            posts[1].AltaReaccion(new Reaccion(BuscarMiembro("test4@gmail.com"), TipoReaccion.NO_ME_GUSTA));

            //Reaccion de Miembro test3@gmail.com a Comentario 0 de test3@gmail.com y Reaccion de test4@gmail.com a Comentario 0 de test3@gmail.com en otra publicacion
            posts[0].Comentarios[0].AltaReaccion(new Reaccion(BuscarMiembro("test3@gmail.com"), TipoReaccion.ME_GUSTA));
            posts[1].Comentarios[0].AltaReaccion(new Reaccion(BuscarMiembro("test4@gmail.com"), TipoReaccion.NO_ME_GUSTA));
        }
        //Filtra post de publicaciones




        #endregion
    }
}
