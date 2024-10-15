using LogicaNegocio;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System.IO;

namespace Web.Controllers
{
    public class MiembroController : Controller
    {
        private Sistema _sistema = Sistema.Instancia;

        
        public IActionResult MostrarMuro()
        {
            if(HttpContext.Session.GetString("Rol") == null)
            {
                return RedirectToAction("Login", "Usuario");
            }
            else if (HttpContext.Session.GetString("Rol") == "MIEMBRO")
            {
                try
                {
                    Miembro miembroLogueado = _sistema.BuscarMiembro(HttpContext.Session.GetString("Email"));
                    if(miembroLogueado != null)
                    {
                        ViewBag.Miembro = miembroLogueado.Email;
                        ViewBag.Bloqueado = miembroLogueado.Bloqueado;
                        ViewBag.Solicitudes = _sistema.DevolverSolicitudesPendientesMiembro(miembroLogueado);
                        List<Post> posts = _sistema.MostrarMuro(miembroLogueado);
                        return View(posts);
                    } else
                    {
                        throw new Exception("Miembro no valido");
                    }
                }
                catch(Exception ex)
                {
                    TempData["Mensaje-Registrarse"] = ex.Message;
                    return RedirectToAction("Login", "Usuario");
                }
            }
            else if(HttpContext.Session.GetString("Rol") == "ADMINISTRADOR")
            {
                return RedirectToAction("BloquearMiembro", "Administrador");
            }
            return RedirectToAction("Login", "Usuario");
        }

        public IActionResult ReaccionPublicacionMG(int id)
        {
            if(HttpContext.Session.GetString("Rol") == null)
            {
                return RedirectToAction("Login", "Usuario");
            }
            else if (HttpContext.Session.GetString("Rol") == "MIEMBRO")
            {
                try
                {
                    Miembro miembroLogueado = _sistema.BuscarMiembro(HttpContext.Session.GetString("Email"));
                    if(miembroLogueado != null)
                    {
                        Publicacion publicacion = _sistema.BuscarPublicacion(id);
                        if (publicacion != null)
                        {
                            Reaccion reaccion = new Reaccion(miembroLogueado, TipoReaccion.ME_GUSTA);
                            publicacion.AltaReaccion(reaccion);
                        }
                        else
                        {
                            throw new Exception("La publicacion no existe");
                        }
                    } else
                    {
                        TempData["Mensaje-Registrarse"] = "Usuario logueado no valido";
                        return RedirectToAction("Login", "Usuario");
                    }
                }
                catch (Exception e) { TempData["Mensaje-Reaccion"] = e.Message; };
                return RedirectToAction("MostrarMuro");
            }
            else if(HttpContext.Session.GetString("Rol") == "ADMINISTRADOR")
            {
                return RedirectToAction("BloquearMiembro", "Administrador");
            }
            return RedirectToAction("Login", "Usuario");  
        }

        public IActionResult ReaccionPublicacionNMG(int id)
        {
            if (HttpContext.Session.GetString("Rol") == null)
            {
                return RedirectToAction("Login", "Usuario");
            }
            else if (HttpContext.Session.GetString("Rol") == "MIEMBRO")
            {
                Miembro miembroLogueado = _sistema.BuscarMiembro(HttpContext.Session.GetString("Email"));
                if(miembroLogueado != null)
                {
                    try
                    {
                        Publicacion publicacion = _sistema.BuscarPublicacion(id);
                        if (publicacion != null)
                        {
                            Reaccion reaccion = new Reaccion(miembroLogueado, TipoReaccion.NO_ME_GUSTA);
                            publicacion.AltaReaccion(reaccion);
                        }
                        else
                        {
                            throw new Exception("La publicacion no existe");
                        }
                    }
                    catch (Exception e) { TempData["Mensaje-Reaccion"] = e.Message; };
                    return RedirectToAction("MostrarMuro");
                }
                else
                {
                    TempData["Mensaje-Registrarse"] = "Usuario logueado no valido";
                    return RedirectToAction("Login", "Usuario");
                }
            }
            else if (HttpContext.Session.GetString("Rol") == "ADMINISTRADOR")
            {
                return RedirectToAction("BloquearMiembro", "Administrador");
            }
            return RedirectToAction("Login", "Usuario");
        }

