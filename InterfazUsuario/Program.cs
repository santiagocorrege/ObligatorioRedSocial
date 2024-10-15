using LogicaNegocio;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace InterfazUsuario
{
    internal class Program
    {
        static Sistema miSistema = new Sistema();
        static Administrador adminTest = new Administrador("asdasd@gmail.com", "asdasd123");
        static Miembro miembroTest = miSistema.BuscarMiembro("test5@gmail.com");
        static void Main(string[] args)
        {
            //MostrarDatosPreCargados();
            //MenuAdministrador();
            //Menu();
            MenuMiembro();
        }
        
        


        //****************ADMINISTRADOR***************
        static void MenuAdministrador()
        {
            int opcion = -1;
            while (opcion != 0)
            {
                Console.Clear();
                Console.WriteLine("====MENU===");
                Console.WriteLine("1- Mostrar Miembros ordenados por nombre y apellido ascendente");
                Console.WriteLine("2- Bloquear Usuario");
                Console.WriteLine("3- Banear Post");
                Console.WriteLine("0- Salir");
                int.TryParse(Console.ReadLine(), out opcion);
                SeleccionMenuAdministrador(opcion);
            }
        }

        static void SeleccionMenuAdministrador(int opcion)
        {
            switch (opcion)
            {
                case 1:
                    MostrarMiembrosOrdenados();
                    break;
                case 2:
                    BloquearUsuario();
                    break;
                case 3:
                    BanearPost();
                    break;
                default:
                    break;
            }
        }

        
        static void MostrarMiembrosOrdenados()
        {
            List<Miembro> miembros = miSistema.DevolverUsuariosMiembroOrdenados();
            if (miembros.Count > 0)
            {
                foreach (Miembro miembro in miembros)
                {
                    Console.WriteLine(miembro);
                }
            } else
            {
                Console.WriteLine("No existen miembros...");
            }
            
            Console.ReadKey();
        }

        static void BloquearUsuario()
        {
            Miembro usuarioBloqueado = null;
            Console.WriteLine("Ingrese el email del usuario");
            string email = Console.ReadLine();
            usuarioBloqueado = miSistema.BuscarMiembro(email.Trim());
            try
            {
                if (usuarioBloqueado != null)
                {
                    //usuarioBloqueado.Bloqueado = true;
                    miSistema.CensurarMiembro(usuarioBloqueado);
                    Console.WriteLine($"Miembro {usuarioBloqueado.Email} censurado...");
                    Console.ReadKey();
                }
                else
                {
                    throw new Exception("El usuario no existe");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            Console.ReadKey();
        }

        static void BanearPost()
        {
            Post postBaneado = null;
            Console.WriteLine("Ingrese Id Post:");
            int.TryParse(Console.ReadLine(), out int id);
            postBaneado = miSistema.BuscarPost(id);
            try
            {
                if(postBaneado != null)
                {
                    miSistema.CensurarPost(postBaneado);
                    Console.WriteLine("El post ha sido baneado");
                } else
                {
                    throw new Exception("El post no existe");
                }
            } catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            Console.WriteLine("Presione una tecla para contiunar");
            Console.ReadKey();
        }

        //****************MIEMBRO***************

        static void MenuMiembro()
        {
            int opcion = -1;
            while (opcion != 0)
            {
                Console.Clear();
                Console.WriteLine("====MENU===");
                Console.WriteLine("1- Ver Muro");
                Console.WriteLine("0- Salir");
                int.TryParse(Console.ReadLine(), out opcion);
                SeleccionMenuMiembro(opcion);
            }
        }

        static void SeleccionMenuMiembro(int opcion)
        {
            switch (opcion)
            {
                case 1:
                    VerMuro();
                    break;
                case 2:

                    break;
                default:

                    break;
            }
        }

        static void VerMuro()
        {
            List<Post> muro = miSistema.MostrarMuro(miembroTest);
            try
            {
                if (muro.Count > 0)
                {
                    foreach (Post post in muro)
                    {
                        Console.WriteLine(post+"\n--------------------------------------\n");
                    }
                }
                else
                {
                    throw new Exception("No tienes publicaciones");
                }
            } 
            catch (Exception e) 
            {
                Console.WriteLine(e.Message);
            }
            Console.ReadKey();
        }


        //****************OBLIGATORIO PRIMER ENTREGA***************

        static void AltaMiembro()
        {
            try
            {
                Console.Clear();
                Console.WriteLine("===ALTA MIEMBRO===");
                Console.WriteLine("Ingrese el Email");
                string email = Console.ReadLine();
                Console.WriteLine("Ingrese el Password:");
                string password = Console.ReadLine();
                Console.WriteLine("Ingrese el Nombre:");
                string nombre = Console.ReadLine();
                Console.WriteLine("Ingrese el Apellido:");
                string apellido = Console.ReadLine();
                Console.WriteLine("Ingrese la Fecha de Nacimiento en formato yyyy-MM-dd:");
                DateTime.TryParse(Console.ReadLine(), out DateTime fechaNacimiento);
                if (email.Trim() != "" && password.Trim() != "" && nombre.Trim() != "" && apellido.Trim() != "" && fechaNacimiento > DateTime.MinValue && fechaNacimiento < DateTime.Now)
                {
                    Miembro miembro = new Miembro(email, password, nombre, apellido, fechaNacimiento);
                    miSistema.AltaUsuario(miembro);
                    Console.WriteLine("\nUsuario Registrado...");
                }
                
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            Console.WriteLine("\nIngrese una tecla para continuar...");
            Console.ReadKey();
        }




        //MOSTRAR PRECARGA DE DATOS SOLICITUDES AMIGOS Y MIEMBROS
        static void MostrarDatosPreCargados()
        {
            MostrarMiembros();
            //MostrarPostsYComentariosYReacciones();
        }

        static void MostrarMiembros()
        {
            List<Miembro> miembros = miSistema.DevolverUsuariosMiembros();
            for (int i = 0; i < miembros.Count; i++)
            {
                Console.WriteLine($"----> MIEMBRO {miembros[i]} <----");
                //Console.WriteLine(miembros[i]);
                MostrarSolicitudesYAmigos(miembros[i]);
            }
        }
        static void MostrarSolicitudesYAmigos(Miembro miembro)
        {
            if (miembro.Solicitudes.Count > 0)
            {
                Console.WriteLine($"\n\n---->SOLICITUDES MIEMBRO {miembro.Email}");
                for (int i = 0; i < miembro.Solicitudes.Count; i++)
                {
                    Console.WriteLine($"\n\n===SOLICITUD N{i} de {miembro.Email}");
                    Console.WriteLine(miembro.Solicitudes[i]);
                }
            }
            if (miembro.Amigos.Count > 0)
            {
                Console.WriteLine($"\n\n---->AMIGOS MIEMBRO {miembro.Email}");
                for (int i = 0; i < miembro.Amigos.Count; i++)
                {
                    Console.WriteLine($"\n\n===AMIGO N{i} de {miembro.Email}");
                    Console.WriteLine(miembro.Amigos[i]);
                }
            }
        }







        //============================Mostrar Precargas de Post Y Comentarios
        static void MostrarPostsYComentariosYReacciones()
        {
            List<Post> posts = miSistema.DevolverPosts();
            for (int i = 0; i < posts.Count; i++)
            {
                Console.WriteLine("\n\n===POST===");
                Console.WriteLine(posts[i]);
                if (posts[i].Reacciones.Count > 0)
                {
                    Console.WriteLine("\n----> REACCIONES POST");
                    MostrarReacciones(posts[i].Reacciones);
                }

                MostrarComentarios(posts[i].Comentarios);
            }
        }

        static void MostrarReacciones(List<Reaccion> reacciones)
        {
            for (int i = 0; i < reacciones.Count; i++)
            {
                Console.WriteLine(reacciones[i]);
            }
        }
        static void MostrarComentarios(List<Comentario> comentarios)
        {
            for (int i = 0; i < comentarios.Count; i++)
            {
                Console.WriteLine($"\n===COMENTARIO===");
                Console.WriteLine(comentarios[i]);
                if (comentarios[i].Reacciones.Count > 0)
                {
                    Console.WriteLine("\n----> REACCIONES COMENTARIO");
                    MostrarReacciones(comentarios[i].Reacciones);
                }
            }
        }

    }
}