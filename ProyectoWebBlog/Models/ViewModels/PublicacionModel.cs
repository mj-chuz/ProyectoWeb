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
        [Display(Name = "Título")]
        public String Titulo { get; set; }

        [Required]
        [Display(Name = "Cuerpo")]
        public String Texto { get; set; }

        public int NumeroComentarios { get; set; }

        public String TipoArchivo { get; set; }

        [Required]
        [Display(Name = "Foto")]
        public HttpPostedFileBase ArchivoFoto { get; set; }

        [Required]
        [Display(Name = "Número de identificación")]
        public int IdUsuario { get; set; }

        [Required]
        [Display(Name = "Categoría de la publicación")]
        public String Categoria { get; set; }
    }
}