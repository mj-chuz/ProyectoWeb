using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ProyectoWebBlog.Models.ViewModels
{
    public class ComentarioModel
    {
        [Required]
        [Display(Name = "Correo electronico")]
        public String Correo { get; set; }
        [Required]
        public String Texto { get; set; }
        public DateTime FechaPublicacion { get; set; }
        public String Titulo { get; set; }
        public DateTime FechaHoraPublicado { get; set; }
    }
}