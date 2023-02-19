using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Company.DAL;
using Company.Models;
using _Excel = Microsoft.Office.Interop.Excel;
using System.IO;
using Spire.Xls;

namespace Company.Controllers
{
    public class SuppliersController : Controller
    {
        private CompanyContext db = new CompanyContext();

        // GET: Suppliers
        public ActionResult Index()
        {
            var suppliers = db.Suppliers.Include(s => s.game);
            return View(suppliers.ToList());
        }

        // GET: Suppliers/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Suppliers suppliers = db.Suppliers.Find(id);
            if (suppliers == null)
            {
                return HttpNotFound();
            }
            return View(suppliers);
        }

        // GET: Suppliers/Create
        public ActionResult Create()
        {
            ViewBag.BookID = new SelectList(db.Games, "ID", "Title");
            return View();
        }

        // POST: Suppliers/Create
        // Aby zapewnić ochronę przed atakami polegającymi na przesyłaniu dodatkowych danych, włącz określone właściwości, z którymi chcesz utworzyć powiązania.
        // Aby uzyskać więcej szczegółów, zobacz https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,NameCompany,GameID,quantity")] Suppliers suppliers)
        {
            if (ModelState.IsValid)
            {
                suppliers.game = db.Games.Single(o => o.ID.Equals(suppliers.GameID));
                suppliers.game.quantity = suppliers.game.quantity + suppliers.quantity;
                db.Suppliers.Add(suppliers);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ID = new SelectList(db.Games, "ID", "Titile", suppliers.GameID);
            return View(suppliers);
        }

        // GET: Suppliers/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Suppliers suppliers = db.Suppliers.Find(id);
            if (suppliers == null)
            {
                return HttpNotFound();
            }
            ViewBag.ID = new SelectList(db.Games, "ID", "Titile", suppliers.GameID);
            return View(suppliers);
        }

        // POST: Suppliers/Edit/5
        // Aby zapewnić ochronę przed atakami polegającymi na przesyłaniu dodatkowych danych, włącz określone właściwości, z którymi chcesz utworzyć powiązania.
        // Aby uzyskać więcej szczegółów, zobacz https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,NameCompany,GameID,quantity")] Suppliers suppliers)
        {
            if (ModelState.IsValid)
            {
                db.Entry(suppliers).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ID = new SelectList(db.Games, "ID", "Titile", suppliers.GameID);
            return View(suppliers);
        }


        [HttpGet]
        public ActionResult OpenFile()
        {
            return View();
        }
        [HttpPost]
        public ActionResult OpenFile(HttpPostedFileBase excelFile)
        {
            try
            {
                string filename = excelFile.FileName;
                //Path.GetExtension(excelFile.FileName);
                if (excelFile != null && (filename.EndsWith(".xls")))
                {
                    string path = Server.MapPath("~/Content/") + Guid.NewGuid() + filename;
                    excelFile.SaveAs(path);
                    Spire.Xls.Workbook workbook = new Spire.Xls.Workbook();
                    workbook.LoadFromFile($@"{path}");
                    Spire.Xls.Worksheet sheet = workbook.Worksheets[0];
                    string FinalPath = HttpContext.Server.MapPath("~/Files/");
                    sheet.SaveToFile($@"{FinalPath}" + $@"{filename.Replace(".xls", ".csv")}", ",", System.Text.Encoding.UTF8);

                    StreamReader reader = null;
                    int lncnt = 0;
                    List<string> line;
                    string csv = filename.Replace(".xls", ".csv");
                    string pathDownload = FinalPath + csv;
                    reader = new StreamReader(Path.Combine(pathDownload), System.Text.Encoding.UTF8);
                    string header = reader.ReadLine();
                    string hd = @"""game_id"",""company name"",""quantity""";
                    List<Suppliers> suppliers = new List<Suppliers>();
                    if (header.Replace(" ", "") == hd.Replace(" ", ""))
                    {
                        while (!reader.EndOfStream)
                        {
                            lncnt++;
                            line = reader.ReadLine().Split(',').Select(t => t.Trim('"', '\'')).ToList();
                            Suppliers s = new Suppliers();
                            s.GameID = int.Parse(line[0]);
                            s.NameCompany = line[1];
                            s.quantity = int.Parse(line[2]);
                            s.game = db.Games.Single(o => o.ID.Equals(s.GameID));
                            s.game.quantity = s.game.quantity + s.quantity;
                            suppliers.Add(s);

                        }
                        foreach (var item in suppliers)
                        {
                            db.Suppliers.Add(item);
                        }

                        db.SaveChanges();

                        reader.Close();

                        Files f = new Files(FinalPath + filename.Replace(".xls", ".csv"));
                        f.RemoveFile();
                        return RedirectToAction("Index");
                    }


                }
            }
            catch (FileNotFoundException e)
            {
                Console.WriteLine("Nie znaleziono pliku" + e);
            }
            // var parti = from a in db.ShopTable select a;
            // return View(parti.ToList());
            return RedirectToAction("Index");
        }



        // GET: Suppliers/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Suppliers suppliers = db.Suppliers.Find(id);
            if (suppliers == null)
            {
                return HttpNotFound();
            }
            return View(suppliers);
        }

        // POST: Suppliers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Suppliers suppliers = db.Suppliers.Find(id);
            db.Suppliers.Remove(suppliers);
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