        [HttpPost]
        public IActionResult Comentar(int idPost, string tituloComentario, string contenido)
        {
            Miembro miembroTest = _sistema.BuscarMiembro(HttpContext.Session.GetString("Email"));
            try
            {
                Post post = _sistema.BuscarPost(idPost);
                if(post != null && tituloComentario != null && contenido != null)
                {
                    Comentario comentario = new Comentario(miembroTest, tituloComentario, contenido, post.Privacidad);
                    _sistema.AltaComentario(post, comentario);
                } 
                else
                {
                    throw new Exception("Ingrese campos no vacios");
                }
            }
            catch (Exception e)
            {
                TempData["Mensaje-Comentario"] = e.Message;
            }
            TempData["Mensaje-Comentario-IdPost"] = idPost;
            return RedirectToAction("MostrarMuro");
        }

        //Task<IActionResult> -await file.CopyToAsync(stream);
        [HttpPost]
        public IActionResult Postear(string tituloPost, string contenido, IFormFile file)
        {
            Miembro miembroLogueado = _sistema.BuscarMiembro(HttpContext.Session.GetString("Email"));
            string wwwrootDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/post-img");
            //string ruta = WebHostEnvironment.WebRootPath;
            try
            {
                if (tituloPost != null && contenido != null && file != null) 
                {
                    //Solucion paleativa para evitar duplicados
                    Random r = new Random();
                    int rint = r.Next(10000, 50000);
                    Post post = new Post(miembroLogueado, tituloPost, contenido, rint + file.FileName, Privacidad.PRIVADO);
                    var path = Path.Combine(wwwrootDirectory, rint + file.FileName);

                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        //CopyToAsync
                        file.CopyTo(stream);
                    }
                    _sistema.AltaPost(post);
                    TempData["Mensaje-Post"] = "Publicado";
                }
                else
                {
                    throw new Exception("Ingrese campos validos");
                }
                
            }
            catch (Exception e)
            {
                TempData["Mensaje-Post"] = e.Message;
            }
            return RedirectToAction("MostrarMuro");
        }

        public IActionResult AceptarSolicitud(int id)
        {
            if (HttpContext.Session.GetString("Rol") == null)
            {
                return RedirectToAction("Login", "Usuario");
            }
            else if (HttpContext.Session.GetString("Rol") == "MIEMBRO")
            {
                Miembro miembroLogueado = _sistema.BuscarMiembro(HttpContext.Session.GetString("Email"));
                if (miembroLogueado != null)
                {
                    try
                    {
                        _sistema.AceptarSolicitudMiembro(HttpContext.Session.GetString("Email"), id);
                        TempData["Mensaje-Solicitud"] = "Solicitud Aceptada";
                    }
                    catch (Exception e)
                    {
                        TempData["Mensaje-Solicitud"] = e.Message;
                    }
                    return RedirectToAction("MostrarMuro");
                }
                else
                {
                    TempData["Mensaje-Registrarse"] = "Usuario logueado no valido";
                    return RedirectToAction("Login", "Usuario");
                }

            }
            else if (HttpContext.Session.GetString("Rol") == "ADMINISTRADOR")
            {
                return RedirectToAction("BloquearMiembro", "Administrador");
            }
            return RedirectToAction("Login", "Usuario");
        }

        public IActionResult RechazarSolicitud(int id)
        {
            if (HttpContext.Session.GetString("Rol") == null)
            {
                return RedirectToAction("Login", "Usuario");
            }
            else if (HttpContext.Session.GetString("Rol") == "MIEMBRO")
            {
                Miembro miembroLogueado = _sistema.BuscarMiembro(HttpContext.Session.GetString("Email"));
                if (miembroLogueado != null)
                {
                    try
                    {
                        _sistema.RechazarSolicitudMiembro(HttpContext.Session.GetString("Email"), id);
                        TempData["Mensaje-Solicitud"] = "Solicitud Rechazada";
                    }
                    catch (Exception e)
                    {
                        TempData["Mensaje-Solicitud"] = e.Message;
                    }
                    return RedirectToAction("MostrarMuro");
                }
                else
                {
                    TempData["Mensaje-Registrarse"] = "Usuario logueado no valido";
                    return RedirectToAction("Login", "Usuario");
                }
            }
            else if (HttpContext.Session.GetString("Rol") == "ADMINISTRADOR")
            {
                return RedirectToAction("BloquearMiembro", "Administrador");
            }
            return RedirectToAction("Login", "Usuario");
        }

