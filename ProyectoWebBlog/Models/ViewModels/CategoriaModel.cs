using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;


namespace ProyectoWebBlog.Models.ViewModels
{
    public class CategoriaModel
    {
        [Required]
        [Display(Name = "Nombre categoria")]
        public String Nombre { get; set; }
    }
}