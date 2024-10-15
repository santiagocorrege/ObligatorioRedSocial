using Microsoft.AspNetCore.Mvc;
using LogicaNegocio;
namespace Web.Controllers
{
    public class UsuarioController : Controller
    {
        private Sistema _sistema = Sistema.Instancia;
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string nombreUsuario, string contrasena)
        {
            try
            {
                if(nombreUsuario != null && contrasena != null)
                {
                    Usuario usuarioComprobado = _sistema.ComprobarUsuario(nombreUsuario, contrasena);
                    
                    
                    if (usuarioComprobado != null)
                    {
                        usuarioComprobado.Validate();
                        string rol = usuarioComprobado.Rol();
                        HttpContext.Session.SetString("Rol", rol);
                        HttpContext.Session.SetString("Email", usuarioComprobado.Email);
                        if (rol == "MIEMBRO")
                        {
                            return RedirectToAction("MostrarMuro", "Miembro");
                        }
                        else if (rol == "ADMINISTRADOR")
                        {
                            return RedirectToAction("BloquearMiembro", "Administrador");
                        }
                    }
                    else
                    {
                        ViewBag.Mensaje = "El nombre de usuario o contrasena son incorrectos";
                    }
                }else
                {
                    throw new Exception("Ingrese campos no vacios");
                }
            }
            catch (Exception ex)
            {
                ViewBag.Mensaje = ex.Message;
            }
            return View();
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        public IActionResult Registrarse()
        {
            if (HttpContext.Session.GetString("Rol") == null)
            {
                return View(new Miembro());
            }
            else if (HttpContext.Session.GetString("Rol") == "MIEMBRO")
            {
                return RedirectToAction("MostrarMuro", "Miembro");
            }
            else if (HttpContext.Session.GetString("Rol") == "ADMINISTRADOR")
            {
                return RedirectToAction("BloquearMiembro", "Administrador");
            }
            return RedirectToAction("Login", "Usuario");

        }

        [HttpPost]
        public IActionResult Registrarse(Miembro miembro)
        {
            try
            {
                miembro.Validate();
                _sistema.AltaUsuario(miembro);
                TempData["Mensaje-Registrarse"] = "Usuario Creado";
                HttpContext.Session.SetString("Rol", miembro.Rol());
                HttpContext.Session.SetString("Email", miembro.Email);
                return RedirectToAction("MostrarMuro", "Miembro");
            }
            catch (Exception ex)
            {
                ViewBag.Mensaje = ex.Message;
            }
            return View(miembro);
        }
    }
}
