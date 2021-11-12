
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ProyectoWebBlog.Models;
using ProyectoWebBlog.Models.ViewModels;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;

namespace ProyectoWebBlog.Controllers
{
    public class UsuarioController : Controller
    {
        public ActionResult ObtenerListaUsuarios()
        {
            List<UsuarioModel> usuario;
            using (WebBlogEntities baseDatos = new WebBlogEntities())
            {
                usuario = (from persona in baseDatos.Usuario
                           select new UsuarioModel
                           {
                               Id = persona.idPK,
                               Nombre = persona.nombre,
                               Correo = persona.correo,
                               PrimerApellido = persona.primerApellido,
                               SegundoApellido = persona.segundoApellido,
                               Rol = persona.rol
                           }).ToList();
            }

            return View(usuario);
        }

        public Dictionary<int, string> ObtenerNombreIdUsuarios()
        {
            List<UsuarioModel> usuarios;
            Dictionary<int, string> infoUsuarios = new Dictionary<int, string>();
            using (WebBlogEntities baseDatos = new WebBlogEntities())
            {

                usuarios = (from persona in baseDatos.Usuario
                            select new UsuarioModel
                            {
                                Id = persona.idPK,
                                Nombre = persona.nombre,
                                Correo = persona.correo,
                                PrimerApellido = persona.primerApellido,
                                SegundoApellido = persona.segundoApellido,
                                Rol = persona.rol
                            }).ToList();
                foreach (var usuario in usuarios)
                {
                    string nombreCompleto = usuario.Nombre + " " + usuario.PrimerApellido + " " + usuario.SegundoApellido;
                    infoUsuarios.Add(usuario.Id, nombreCompleto);
                }
            }

            return infoUsuarios;
        }

        public ActionResult CrearUsuario()
        {
            return View();
        }


