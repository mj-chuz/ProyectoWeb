using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ProyectoWebBlog.Models;
using ProyectoWebBlog.Models.ViewModels;
using System.IO;
using System.Data.Entity;
using System.Diagnostics;
using System.Globalization;

namespace ProyectoWebBlog.Controllers
{
    public class PublicacionController : Controller
    {
        public UsuarioController AccesoAUsuarios;
        public CategoriaController AccesoCategorias;
        public int CantidadPaginas { get; set; }
        public PublicacionController()
        {
            AccesoCategorias = new CategoriaController();
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

        public void DisminuirComentarios(string Titulo, DateTime Fecha)
        {
            using (WebBlogEntities baseDatos = new WebBlogEntities())
            {
                var publicacionTabla = baseDatos.Publicacion.Where(x => x.tituloPK == Titulo && x.fechaPK == Fecha).SingleOrDefault();
                publicacionTabla.numeroComentarios -= 1;
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
                                 orderby publicacionActual.fechaPK descending
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

        public List<PublicacionModel> ObtenerPublicacionesSegunCategoria(string nombreCategoria)
        {
            List<PublicacionModel> publicaciones;
            using (WebBlogEntities baseDatos = new WebBlogEntities())
            {
                publicaciones = (from publicacionActual in baseDatos.Publicacion.Where(x => x.nombreCategoriaFK == nombreCategoria)
                                 orderby publicacionActual.fechaPK descending
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

        public ActionResult VerPublicacionesCategoria(string nombreCategoria, int paginaMostrada = 1)
        {
            paginaMostrada = this.ValidarNumeroDePagina(paginaMostrada);
            Tuple<int, int> limitesPaginacion = this.CalcularLimitesPaginacion(paginaMostrada);
            List<PublicacionModel> publicacion = this.ObtenerPaginaPublicacionesCategoria(nombreCategoria, paginaMostrada);
            ViewBag.NumeroInicioPaginacion = limitesPaginacion.Item1;
            ViewBag.NumeroFinalPaginacion = limitesPaginacion.Item2;
            ViewBag.PaginaActual = paginaMostrada;
            ViewBag.CantidadTotalDePaginas = this.CantidadPaginas;
            ViewBag.Categoria = nombreCategoria;
            ViewBag.ListaUsuarios = AccesoAUsuarios.ObtenerNombreIdUsuarios();
            ViewBag.ListaCategorias = AccesoCategorias.ObtenerNombreCategorias();
            return View(publicacion);
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
            ViewBag.ListaCategorias = AccesoCategorias.ObtenerNombreCategorias();
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
            int indiceInicio = (pagina - 1) * 5;
            int indiceFinal = indiceInicio + 5;
            Tuple<int, int> indicesBusqueda = new Tuple<int, int>(indiceInicio, indiceFinal);
            return this.ObtenerPublicacionesPaginacion(indicesBusqueda, publicaciones);
        }


        public List<PublicacionModel> ObtenerPaginaPublicacionesCategoria(string nombreCategoria, int pagina = 1)
        {
            List<PublicacionModel> publicaciones = this.ObtenerPublicacionesSegunCategoria(nombreCategoria);
            int indiceInicio = (pagina - 1) * 5;
            int indiceFinal = indiceInicio + 5;
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
                               orderby publicacionActual.fechaPK descending
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

        public ActionResult VerPublicacion(string Titulo, String Fecha, String autor)
        {

            DateTime fechaParseada = DateTime.ParseExact(Fecha, "yyyyMMddHHmmss", CultureInfo.InvariantCulture);
            PublicacionModel publicacion = new PublicacionModel();
            using (WebBlogEntities baseDatos = new WebBlogEntities())
            {
                var publicacionTabla = baseDatos.Publicacion.Where(x => x.tituloPK == Titulo && DbFunctions.TruncateTime(x.fechaPK) == DbFunctions.TruncateTime(fechaParseada)).SingleOrDefault();
                publicacion.Fecha = publicacionTabla.fechaPK;
                publicacion.Texto = publicacionTabla.texto;
                publicacion.Titulo = publicacionTabla.tituloPK;
                publicacion.Categoria = publicacionTabla.nombreCategoriaFK;
                publicacion.IdUsuario = publicacionTabla.idAutorFK;
                publicacion.NumeroComentarios = (int)publicacionTabla.numeroComentarios;
            }
            ViewBag.publicacion = publicacion;
            ViewBag.autor = autor;
            return View();
        }

        public int ValidarNumeroDePagina(int numeroDePagina)
        {
            if (numeroDePagina < 1)
            {
                numeroDePagina = 1;
            }
            else if (numeroDePagina > 5)
            {
                numeroDePagina = 5;
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
            else if (numeroFinalPaginacion == 5)
            {
                numeroInicioPaginacion = numeroFinalPaginacion - 2;
                numeroInicioPaginacion = this.ValidarNumeroDePagina(numeroInicioPaginacion);
            }
            numeroFinalPaginacion = this.ValidarNumeroDePagina(numeroFinalPaginacion);
            return new Tuple<int, int>(numeroInicioPaginacion, numeroFinalPaginacion);
        }
        [Authorize(Roles = "Admin, Autor")]
        public ActionResult CrearPublicacion()
        {
            List<SelectListItem> categorias = this.ObtenerCategorias();
            ViewData["categorias"] = categorias;
            ViewBag.UsuarioId = User.Identity.Name;
            return View();
        }

        public byte[] obtenerBytes(HttpPostedFileBase archivo)
        {
            byte[] bytes;
            BinaryReader lector = new BinaryReader(archivo.InputStream);
            bytes = lector.ReadBytes(archivo.ContentLength);
            return bytes;
        }

        public List<SelectListItem> ObtenerCategorias()
        {

            List<String> categorias = AccesoCategorias.ObtenerNombreCategorias();
            List<SelectListItem> categoriasParseadas = new List<SelectListItem>();
            foreach (string categoria in categorias)
            {
                categoriasParseadas.Add(new SelectListItem { Text = categoria, Value = categoria });
            }
            return categoriasParseadas;
        }
        [Authorize(Roles = "Admin, Autor")]
        [HttpPost]
        public ActionResult CrearPublicacion(PublicacionModel publicacionNueva)
        {
            List<SelectListItem> categorias = this.ObtenerCategorias();
            ViewData["categorias"] = categorias;
            publicacionNueva.Categoria = Request.Form["categoria"];
            publicacionNueva.TipoArchivo = publicacionNueva.ArchivoFoto.ContentType;
            try
            {
                if (ModelState.IsValid)
                {
                    using (WebBlogEntities baseDatos = new WebBlogEntities())
                    {

                        var publicacion = new Publicacion();
                        publicacion.fechaPK = DateTime.ParseExact(DateTime.Now.ToString("yyyyMMddHHmmss"), "yyyyMMddHHmmss", CultureInfo.InvariantCulture);
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
                    return Redirect("~/Home");
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

        public List<PublicacionModel> ObtenerPaginaPublicaciones(int pagina = 1)
        {
            List<PublicacionModel> publicaciones = this.ObtenerPublicaciones();
            int indiceInicio = (pagina - 1) * 5;
            int indiceFinal = indiceInicio + 5;
            Tuple<int, int> indicesBusqueda = new Tuple<int, int>(indiceInicio, indiceFinal);
            return this.ObtenerPublicacionesPaginacion(indicesBusqueda, publicaciones);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult EliminarPublicacion(string Titulo, String Fecha)
        {
            using (WebBlogEntities baseDatos = new WebBlogEntities())
            {

                DateTime fechaParseada = DateTime.ParseExact(Fecha, "yyyyMMddHHmmss", CultureInfo.InvariantCulture);
                var publicacionTabla = baseDatos.Publicacion.Where(x => x.tituloPK == Titulo && DbFunctions.TruncateTime(x.fechaPK) == DbFunctions.TruncateTime(fechaParseada)).SingleOrDefault();
                baseDatos.Publicacion.Attach(publicacionTabla);
                baseDatos.Publicacion.Remove(publicacionTabla);
                baseDatos.SaveChanges();

            }
            return Redirect("~/Home");
        }

    }
}