        public IActionResult AgregarAmigo()
        {
            if (HttpContext.Session.GetString("Rol") == null)
            {
                return RedirectToAction("Login", "Usuario");
            }
            else if (HttpContext.Session.GetString("Rol") == "MIEMBRO")
            {
                Miembro miembroLogueado = _sistema.BuscarMiembro(HttpContext.Session.GetString("Email"));
                if (miembroLogueado != null)
                {
                    try
                    {
                        if (miembroLogueado != null)
                        {
                            ViewBag.MiembrosNoAmigos = _sistema.DevolverMiembrosNoAmigos(miembroLogueado);
                            ViewBag.Solicitudes = _sistema.DevolverSolicitudesPendientesMiembro(miembroLogueado);
                        }
                    }
                    catch (Exception e)
                    {
                        ViewBag.Mensaje = e.Message;
                    }
                    return View();
                }
                else
                {
                    TempData["Mensaje-Registrarse"] = "Usuario logueado no valido";
                    return RedirectToAction("Login", "Usuario");
                }
            }
            else if (HttpContext.Session.GetString("Rol") == "ADMINISTRADOR")
            {
                return RedirectToAction("BloquearMiembro", "Administrador");
            }
            return RedirectToAction("Login", "Usuario");
        }

        public IActionResult EnviarSolicitud(string id)
        {
            if (HttpContext.Session.GetString("Rol") == null)
            {
                return RedirectToAction("Login", "Usuario");
            }
            else if (HttpContext.Session.GetString("Rol") == "MIEMBRO")
            {
                Miembro miembroLogueado = _sistema.BuscarMiembro(HttpContext.Session.GetString("Email"));
                if (miembroLogueado != null)
                {
                    try
                    {
                        if (miembroLogueado != null)
                        {
                            _sistema.AltaSolicitud(miembroLogueado, id);
                            TempData["Mensaje-EnviarSolicitud"] = "Solicitud Enviada";
                        }
                    }
                    catch (Exception e)
                    {
                        TempData["Mensaje-EnviarSolicitud"] = e.Message;
                    }
                    ViewBag.Solicitudes = _sistema.DevolverSolicitudesPendientesMiembro(miembroLogueado);
                    return RedirectToAction("AgregarAmigo");
                } else
                {
                    TempData["Mensaje-Registrarse"] = "Usuario logueado no valido";
                    return RedirectToAction("Login", "Usuario");
                }
            }
            else if (HttpContext.Session.GetString("Rol") == "ADMINISTRADOR")
            {
                return RedirectToAction("BloquearMiembro", "Administrador");
            }
            return RedirectToAction("Login", "Usuario");
        }

        public IActionResult FiltrarPublicaciones()
        {
            if (HttpContext.Session.GetString("Rol") == null)
            {
                return RedirectToAction("Login", "Usuario");
            }
            else if (HttpContext.Session.GetString("Rol") == "MIEMBRO")
            {
                Miembro miembroLogueado = _sistema.BuscarMiembro(HttpContext.Session.GetString("Email"));
                if (miembroLogueado != null)
                {
                    ViewBag.Solicitudes = _sistema.DevolverSolicitudesPendientesMiembro(miembroLogueado);
                    return View();
                } else
                {
                    TempData["Mensaje-Registrarse"] = "Usuario logueado no valido";
                    return RedirectToAction("Login", "Usuario");
                }
            }
            else if (HttpContext.Session.GetString("Rol") == "ADMINISTRADOR")
            {
                return RedirectToAction("BloquearMiembro", "Administrador");
            }
            return RedirectToAction("Login", "Usuario");
        }
        [HttpPost]
        public IActionResult FiltrarPublicaciones(int valorAceptacion, string textoIncluido)
        {
            if (HttpContext.Session.GetString("Rol") == null)
            {
                return RedirectToAction("Login", "Usuario");
            }
            else if (HttpContext.Session.GetString("Rol") == "MIEMBRO")
            {
                List<Publicacion> publicaciones = null;
                Miembro miembroLogueado = _sistema.BuscarMiembro(HttpContext.Session.GetString("Email"));
                try
                {
                    if (valorAceptacion != null && valorAceptacion > 0 && textoIncluido != null && textoIncluido != "")
                    {
                        publicaciones = _sistema.FiltrarPublicaciones(textoIncluido, valorAceptacion, miembroLogueado);
                    }
                    else
                    {
                        throw new Exception("Ingrese datos validos: El numero tiene que ser superior a 0 y el texto no puede estar vacio");
                    }
                } catch (Exception ex)
                {
                    ViewBag.Mensaje = ex.Message;
                }
                ViewBag.Solicitudes = _sistema.DevolverSolicitudesPendientesMiembro(miembroLogueado);
                return View(publicaciones);

            }
            else if (HttpContext.Session.GetString("Rol") == "ADMINISTRADOR")
            {
                return RedirectToAction("BloquearMiembro", "Administrador");
            }
            return RedirectToAction("Login", "Usuario");
        }
    }
}
