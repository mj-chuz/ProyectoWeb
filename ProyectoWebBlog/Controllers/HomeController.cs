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
        public UsuarioController AccesoUsuarios;
        public CategoriaController AccesoCategorias;
        public PublicacionController AccesoPublicaciones;
        public int CantidadPaginas { get; set; }
        public HomeController()
        {
            AccesoPublicaciones = new PublicacionController();
            this.CantidadPaginas = this.CalcularTotalDePaginas(5);
            AccesoUsuarios = new UsuarioController();
            AccesoCategorias = new CategoriaController();
        }
        [AllowAnonymous]
        public ActionResult Index(int paginaMostrada = 1)
        {
            
                paginaMostrada = AccesoPublicaciones.ValidarNumeroDePagina(paginaMostrada);
                Tuple<int, int> limitesPaginacion = AccesoPublicaciones.CalcularLimitesPaginacion(paginaMostrada);
                List<PublicacionModel> publicacion = AccesoPublicaciones.ObtenerPaginaPublicaciones(paginaMostrada);
                ViewBag.NumeroInicioPaginacion = limitesPaginacion.Item1;
                ViewBag.NumeroFinalPaginacion = limitesPaginacion.Item2;
                ViewBag.PaginaActual = paginaMostrada;
                ViewBag.CantidadTotalDePaginas = this.CantidadPaginas;
                ViewBag.ListaUsuarios = AccesoUsuarios.ObtenerNombreIdUsuarios();
                ViewBag.ListaCategorias = AccesoCategorias.ObtenerNombreCategorias();
                if (User.IsInRole("Admin"))
                {
                    ViewBag.EsAdmin = true;
                    ViewBag.EsAutor = true;
                }
                else if (User.IsInRole("Autor"))
                {
                    ViewBag.EsAdmin = false;
                    ViewBag.EsAutor = true;
                }
            return View(publicacion);
        }

        public int CalcularTotalDePaginas(int publicacionesPorPagina)
        {
            List<PublicacionModel> publicaciones = AccesoPublicaciones.ObtenerPublicaciones();
            int cantidadTotalPublicaciones = publicaciones.Count;
            int cantidadTotalDePaginas = cantidadTotalPublicaciones / publicacionesPorPagina;
            if (cantidadTotalPublicaciones % publicacionesPorPagina != 0)
            {
                cantidadTotalDePaginas += 1;
            }
            return cantidadTotalDePaginas;
        }

        
        public ActionResult About()
        {
            ViewBag.Message = "Acerca de Diario AM";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Contacto";

            return View();
        }

    }
}