        [HttpPost]
        public ActionResult CrearUsuario(UsuarioModel usuarioNuevo)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    using (WebBlogEntities baseDatos = new WebBlogEntities())
                    {
                        var usuario = new Usuario();
                        usuario.nombre = usuarioNuevo.Nombre;
                        usuario.correo = usuarioNuevo.Correo;
                        usuario.idPK = usuarioNuevo.Id;
                        usuario.primerApellido = usuarioNuevo.PrimerApellido;
                        usuario.segundoApellido = usuarioNuevo.SegundoApellido;
                        usuario.rol = usuarioNuevo.Rol;
                        usuario.contrasena = usuarioNuevo.Contrasena;
                        baseDatos.Usuario.Add(usuario);
                        baseDatos.SaveChanges();
                    }
                    var creacionDeUsuario = RegistrarUsuario(usuarioNuevo);
                    if (creacionDeUsuario != null)
                    {
                        ViewBag.CreacionDeUsuario = creacionDeUsuario;
                    }
                    return Redirect("~/Home");
                }
                ViewBag.CreacionDeUsuario = "Algo salió mal y no se pudo crear el usuario :(";
                return View(usuarioNuevo);

            }
            catch (Exception excepcion)
            {
                throw new Exception(excepcion.Message);
            }
            

        }

        public string RegistrarUsuario(UsuarioModel registroUsuario)
        {

            var usuarioFabrica = new UserStore<IdentityUser>();
            var manejadorUsuario = new UserManager<IdentityUser>(usuarioFabrica);
            var usuario = new IdentityUser() { UserName = registroUsuario.Id.ToString() };

            IdentityResult result = manejadorUsuario.Create(usuario, registroUsuario.Contrasena);

            if (result.Succeeded)
            {
                manejadorUsuario.AddToRole(usuario.Id, registroUsuario.Rol);
                var manejadorAutenticacion = HttpContext.Request.GetOwinContext().Authentication;
                var identidadUsuario = manejadorUsuario.CreateIdentity(usuario, DefaultAuthenticationTypes.ApplicationCookie);
                manejadorAutenticacion.SignIn(new AuthenticationProperties() { }, identidadUsuario);
                String resultado = "Usuario registrado con éxito";
                return resultado;
            }
            else
            {
                return null;
            }
        }

        public bool CreateRole(string name)
        {
            var rolFabrica = new RoleStore<IdentityRole>();
            var manejadorRol = new RoleManager<IdentityRole>(rolFabrica);
            var resultado = manejadorRol.Create(new IdentityRole(name));
            return resultado.Succeeded;
        }

        [Authorize(Roles = "Admin")]
        public ActionResult EditarUsuario(int Id)
        {
            UsuarioModel usuario = new UsuarioModel();
            using (WebBlogEntities baseDatos = new WebBlogEntities())
            {
                var usuarioTabla = baseDatos.Usuario.Find(Id);
                usuario.Id = usuarioTabla.idPK;
                usuario.Correo = usuarioTabla.correo;
                usuario.Nombre = usuarioTabla.nombre;
                usuario.PrimerApellido = usuarioTabla.primerApellido;
                usuario.SegundoApellido = usuarioTabla.segundoApellido;
                usuario.Rol = usuarioTabla.rol;

            }
            return View(usuario);
        }

        public UsuarioModel ObtenerUsuario(string contrasena, string correo)
        {
            UsuarioModel usuario = new UsuarioModel();
            using (WebBlogEntities baseDatos = new WebBlogEntities())
            {
                var usuarioTabla = baseDatos.Usuario.Where(x => x.contrasena == contrasena && x.correo == correo).SingleOrDefault();
                usuario.Id = usuarioTabla.idPK;
                usuario.Correo = usuarioTabla.correo;
                usuario.Nombre = usuarioTabla.nombre;
                usuario.PrimerApellido = usuarioTabla.primerApellido;
                usuario.SegundoApellido = usuarioTabla.segundoApellido;
                usuario.Rol = usuarioTabla.rol;

            }
            return usuario;
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult EditarUsuario(UsuarioModel usuarioNuevo)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    using (WebBlogEntities baseDatos = new WebBlogEntities())
                    {
                        var usuario = baseDatos.Usuario.Find(usuarioNuevo.Id);
                        usuario.nombre = usuarioNuevo.Nombre;
                        usuario.correo = usuarioNuevo.Correo;
                        usuario.idPK = usuarioNuevo.Id;
                        usuario.primerApellido = usuarioNuevo.PrimerApellido;
                        usuario.segundoApellido = usuarioNuevo.SegundoApellido;
                        usuario.rol = usuarioNuevo.Rol;

                        baseDatos.Entry(usuario).State = System.Data.Entity.EntityState.Modified;
                        baseDatos.SaveChanges();
                    }
                    return Redirect("~/Usuario/ObtenerListaUsuarios");
                }
                else
                {
                    ViewBag.MensajeError = "Algo salió mal, no se pudieron guardar las nuevas especificaciones :(";
                    return View(usuarioNuevo);
                }


            }
            catch (Exception excepcion)
            {
                throw new Exception(excepcion.Message);
            }
        }
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult EliminarUsuario(int Id)
        {
            using (WebBlogEntities baseDatos = new WebBlogEntities())
            {

                var usuarioTabla = baseDatos.Usuario.Find(Id);
                baseDatos.Usuario.Remove(usuarioTabla);
                baseDatos.SaveChanges();

            }
            return Redirect("~/Usuario/ObtenerListaUsuarios");
        }

        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(LoginModel usuario)
        {
            var usuarioFabrica = new UserStore<IdentityUser>();
            var manejadorUsuario = new UserManager<IdentityUser>(usuarioFabrica);
            var user = manejadorUsuario.Find(usuario.Cedula, usuario.Contrasena);

            if (user != null)
            {
                var manejadorAutenticacion = HttpContext.Request.GetOwinContext().Authentication;
                var identidadUsuario = manejadorUsuario.CreateIdentity(user, DefaultAuthenticationTypes.ApplicationCookie);

                manejadorAutenticacion.SignIn(new AuthenticationProperties() { IsPersistent = false }, identidadUsuario);
                ViewBag.resultadoLogin = true;
                return Redirect("~/Home");
            }
            else
            {
                ViewBag.resultadoLogin = false;
                return View();
            }
            
        }

        public void Logout()
        {
            var manejadorAutenticacion = HttpContext.Request.GetOwinContext().Authentication;
            manejadorAutenticacion.SignOut();
            Response.Redirect("~/Home");
        }

    }
}