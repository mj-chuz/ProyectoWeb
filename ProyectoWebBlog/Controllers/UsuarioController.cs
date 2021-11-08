
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ProyectoWebBlog.Models;
using ProyectoWebBlog.Models.ViewModels;

namespace ProyectoWebBlog.Controllers
{
    public class UsuarioController : Controller
    {
        // GET: Usuario
        public ActionResult Index()
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
                        usuario.contrasena = "culillo";
                        baseDatos.Usuario.Add(usuario);
                        baseDatos.SaveChanges();
                    }

                    return Redirect("~/Home");
                }

                return View(usuarioNuevo);

            }
            catch (Exception excepcion)
            {
                throw new Exception(excepcion.Message);
            }
        }

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
                    return Redirect("~/Usuario");
                }

                return View(usuarioNuevo);

            }
            catch (Exception excepcion)
            {
                throw new Exception(excepcion.Message);
            }
        }

        [HttpGet]
        public ActionResult EliminarUsuario(int Id)
        {
            using (WebBlogEntities baseDatos = new WebBlogEntities())
            {

                var usuarioTabla = baseDatos.Usuario.Find(Id);
                baseDatos.Usuario.Remove(usuarioTabla);
                baseDatos.SaveChanges();

            }
            return Redirect("~/Usuario");
        }

    }
}