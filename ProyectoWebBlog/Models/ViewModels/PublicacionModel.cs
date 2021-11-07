using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;


namespace ProyectoWebBlog.Models.ViewModels
{
    public class PublicacionModel
    {
        public DateTime Fecha { get; set; }

        [Required]
        [Display(Name = "Titulo de publicacion")]
        public String Titulo { get; set; }

        [Required]
        [Display(Name = "Cuerpo de la publicacion")]
        public String Texto { get; set; }

        public int NumeroComentarios { get; set; }

        public String TipoArchivo { get; set; }

        [Required]
        [Display(Name = "Ingrese una foto")]
        public HttpPostedFileBase ArchivoFoto { get; set; }

        [Required]
        [Display(Name = "Numero de identificacion")]
        public int IdUsuario { get; set; }

        [Required]
        [Display(Name = "Categoria de la publicacion")]
        public String Categoria { get; set; }
    }
}