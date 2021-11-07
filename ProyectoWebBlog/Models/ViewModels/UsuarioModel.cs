using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ProyectoWebBlog.Models.ViewModels
{
    public class UsuarioModel
    {
        [Required]
        [Display(Name = "Numero de identificacion")]
        public int Id { get; set; }
        [Required]
        [EmailAddress]
        [Display(Name = "Correo")]
        public String Correo { get; set; }
        [Required]
        [Display(Name = "Nombre")]
        public String Nombre { get; set; }
        [Required]
        [Display(Name = "Primer Apellido")]
        public String PrimerApellido { get; set; }
        [Required]
        [Display(Name = "Segundo Apellido")]
        public String SegundoApellido { get; set; }
        public String Rol { get; set; }
    }
}