//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ProyectoWebBlog.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Publicacion
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Publicacion()
        {
            this.Comentario = new HashSet<Comentario>();
        }
    
        public System.DateTime fechaPK { get; set; }
        public string tituloPK { get; set; }
        public string texto { get; set; }
        public Nullable<int> numeroComentarios { get; set; }
        public int idAutorFK { get; set; }
        public byte[] imagenPublicacion { get; set; }
        public string tipoArchivo { get; set; }
        public string nombreCategoriaFK { get; set; }
    
        public virtual Categoria Categoria { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Comentario> Comentario { get; set; }
        public virtual Usuario Usuario { get; set; }
    }
}
