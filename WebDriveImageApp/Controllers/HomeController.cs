using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PagedList.Core;
using WebDriveImageApp.Models;
using WebDriveImageApp.ServiseRepozitory;


namespace WebDriveImageApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IWebHostEnvironment _env;

        public HomeController(IWebHostEnvironment env, ILogger<HomeController> logger)
        {
            _env = env;
            _logger = logger;
        }
        [HttpGet]
        public ActionResult GetGoogleDriveImages(int? page)
        {
            string setsesname = "userinfo";
            string getsesname = "";
            string info = Guid.NewGuid().ToString() /*+ Guid.NewGuid().ToString()*/;
            getsesname = HttpContext.Session.GetString(setsesname);
            if (string.IsNullOrEmpty(getsesname))
            {
                HttpContext.Session.SetString(setsesname, info);
            }
            getsesname = HttpContext.Session.GetString(setsesname);
            
            IQueryable<Image> images = DriveMethod.GetDriveFiles(getsesname).AsQueryable();
            var GetArticles = images.ToPagedList(page ?? 1, 12);
            return View(GetArticles);
        }
        public ActionResult Profile()
        {
            string setsesname = "userinfo";
            string getsesname = "";
            string info = Guid.NewGuid().ToString() /*+ Guid.NewGuid().ToString()*/;
            getsesname = HttpContext.Session.GetString(setsesname);
            if (string.IsNullOrEmpty(getsesname))
            {
                HttpContext.Session.SetString(setsesname, info);
            }
            getsesname = HttpContext.Session.GetString(setsesname);
            return View(DriveMethod.Getuser(getsesname));
        }
        public ActionResult login()
        {
            string setsesname = "userinfo";
            string getsesname = "";
            string info = Guid.NewGuid().ToString() + Guid.NewGuid().ToString();
            getsesname = HttpContext.Session.GetString(setsesname);
            if (string.IsNullOrEmpty(getsesname))
            {
                HttpContext.Session.SetString(setsesname, info);
            }
            getsesname = HttpContext.Session.GetString(setsesname);
            DriveMethod.Getuser(getsesname);
            return RedirectToAction("Profile");
        }
        public IActionResult Index()
        {
           
            return View();
        }
        [HttpPost]
        public ActionResult UploadFile(IFormFile file)
        {
            string setsesname = "userinfo";
            string getsesname = "";
            string info = Guid.NewGuid().ToString() /*+ Guid.NewGuid().ToString()*/;
            getsesname = HttpContext.Session.GetString(setsesname);
            if (string.IsNullOrEmpty(getsesname))
            {
                HttpContext.Session.SetString(setsesname, info);
            }
            getsesname = HttpContext.Session.GetString(setsesname);
            new DriveMethod(_env,getsesname).FileUpload(file);
            string path = Path.Combine(_env.WebRootPath, "image", "GoogleDriveFiles");
            var filePath = Path.Combine(path, file.FileName);
            System.IO.File.Delete(filePath);
            return RedirectToAction("GetGoogleDriveImages");
        }
       


       
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        

    }
}
