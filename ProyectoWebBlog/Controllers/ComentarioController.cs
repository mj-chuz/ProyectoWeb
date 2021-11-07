using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ProyectoWebBlog.Models;
using ProyectoWebBlog.Models.ViewModels;
using System.IO;

namespace ProyectoWebBlog.Controllers
{
    public class ComentarioController : Controller
    {
        public PublicacionController AccesoAPublicacion;

        public ComentarioController()
        {
            AccesoAPublicacion = new PublicacionController();
        }
        public ActionResult Index()
        {

            List<ComentarioModel> comentarios;
            using (WebBlogEntities baseDatos = new WebBlogEntities())
            {
                comentarios = (from comentario in baseDatos.Comentario
                               select new ComentarioModel
                               {
                                   Correo = comentario.correoPK,
                                   Texto = comentario.texto,
                                   FechaPublicacion = comentario.fechaFK,
                                   Titulo = comentario.tituloFK,
                                   FechaHoraPublicado = comentario.fechaPublicadoPK
                               }).ToList();
            }

            return View(comentarios);
        }

        public ActionResult AgregarComentario(String titulo, DateTime fecha)
        {
            ViewBag.titulo = titulo;
            ViewBag.fecha = fecha;
            return View();
        }

        [HttpPost]
        public ActionResult AgregarComentario(ComentarioModel comentarioNuevo)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    using (WebBlogEntities baseDatos = new WebBlogEntities())
                    {

                        var comentario = new Comentario();
                        comentario.correoPK = comentarioNuevo.Correo;
                        comentario.fechaFK = comentarioNuevo.FechaHoraPublicado;
                        comentario.texto = comentarioNuevo.Texto;
                        comentario.tituloFK = comentarioNuevo.Titulo;
                        comentario.fechaPublicadoPK = DateTime.Now;
                        baseDatos.Comentario.Add(comentario);
                        baseDatos.SaveChanges();
                    }
                    AccesoAPublicacion.AumentarComentarios(comentarioNuevo.Titulo, comentarioNuevo.FechaPublicacion);
                    return Redirect("~/Publicacion");
                }
                return View(comentarioNuevo);
            }
            catch (Exception excepcion)
            {
                throw new Exception(excepcion.Message);
            }
        }

        public ActionResult VerComentarios(String titulo, DateTime fecha)
        {
            List<ComentarioModel> comentariosPublicacion = this.ObtenerComentariosSegunPublicacion(titulo, fecha);
            ViewBag.tituloPublicacion = titulo;
            return View(comentariosPublicacion);
        }

        public List<ComentarioModel> ObtenerComentariosSegunPublicacion(string Titulo, DateTime Fecha)
        {
            List<ComentarioModel> comentarios;
            using (WebBlogEntities baseDatos = new WebBlogEntities())
            {
                comentarios = (from comentario in baseDatos.Comentario.Where(x => x.tituloFK == Titulo && x.fechaFK == Fecha)
                               select new ComentarioModel
                               {
                                   FechaHoraPublicado = comentario.fechaPublicadoPK,
                                   Correo = comentario.correoPK,
                                   Texto = comentario.texto,
                                   FechaPublicacion = comentario.fechaFK,
                                   Titulo = comentario.tituloFK

                               }).ToList();
            }
            return comentarios;
        }
    }


}