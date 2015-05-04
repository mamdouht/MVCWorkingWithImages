using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WorkingWithImages.Models;

namespace WorkingWithImages.Controllers
{
    public class HTasksController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: HTasks
        public ActionResult Index()
        {
            return View(db.HTasks.ToList());
        }

        // GET: HTasks/Details/5
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HTask hTask = db.HTasks.Find(id);
            if (hTask == null)
            {
                return HttpNotFound();
            }
            return View(hTask);
        }

        // GET: HTasks/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Tasks/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Title,Discription")] HTask Task, HttpPostedFileBase image)
        {
            if (ModelState.IsValid)
            {
                if (image != null)
                {
                    //attach the uploaded image to the object before saving to Database
                    Task.ImageMimeType = image.ContentLength;
                    Task.Image = new byte[image.ContentLength];
                    image.InputStream.Read(Task.Image, 0, image.ContentLength);

                    //Save image to file
                    var filename = image.FileName;
                    
                    var filePathOriginal = Server.MapPath("/Content/Uploads/Originals");
                    var filePathThumbnail = Server.MapPath("/Content/Uploads/Thumbnails");
                    string savedFileName = Path.Combine(filePathOriginal, filename);
                    image.SaveAs(savedFileName);

                    //Read image back from file and create thumbnail from it
                    var imageFile = Path.Combine(Server.MapPath("~/Content/Uploads/Originals"), filename);
                    using (var srcImage = Image.FromFile(imageFile))
                    using (var newImage = new Bitmap(100, 100))
                    using (var graphics = Graphics.FromImage(newImage))
                    using (var stream = new MemoryStream())
                    {
                        graphics.SmoothingMode = SmoothingMode.AntiAlias;
                        graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                        graphics.DrawImage(srcImage, new Rectangle(0, 0, 100, 100));
                        newImage.Save(stream, ImageFormat.Png);
                        var thumbNew = File(stream.ToArray(), "image/png");
                        Task.thumbnail = thumbNew.FileContents;
                    }
                }
                Task.HTaskId = Guid.NewGuid();
                //Save model object to database
                db.HTasks.Add(Task);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(Task);
        }


        public FileContentResult GetThumbnailImage(Guid? TaskID)
        {
            HTask Task = db.HTasks.FirstOrDefault(p => p.HTaskId == TaskID);
            if (Task != null)
            {
                return File(Task.thumbnail, Task.ImageMimeType.ToString());
            }
            else
            {
                return null;
            }
        }

        public FileContentResult GetImage(Guid? TaskID)
        {
            HTask Task = db.HTasks.FirstOrDefault(p => p.HTaskId == TaskID);
            if (Task != null)
            {
                return File(Task.Image, Task.ImageMimeType.ToString());
            }
            else
            {
                return null;
            }
        }


        // GET: HTasks/Edit/5
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HTask hTask = db.HTasks.Find(id);
            if (hTask == null)
            {
                return HttpNotFound();
            }
            return View(hTask);
        }

        // POST: HTasks/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "HTaskId,Title,Discription,Image,ImageMimeType,thumbnail")] HTask hTask)
        {
            if (ModelState.IsValid)
            {
                db.Entry(hTask).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(hTask);
        }

        // GET: HTasks/Delete/5
        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HTask hTask = db.HTasks.Find(id);
            if (hTask == null)
            {
                return HttpNotFound();
            }
            return View(hTask);
        }

        // POST: HTasks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            HTask hTask = db.HTasks.Find(id);
            db.HTasks.Remove(hTask);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
