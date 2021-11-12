using ProyectoWebBlog.Models;
using ProyectoWebBlog.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace ProyectoWebBlog.Controllers
{
    public class AccountController : Controller
    {
        UsuarioController AccesoUsuarios = new UsuarioController();

        public ActionResult RegistrarUsuario()
        {
            return View();
        }

        [HttpPost]
        public ActionResult GuardarUsuario(UsuarioModel registerDetails)
        {
         
            if (ModelState.IsValid)
            {
                AccesoUsuarios.CrearUsuario(registerDetails);

                ViewBag.Message = "Usuario creado";
                return View("RegistrarUsuario");
            }
            else
            {
                return View("RegistrarUsuario", registerDetails);
            }
        }

        public ActionResult IniciarSesion()
        {
            return View();
        }

        [HttpPost]
        public ActionResult IniciarSesion(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                var isValidUser = ValidarUsuario(model);
                if (isValidUser != null)
                {
                    FormsAuthentication.SetAuthCookie(model.Cedula, false);
                    return RedirectToAction("~/Home");
                }
                else
                {
                    ModelState.AddModelError("Error", "Correo o contraseña incorrecto :(");
                    return View();
                }
            }
            else
            {
                return View(model);
            }
        }

        public UsuarioModel ValidarUsuario(LoginModel model)
        {
            var usuario = AccesoUsuarios.ObtenerUsuario(model.Contrasena, model.Cedula);
            if (usuario == null)
            {
                return null;
            }
            else
            {
                return usuario;
            }

        }


        public ActionResult CerrarSesion()
        {
            FormsAuthentication.SignOut();
            Session.Abandon(); 
            return RedirectToAction("~/Home");
        }
    }
}