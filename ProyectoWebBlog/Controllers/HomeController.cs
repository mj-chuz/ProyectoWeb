using ProyectoWebBlog.Models;
using ProyectoWebBlog.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProyectoWebBlog.Controllers
{
    public class HomeController : Controller
    {
        public UsuarioController AccesoAUsuarios;
        public CategoriaController AccesoCategorias;
        public int CantidadPaginas { get; set; }
        public HomeController()
        {
            AccesoAUsuarios = new UsuarioController();
            AccesoCategorias = new CategoriaController();
        }

        public ActionResult Index(int paginaMostrada = 1)
        {
            paginaMostrada = this.ValidarNumeroDePagina(paginaMostrada);
            Tuple<int, int> limitesPaginacion = this.CalcularLimitesPaginacion(paginaMostrada);
            List<PublicacionModel> publicacion = this.ObtenerPaginaPublicaciones(paginaMostrada);
            ViewBag.NumeroInicioPaginacion = limitesPaginacion.Item1;
            ViewBag.NumeroFinalPaginacion = limitesPaginacion.Item2;
            ViewBag.PaginaActual = paginaMostrada;
            ViewBag.CantidadTotalDePaginas = this.CantidadPaginas;
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

        public List<PublicacionModel> ObtenerPaginaPublicaciones(int pagina = 1)
        {
            List<PublicacionModel> publicaciones = this.ObtenerPublicaciones();
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

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
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
    }
}