﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ProyectoWebBlog.Models;
using ProyectoWebBlog.Models.ViewModels;
using System.IO;
using System.Data.Entity;
using System.Diagnostics;

namespace ProyectoWebBlog.Controllers
{
    public class PublicacionController : Controller
    {
        public UsuarioController AccesoAUsuarios;
        public int CantidadPaginas { get; set; }
        public PublicacionController()
        {
            AccesoAUsuarios = new UsuarioController();
        }
        public ActionResult Index()
        {
            List<PublicacionModel> publicacion;
            using (WebBlogEntities baseDatos = new WebBlogEntities())
            {
                publicacion = (from publicacionActual in baseDatos.Publicacion
                               select new PublicacionModel
                               {
                                   Fecha = publicacionActual.fechaPK,
                                   Titulo = publicacionActual.tituloPK,
                                   Texto = publicacionActual.texto,
                                   IdUsuario = publicacionActual.idAutorFK,
                                   NumeroComentarios = (int)publicacionActual.numeroComentarios,
                                   Categoria = publicacionActual.nombreCategoriaFK

                               }).ToList();
            }
            return View(publicacion);
        }

        public void AumentarComentarios(string Titulo, DateTime Fecha)
        {
            using (WebBlogEntities baseDatos = new WebBlogEntities())
            {
                var publicacionTabla = baseDatos.Publicacion.Where(x => x.tituloPK == Titulo && x.fechaPK == Fecha).SingleOrDefault();
                publicacionTabla.numeroComentarios += 1;
                baseDatos.Entry(publicacionTabla).State = System.Data.Entity.EntityState.Modified;
                baseDatos.SaveChanges();

            }
        }

        public List<PublicacionModel> ObtenerPublicacionesSegunAutor(int id)
        {
            List<PublicacionModel> publicaciones;
            using (WebBlogEntities baseDatos = new WebBlogEntities())
            {
                publicaciones = (from publicacionActual in baseDatos.Publicacion.Where(x => x.idAutorFK == id)
                                 select new PublicacionModel
                                 {
                                     Fecha = publicacionActual.fechaPK,
                                     Titulo = publicacionActual.tituloPK,
                                     Texto = publicacionActual.texto,
                                     IdUsuario = publicacionActual.idAutorFK,
                                     NumeroComentarios = (int)publicacionActual.numeroComentarios

                                 }).ToList();
            }
            return publicaciones;
        }

        public ActionResult VerPublicacionesAutor(int id, string nombreCompleto, int paginaMostrada = 1)
        {
            paginaMostrada = this.ValidarNumeroDePagina(paginaMostrada);
            Tuple<int, int> limitesPaginacion = this.CalcularLimitesPaginacion(paginaMostrada);
            List<PublicacionModel> publicacion = this.ObtenerPaginaPublicaciones(id, paginaMostrada);
            ViewBag.NumeroInicioPaginacion = limitesPaginacion.Item1;
            ViewBag.NumeroFinalPaginacion = limitesPaginacion.Item2;
            ViewBag.PaginaActual = paginaMostrada;
            ViewBag.CantidadTotalDePaginas = this.CantidadPaginas;
            ViewBag.nombreAutor = nombreCompleto;
            ViewBag.idAutor = id;
            ViewBag.ListaUsuarios = AccesoAUsuarios.ObtenerNombreIdUsuarios();
            return View(publicacion);
        }

        public int CalcularTotalDePaginas(int publicacionesPorPagina)
        {
            List<PublicacionModel> publicaciones = this.ObtenerPublicaciones();
            int cantidadTotalPublicaciones = publicaciones.Count;
            int cantidadTotalDePaginas = cantidadTotalPublicaciones / publicacionesPorPagina;
            if (cantidadTotalPublicaciones % publicacionesPorPagina != 0)
            {
                cantidadTotalDePaginas += 1;
            }
            return cantidadTotalDePaginas;
        }

        public List<PublicacionModel> ObtenerPaginaPublicaciones(int id, int pagina = 1)
        {
            List<PublicacionModel> publicaciones = this.ObtenerPublicacionesSegunAutor(id);
            int indiceInicio = (pagina - 1) * 4;
            int indiceFinal = indiceInicio + 4;
            Tuple<int, int> indicesBusqueda = new Tuple<int, int>(indiceInicio, indiceFinal);
            return this.ObtenerPublicacionesPaginacion(indicesBusqueda, publicaciones);
        }

        List<PublicacionModel> ObtenerPublicacionesPaginacion(Tuple<int, int> indicesPaginacion, List<PublicacionModel> publicaciones)
        {
            List<PublicacionModel> publicacionesPaginacion = new List<PublicacionModel>();
            for (int indicePublicacion = indicesPaginacion.Item1; indicePublicacion < indicesPaginacion.Item2 && indicePublicacion < publicaciones.Count; ++indicePublicacion)
            {
                publicacionesPaginacion.Add(publicaciones[indicePublicacion]);
            }
            return publicacionesPaginacion;
        }

