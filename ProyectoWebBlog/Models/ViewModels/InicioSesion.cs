using System.ComponentModel.DataAnnotations;


namespace ProyectoWebBlog.Models.ViewModels
{
    public class LoginModel
    {
        [Required]
        public string Cedula { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Contrasena { get; set; }
    }
}