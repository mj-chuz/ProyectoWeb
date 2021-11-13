using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ProyectoWebBlog.Models;
using ProyectoWebBlog.Models.ViewModels;

namespace ProyectoWebBlog.Controllers
{
    public class CategoriaController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
        [Authorize(Roles = "Admin")]
        public ActionResult AgregarCategoria()
        {
            return View();
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult AgregarCategoria(CategoriaModel nuevaCategoria)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    using (WebBlogEntities baseDatos = new WebBlogEntities())
                    {
                        var categoria = new Categoria();
                        categoria.nombrePK = nuevaCategoria.Nombre;
                        baseDatos.Categoria.Add(categoria);
                        baseDatos.SaveChanges();
                    }
                    return Redirect("~/Home");
                }

                return View(nuevaCategoria);

            }
            catch (Exception excepcion)
            {
                throw new Exception(excepcion.Message);
            }
        }

        public List<String> ObtenerNombreCategorias()
        {
            List<String> nombreCategorias = new List<string>();
            List<CategoriaModel> categorias = new List<CategoriaModel>();
            using (WebBlogEntities baseDatos = new WebBlogEntities())
            {
                categorias = (from categoria in baseDatos.Categoria
                                    select new CategoriaModel
                                    {
                                        Nombre = categoria.nombrePK
                                        
                                    }).ToList();
            }
            foreach (var categoria in categorias)
            {
                nombreCategorias.Add(categoria.Nombre);
            }
            return nombreCategorias;
        }

        [Authorize(Roles = "Admin")]
        public ActionResult ObtenerListaCategorias()
        {
            List<CategoriaModel> categorias;
            using (WebBlogEntities baseDatos = new WebBlogEntities())
            {
                categorias = (from categoria in baseDatos.Categoria
                              select new CategoriaModel
                              {
                                  Nombre = categoria.nombrePK
                              }).ToList();
            }

            return View(categorias);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult EliminarCategoria(string Id)
        {
            using (WebBlogEntities baseDatos = new WebBlogEntities())
            {

                var categoriaTabla = baseDatos.Categoria.Find(Id);
                baseDatos.Categoria.Remove(categoriaTabla);
                baseDatos.SaveChanges();

            }
            return Redirect("~/Categoria/ObtenerListaCategorias");
        }

    }

}