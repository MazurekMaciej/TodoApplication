using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using TodoApplication.DAL;
using TodoApplication.Models;

namespace TodoApplication.Controllers
{
    public class ToDoController : Controller
    {
        private CrmContext db = new CrmContext();

        // GET: ToDoModels
        public ActionResult Index()
        {
          //  if (Roles.IsUserInRole("User"))
           // {
                var iduser = (int)(Session["iduser"]);
                var PersonTodo = db.ToDoModels.Where(c => c.PersonId == iduser);
                return View(PersonTodo.ToList());
           // }
         
           // if (Roles.IsUserInRole("Admin"))
          //  {

            //    return View(db.ToDoModels.ToList());
          //  }
           // return View(db.ToDoModels.ToList());
        }

        // GET: ToDoModels/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ToDoModels toDoModels = db.ToDoModels.Find(id);
            if (toDoModels == null)
            {
                return HttpNotFound();
            }
            return View(toDoModels);
        }

        // GET: ToDoModels/Create
        public ActionResult Create()
        {
           ToDoModels todo = new ToDoModels();
            {
               todo.PersonId = (int)Session["iduser"];
               todo.Date = DateTime.Now;
            };
            return View(todo);
        }

        // POST: ToDoModels/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Title,Details,Date,IsDone,PersonId")] ToDoModels toDoModels)
        {
            if (ModelState.IsValid)
            {
                db.ToDoModels.Add(toDoModels);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(toDoModels);
        }

        // GET: ToDoModels/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ToDoModels toDoModels = db.ToDoModels.Find(id);
            if (toDoModels == null)
            {
                return HttpNotFound();
            }
            return View(toDoModels);
        }

        // POST: ToDoModels/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Title,Details,Date,IsDone,PersonId")] ToDoModels toDoModels)
        {
            if (ModelState.IsValid)
            {
                db.Entry(toDoModels).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(toDoModels);
        }

        // GET: ToDoModels/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ToDoModels toDoModels = db.ToDoModels.Find(id);
            if (toDoModels == null)
            {
                return HttpNotFound();
            }
            return View(toDoModels);
        }

        // POST: ToDoModels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ToDoModels toDoModels = db.ToDoModels.Find(id);
            db.ToDoModels.Remove(toDoModels);
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
