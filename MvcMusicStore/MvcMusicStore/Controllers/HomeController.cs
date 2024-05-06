using MvcMusicStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MvcMusicStore.Controllers
{
    public class HomeController : Controller
    {
        MvcMusicStoreEntities db = new MvcMusicStoreEntities();
        // GET: Home
        public ActionResult Index()
        {
            var albums = GetTopSellingAlbums(5);
            return View(albums);
        }
        private List<Album> GetTopSellingAlbums(int count)
        {
            // Group the order details by album and return
            // the albums with the highest count
            return db.Albums.OrderByDescending(a => a.OrderDetails.Count()).Take(count).ToList();
        }
    }
}