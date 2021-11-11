using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using ProyectoWebBlog.Models.ViewModels;
using System;
using System.Linq;
using System.Web;

namespace ProyectoWebBlog
{


    public partial class Register : System.Web.UI.Page
    {
        UsuarioModel usuarioNuevo;
        public Register(UsuarioModel usuario)
        {
            this.usuarioNuevo = usuario;
        }

        public string CreateUser_Click()
        {

            var usuarioFabrica = new UserStore<IdentityUser>();
            var manejadorUsuario = new UserManager<IdentityUser>(usuarioFabrica);
            var user = new IdentityUser() { UserName = usuarioNuevo.Id.ToString() };
            
            IdentityResult result = manejadorUsuario.Create(user, usuarioNuevo.Contrasena);
            
            if (result.Succeeded)
            {
                manejadorUsuario.AddToRole(user.Id, "Autor");
                var manejadorAutenticacion = HttpContext.Current.GetOwinContext().Authentication;
                var identidadUsuario = manejadorUsuario.CreateIdentity(user, DefaultAuthenticationTypes.ApplicationCookie);
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

    }
}