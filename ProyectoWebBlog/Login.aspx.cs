using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using ProyectoWebBlog.Models.ViewModels;
using System;
using System.Web;
using System.Web.UI.WebControls;

namespace ProyectoWebBlog
{
    public partial class Login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (User.Identity.IsAuthenticated)
                {
                    StatusText.Text = string.Format("Bienvenido!! ", User.Identity.GetUserName());
                    LoginStatus.Visible = true;
                    LogoutButton.Visible = true;
                }
                else
                {
                    LoginForm.Visible = true;
                }
            }
        }

        public bool SignIn(LoginModel usuario)
        {
            var usuarioFabrica = new UserStore<IdentityUser>();
            var manejadorUsuario = new UserManager<IdentityUser>(usuarioFabrica);
            var user = manejadorUsuario.Find(usuario.Cedula, usuario.Contrasena);

            if (user != null)
            {
                var manejadorAutenticacion = HttpContext.Current.GetOwinContext().Authentication;
                var identidadUsuario = manejadorUsuario.CreateIdentity(user, DefaultAuthenticationTypes.ApplicationCookie);

                manejadorAutenticacion.SignIn(new AuthenticationProperties() { IsPersistent = false }, identidadUsuario);
                return true;
            }
            else
            {
                return false;
            }
        }

        public void SignOut()
        {
            var manejadorAutenticacion = HttpContext.Current.GetOwinContext().Authentication;
            manejadorAutenticacion.SignOut();
            Response.Redirect("~/Home");
        }
    }
}