        public List<PublicacionModel> ObtenerPublicaciones()
        {
            List<PublicacionModel> publicacion;
            using (WebBlogEntities baseDatos = new WebBlogEntities())
            {
                publicacion = (from publicacionActual in baseDatos.Publicacion
                               select new PublicacionModel
                               {
                                   Fecha = publicacionActual.fechaPK,
                                   Titulo = publicacionActual.tituloPK,
                                   Texto = publicacionActual.texto,
                                   IdUsuario = publicacionActual.idAutorFK,
                                   NumeroComentarios = (int)publicacionActual.numeroComentarios

                               }).ToList();
            }
            return publicacion;
        }

        public int ValidarNumeroDePagina(int numeroDePagina)
        {
            if (numeroDePagina < 1)
            {
                numeroDePagina = 1;
            }
            else if (numeroDePagina > 4)
            {
                numeroDePagina = 4;
            }
            return numeroDePagina;
        }

        public Tuple<int, int> CalcularLimitesPaginacion(int paginaMostrar)
        {
            int numeroInicioPaginacion = paginaMostrar - 1;
            numeroInicioPaginacion = this.ValidarNumeroDePagina(numeroInicioPaginacion);
            int numeroFinalPaginacion = paginaMostrar + 1;
            if (numeroInicioPaginacion == 1)
            {
                numeroFinalPaginacion = numeroInicioPaginacion + 2;
                numeroFinalPaginacion = this.ValidarNumeroDePagina(numeroFinalPaginacion);
            }
            else if (numeroFinalPaginacion == 4)
            {
                numeroInicioPaginacion = numeroFinalPaginacion - 2;
                numeroInicioPaginacion = this.ValidarNumeroDePagina(numeroInicioPaginacion);
            }
            numeroFinalPaginacion = this.ValidarNumeroDePagina(numeroFinalPaginacion);
            return new Tuple<int, int>(numeroInicioPaginacion, numeroFinalPaginacion);
        }

        public ActionResult CrearPublicacion()
        {
            return View();
        }

        public byte[] obtenerBytes(HttpPostedFileBase archivo)
        {
            byte[] bytes;
            BinaryReader lector = new BinaryReader(archivo.InputStream);
            bytes = lector.ReadBytes(archivo.ContentLength);
            return bytes;
        }

        [HttpPost]
        public ActionResult CrearPublicacion(PublicacionModel publicacionNueva)
        {
            publicacionNueva.TipoArchivo = publicacionNueva.ArchivoFoto.ContentType;
            try
            {
                if (ModelState.IsValid)
                {
                    using (WebBlogEntities baseDatos = new WebBlogEntities())
                    {

                        var publicacion = new Publicacion();
                        publicacion.fechaPK = DateTime.Now;
                        publicacion.tituloPK = publicacionNueva.Titulo;
                        publicacion.texto = publicacionNueva.Texto;
                        publicacion.imagenPublicacion = obtenerBytes(publicacionNueva.ArchivoFoto);
                        publicacion.tipoArchivo = publicacionNueva.ArchivoFoto.ContentType;
                        publicacion.idAutorFK = publicacionNueva.IdUsuario;
                        publicacion.numeroComentarios = 0;
                        publicacion.nombreCategoriaFK = publicacionNueva.Categoria;
                        baseDatos.Publicacion.Add(publicacion);
                        baseDatos.SaveChanges();
                    }
                    return Redirect("~/Publicacion");
                }
                return View(publicacionNueva);
            }
            catch (Exception excepcion)
            {
                throw new Exception(excepcion.Message);
            }

        }

        public Tuple<byte[], string> descargarContenido(string Titulo, DateTime Fecha)
        {
            byte[] bytes;
            string contentType;

            PublicacionModel publicacion = new PublicacionModel();
            using (WebBlogEntities baseDatos = new WebBlogEntities())
            {
                var publicacionTabla = baseDatos.Publicacion.Where(x => x.tituloPK == Titulo && DbFunctions.TruncateTime(x.fechaPK) == DbFunctions.TruncateTime(Fecha)).SingleOrDefault();
                bytes = publicacionTabla.imagenPublicacion;
                contentType = publicacionTabla.tipoArchivo;

            }
            return new Tuple<byte[], string>(bytes, contentType);
        }


        [HttpGet]
        public FileResult accederArchivo(string identificador, DateTime fecha)
        {
            var tupla = descargarContenido(identificador, fecha);
            return File(tupla.Item1, tupla.Item2);
        }



    }
}