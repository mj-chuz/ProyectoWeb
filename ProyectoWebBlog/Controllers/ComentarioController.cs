﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ProyectoWebBlog.Models;
using ProyectoWebBlog.Models.ViewModels;
using System.IO;
using System.Globalization;
using System.Text.RegularExpressions;

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

        public ActionResult AgregarComentario(String titulo, String fecha)
        {
            ViewBag.titulo = titulo;
            ViewBag.fecha = DateTime.ParseExact(fecha, "yyyyMMddHHmmss", CultureInfo.InvariantCulture);
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
                        comentario.fechaFK = comentarioNuevo.FechaPublicacion;
                        comentario.texto = comentarioNuevo.Texto;
                        comentario.tituloFK = comentarioNuevo.Titulo;
                        comentario.fechaPublicadoPK = DateTime.Now;
                        baseDatos.Comentario.Add(comentario);
                        baseDatos.SaveChanges();
                    }
                    AccesoAPublicacion.AumentarComentarios(comentarioNuevo.Titulo, comentarioNuevo.FechaPublicacion);
                    return Redirect("~/Home");
                }
                return View(comentarioNuevo);
            }
            catch (Exception excepcion)
            {
                throw new Exception(excepcion.Message);
            }
        }

        public JsonResult ObtenerComentariosSegunPublicacion(string identificadorPublicacion)
        {
            string[] partesIdentificador = identificadorPublicacion.Split('|');
            string titulo = partesIdentificador[0];
            string fecha = partesIdentificador[1];
            titulo = Regex.Replace(titulo, "-", " ");
            DateTime fechaParseada = DateTime.ParseExact(fecha, "yyyyMMddHHmmss", CultureInfo.InvariantCulture);
            List<ComentarioModel> comentarios;
            using (WebBlogEntities baseDatos = new WebBlogEntities())
            {
                comentarios = (from comentario in baseDatos.Comentario.Where(x => x.tituloFK == titulo && x.fechaFK == fechaParseada)
                               select new ComentarioModel
                               {
                                   FechaHoraPublicado = comentario.fechaPublicadoPK,
                                   Correo = comentario.correoPK,
                                   Texto = comentario.texto,
                                   FechaPublicacion = comentario.fechaFK,
                                   Titulo = comentario.tituloFK

                               }).ToList();
            }
            return Json(comentarios);
        }
       
    }


}