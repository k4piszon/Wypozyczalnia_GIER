using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Company.DAL;
using Company.Models;
using Company.ViewModels;
using PagedList;

namespace Company.Controllers
{
    public class GamesController : Controller
    {
        private CompanyContext db = new CompanyContext();

        // GET: Books
        /*  public ActionResult Index()
        {
            return View(db.Books.ToList());
        }*/

        public ActionResult Index(string sortOrder, string searchString, int? page, string currentFilter)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.GameSortParm = sortOrder == "Game" ? "Game_desc" : "Game_desc";
            ViewBag.DoubleSortParm = sortOrder == "Double" ? "double_desc" : "Double";
            var games = from a in db.Games select a;
            if (!String.IsNullOrEmpty(searchString) )
            {
                games = db.Games.Where(a => a.Title.Contains(searchString));
            }
            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            switch (sortOrder)
            {
                case "Game_desc":
                    games = games.OrderByDescending(a => a.Title);
                    break;
                case "Game":
                    games = games.OrderBy(a => a.Title);
                    break;
                case "double_desc":
                    games = games.OrderByDescending(a => a.PricePerPieces);
                    break;
                case "Double":
                    games = games.OrderBy(a => a.PricePerPieces);
                    break;
                case "name_desc":
                    games = games.OrderByDescending(a => a.Plant);
                    break;
                default:
                    games = games.OrderBy(a => a.Plant);
                    break;


            }
            int pageSize = 3;
            int pageNumber = (page ?? 1);
            return View(games.ToPagedList(pageNumber, pageSize));
        }

        // GET: Books/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Game game = db.Games.Find(id);
            if (game == null)
            {
                return HttpNotFound();
            }
            Orders orders = new Orders
            {
                GameID = game.ID
            };

            IEnumerable<Orders> ordersEn = db.Orders.Where(o => o.GameID.Equals(game.ID));

            GameOrdersViewModel viewModel = new GameOrdersViewModel
            {
                GameVM = game,
                OrdersVM = orders,
                OrdersVME = ordersEn

            };
            return View(viewModel);
        }


        // GET: Books/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Books/Create
        // Aby zapewnić ochronę przed atakami polegającymi na przesyłaniu dodatkowych danych, włącz określone właściwości, z którymi chcesz utworzyć powiązania.
        // Aby uzyskać więcej szczegółów, zobacz https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Title,Plant,EquipmentDesc,gamekind,PricePerPieces,quantity,Description,Image")] Game book)
        {
            if (ModelState.IsValid)
            {
                UpdateModel(book);
                HttpPostedFileBase file = Request.Files["fileWithImage"];
                if (file != null && file.ContentLength > 0)
                {
                    book.Image = Guid.NewGuid() + Path.GetExtension(file.FileName);
                    file.SaveAs(HttpContext.Server.MapPath("~/Images/") + book.Image);
                }
                db.Games.Add(book);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(book);
        }

        // GET: Books/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Game book = db.Games.Find(id);
            if (book == null)
            {
                return HttpNotFound();
            }
            return View(book);
        }

        // POST: Books/Edit/5
        // Aby zapewnić ochronę przed atakami polegającymi na przesyłaniu dodatkowych danych, włącz określone właściwości, z którymi chcesz utworzyć powiązania.
        // Aby uzyskać więcej szczegółów, zobacz https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Title,Plant,EquipmentDesc,gamekind,PricePerPieces,quantity,Description,Image")] Game book)
        {
            if (ModelState.IsValid)
            {
                UpdateModel(book);
                HttpPostedFileBase file = Request.Files["fileWithImage"];
                if (file != null && file.ContentLength > 0)
                {
                    book.Image = Guid.NewGuid() + Path.GetExtension(file.FileName);
                    file.SaveAs(HttpContext.Server.MapPath("~/Images/") + book.Image);
                }
                db.Entry(book).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(book);
        }

        // GET: Books/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Game book = db.Games.Find(id);
            if (book == null)
            {
                return HttpNotFound();
            }
            return View(book);
        }

        // POST: Books/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Game book = db.Games.Find(id);
            db.Games.Remove(book);
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
