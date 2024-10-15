using LogicaNegocio;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers
{
    public class AdministradorController : Controller
    {
        private Sistema _sistema = Sistema.Instancia;

        public IActionResult BloquearMiembro()
        {
            if (HttpContext.Session.GetString("Rol") == null)
            {
                return RedirectToAction("Login", "Usuario");
            }
            else if (HttpContext.Session.GetString("Rol") == "MIEMBRO")
            {
                return RedirectToAction("MostrarMuro");
            }
            else if (HttpContext.Session.GetString("Rol") == "ADMINISTRADOR")
            {
                List<Miembro> miembros = _sistema.DevolverUsuariosMiembroOrdenados();
                return View(miembros);
            }
            return RedirectToAction("Login", "Usuario"); 
        }

        [HttpPost]
        public IActionResult BloquearMiembro(string email)
        {
            List<Miembro> miembros = _sistema.DevolverUsuariosMiembroOrdenados();
            try
            {
                Administrador administradorLogueado = _sistema.BuscarAdministrador(HttpContext.Session.GetString("Email"));
                if(administradorLogueado != null)
                {
                    if (email != null && email != "")
                    {
                        Miembro miembro = _sistema.BuscarMiembro(email);
                        _sistema.BloquearMiembro(miembro);
                        ViewBag.Message = "Bloqueo Exitoso";
                    }
                    else
                    {
                        throw new Exception("El miembro no existe, ingrese un mail valido");
                    }
                }else
                {
                    return RedirectToAction("Login", "Usuario");
                }
                
            } 
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
            }
            return View(miembros);
        }

        public IActionResult BannearPost()
        {
            if (HttpContext.Session.GetString("Rol") == null)
            {
                return RedirectToAction("Login", "Usuario");
            }
            else if (HttpContext.Session.GetString("Rol") == "MIEMBRO")
            {
                return RedirectToAction("MostrarMuro");
            }
            else if (HttpContext.Session.GetString("Rol") == "ADMINISTRADOR")
            {

                Administrador administradorLogueado = _sistema.BuscarAdministrador(HttpContext.Session.GetString("Email"));
                if (administradorLogueado != null)
                {
                    List<Post> posts = _sistema.DevolverPosts();
                    return View(posts);
                }else
                {
                    return RedirectToAction("Login", "Usuario");
                }
            }
            return RedirectToAction("Login", "Usuario");
        }

        [HttpPost]
        public IActionResult BannearPost(int id)
        {
            List<Post> posts = _sistema.DevolverPosts();
            try
            {
                if(id != 0)
                {
                    Post post = _sistema.BuscarPost(id);
                    _sistema.CensurarPost(post);
                    ViewBag.Message = "Post censurado correctamente";
                } else
                {
                    throw new Exception("No existe post con ese id");
                }
            } catch (Exception e) { ViewBag.Message = e.Message;}

            return View(posts);
        }
    }
}
