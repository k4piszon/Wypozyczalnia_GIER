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
using Microsoft.AspNet.Identity;
using System.Net.Mail;
using Company.ViewModels;
using MimeKit;
using MailKit.Security;
using MailKit.Net.Smtp;

using MailKit.Search;
using MailKit.Net.Imap;
using System.IO;
using MimeKit.Text;


namespace Company.Controllers
{
    public class OrdersController : Controller
    {
        private CompanyContext db = new CompanyContext();

        // GET: Orders
        public ActionResult Index()
        {
            var orders = db.Orders.Include(o => o.game).Include(o => o.profile);
            if (User.IsInRole("Admin"))
            {
                return View(orders.ToList());
            }
            else
            {
                return View(db.Orders.Where(o => o.profile.UserName == User.Identity.Name).ToList());
            }
        }


        // GET: Orders/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Orders orders = db.Orders.Find(id);
            if (orders == null)
            {
                return HttpNotFound();
            }
            return View(orders);
        }

        // GET: Orders/Create
        public ActionResult Create()
        {
            ViewBag.BookID = new SelectList(db.Games, "ID", "Title");
            ViewBag.ProfileID = new SelectList(db.Profiles, "ID", "UserName");
            return View();
        }

        // POST: Orders/Create
        // Aby zapewnić ochronę przed atakami polegającymi na przesyłaniu dodatkowych danych, włącz określone właściwości, z którymi chcesz utworzyć powiązania.
        // Aby uzyskać więcej szczegółów, zobacz https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Prefix = "OrdersVM", Include = "ID,ProfileID,GameID,NumberOfPieces,Price,status,TransactionDate")] Orders orders)
        {
            if (ModelState.IsValid)
            {
                orders.status = Orders.Status.nieopłacone;
                Profile user = db.Profiles.Single(o => o.UserName.Equals(User.Identity.Name));
                orders.ProfileID = user.ID;
                orders.profile = db.Profiles.Single(o => o.ID.Equals(orders.ProfileID));
                orders.TransactionDate = DateTime.Now.ToString("dd/MM/yyyy");
                orders.game = db.Games.Single(o => o.ID.Equals(orders.GameID));
                orders.Price = orders.NumberOfPieces * orders.game.PricePerPieces;
                orders.game.quantity = orders.game.quantity - orders.NumberOfPieces;
                db.Orders.Add(orders);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.BookID = new SelectList(db.Games, "ID", "Title", orders.GameID);
            ViewBag.ProfileID = 1;
            return View(orders);
        }

        // GET: Orders/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Orders orders = db.Orders.Find(id);
            if (orders == null)
            {
                return HttpNotFound();
            }
            ViewBag.GameID = new SelectList(db.Games, "ID", "Title", orders.GameID);
            ViewBag.ProfileID = new SelectList(db.Profiles, "ID", "UserName", orders.ProfileID);
            return View(orders);
        }

        // POST: Orders/Edit/5
        // Aby zapewnić ochronę przed atakami polegającymi na przesyłaniu dodatkowych danych, włącz określone właściwości, z którymi chcesz utworzyć powiązania.
        // Aby uzyskać więcej szczegółów, zobacz https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,ProfileID,GameID,NumberOfPieces,Price,status,TransactionDate")] Orders orders)
        {
            if (ModelState.IsValid)
            {
                db.Entry(orders).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.GameID = new SelectList(db.Games, "ID", "Title", orders.GameID);
            ViewBag.ProfileID = new SelectList(db.Profiles, "ID", "UserName", orders.ProfileID);
            return View(orders);
        }
       

        [HttpPost]
        public ActionResult SendConfirmMessage(Orders orders)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    String username = "shopex1234@outlook.com";
                    String pass = "Xena1994!";
                    Profile profile = db.Profiles.Single(p => p.ID.Equals(orders.ProfileID));
                    var email = new MimeMessage();
                    email.From.Add(MailboxAddress.Parse(username));
                    var receiverEmail = new MailAddress(orders.profile.UserName, "Receiver");
                    email.To.Add(MailboxAddress.Parse(receiverEmail.ToString()));
                    var subject = "Potwierdzenie Zakupu!";
                    var body = $"Szanowny Kliencie! " + "\n" +
                        $"Przesyłam potwierdzenie zakupu : Gra {orders.game.gamekind} {orders.game.Title}  " +
                        $"w liczbie sztuk : {orders.NumberOfPieces} , towar dotrze w przeciagu tygodnia dzieki darmowej przesyłce zawartej w cenie naszych usług!." + "\n" +
                        $"Koszt który Państwo uiścili to  {orders.Price} PLN." + $"W razie wszelkich pytań porosimy o kontakt podany na stronie sklepu." + "\n" + "\n" +
                        $"Dziękujemy za zaufanie i wybranie sklepu Gerald!" + "\n" + "\n" +
                        $"____________________________FAKTURA___________________________________" + "\n" +
                        $"Pan/ Pani :  {profile.Name}  {profile.Surname}" + "\n" +
                        $"zamieszkały/ła w {profile.Address}" + "\n" +
                        $"Gatunek gry : {orders.game.gamekind} " + "\n" +
                        $"Tytuł:  {orders.game.Title} " + "\n" +
                        $"Wytwórnia:  {orders.game.Plant} " + "\n" +
                        $"W terminie dnia {orders.TransactionDate} roku" + "\n" +
                        $"w liczbie sztuk : {orders.NumberOfPieces}" + "\n" +
                        $"Koszt : {orders.Price} PLN";
                    email.Subject = subject.ToString();
                    email.Body = new TextPart(TextFormat.Plain) { Text = body };
                    using (var smtp = new MailKit.Net.Smtp.SmtpClient())
                    {

                        smtp.ServerCertificateValidationCallback = (s, c, h, e) => true;
                        smtp.Connect("smtp-mail.outlook.com", 587, SecureSocketOptions.StartTls);
                        smtp.Authenticate(username, pass);
                        smtp.Send(email);
                        smtp.Disconnect(true);
                    }

                }
            }
            catch (Exception)
            {
                ViewBag.Error = "Nie udało się wysłać email!";
            }
            return View();
        }















        public ActionResult Pay([Bind(Include = "ID")] Orders orders)
        {
            if (ModelState.IsValid)
            {
                orders = db.Orders.Single(o => o.ID.Equals(orders.ID));
                db.Entry(orders).State = EntityState.Modified;
                orders.status = Orders.Status.opłacone;
                db.SaveChanges();
                SendConfirmMessage(orders);
                return RedirectToAction("Index");
            }
            return View(orders);
        }
        // GET: Orders/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Orders orders = db.Orders.Find(id);
            if (orders == null)
            {
                return HttpNotFound();
            }
            return View(orders);
        }

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Orders orders = db.Orders.Find(id);
            if(orders.status== Orders.Status.nieopłacone)
            {
                orders.game.quantity = orders.game.quantity + orders.NumberOfPieces;
            }
            db.Orders.Remove(orders);
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
