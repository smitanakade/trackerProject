using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using POTracker.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using POTracker.CustomFilters;
using System.Data.SqlClient;
using System.Data;

namespace POTracker.Controllers
{
    public class RoleController : Controller
    {
        ApplicationDbContext context;
        public RoleController()
        {
            context = new ApplicationDbContext();
        }

        /// <summary>
        /// Get All Roles
        /// </summary>
        /// <returns></returns>
        /// 
        [AuthLog(Roles = "Admin")]
        public ActionResult Index()
        {
            var Roles = context.Roles.ToList();
            return View(Roles);
        }

        /// <summary>
        /// Create  a New role
        /// </summary>
        /// <returns></returns>
     //   [AuthLog(Roles = "Admin,IT")]
        public ActionResult Create()
        {
            var Role = new IdentityRole();
            return View(Role);
        }
        /// <summary>
        /// Create a New Role
        /// </summary>
        /// <param name="Role"></param>
        /// <returns></returns>
        [HttpPost]
       [AuthLog(Roles = "Admin")]
        public ActionResult Create(IdentityRole Role)
        {
            var name = Role.Name;
            var check = context.Roles.Any(r => r.Name == name);
            if (check == false)
            {
                context.Roles.Add(Role);
                context.SaveChanges();
                return RedirectToAction("Index");
            }
            else
            {
                TempData["Error"] = "This Role Already Present.Please try to create different Role!";
                return RedirectToAction("Create");

            }

        }
        [HttpGet]
        [AuthLog(Roles = "Admin")]

        public ActionResult ManageUser(string Id)
        {
            
            var model = new ShowLoginUser();
            var Context = new POTrackerDBEntities();
            model.UserList = (from u in Context.AspNetUsers
                              join ur in Context.AspNetUserRoles on u.Id equals ur.UserId
                              join r in Context.AspNetRoles on ur.RoleId equals r.Id
                              select (new ShowLoginUser
                              {
                                  UserId = u.Id,
                                  UserName = u.UserName,
                                  Email = u.Email,
                                  PhoneNumber = u.PhoneNumber,
                                  UserInRole = r.Name                                 

                              })
                     ).Where(a => a.UserId == Id).ToList();
            model.RoleList = RoleList();
            return View(model);

        }
        [HttpPost]
        [AuthLog(Roles = "Admin")]

        public ActionResult ManageUser(ShowLoginUser model)
        {
            return View(model);
        }
        //   [AuthLog(Roles = "Admin,IT")]
        //public ActionResult ShowUser()
        //{
        //    DataTable table = new DataTable();
        //    var model = new ShowLoginUser();

        //    model.UserList = table.AsEnumerable().Select(r => new ShowLoginUser(r)).ToList();

        //    return View("ShowUser", model);

        //}
        [AuthLog(Roles = "Admin,Staff")]
        public ActionResult ShowUser()
        {
            var model = new ShowLoginUser();
            var Context = new POTrackerDBEntities();
            var userData = Context.AspNetUsers.ToList();
            //model.UserList = userData.Select(U => new ShowLoginUser
            //{
            //    UserId = U.Id,
            //    UserName = U.UserName,
            //    Email = U.Email,
            //    PhoneNumber = U.PhoneNumber
            //}).ToList();

            model.UserList = (from u in Context.AspNetUsers
                              join ur in Context.AspNetUserRoles on u.Id equals ur.UserId
                              join r in Context.AspNetRoles on ur.RoleId equals r.Id
                              select (new ShowLoginUser
                              {
                                  UserId = u.Id,
                                  UserName = u.UserName,
                                  Email = u.Email,
                                  PhoneNumber = u.PhoneNumber,
                                  UserInRole = r.Name

                              })
                        ).Where(a=>a.UserInRole != "Supplier").ToList();

          
            return View("ShowUser", model);

        }
        private List<getListofRoles> RoleList()
        {
            //          select u.Id,u.UserName from AspNetUsers as u INNER JOIN AspNetUserRoles as ur on ur.UserId = u.Id
            //INNER JOIN AspNetRoles as r on r.Id = ur.RoleId where r.Name = 'InspectionTeam'
            var context = new POTrackerDBEntities();
            var query = (from u in context.AspNetUsers
                         join ur in context.AspNetUserRoles on u.Id equals ur.UserId
                         join r in context.AspNetRoles on ur.RoleId equals r.Id
                         select new { Id = u.Id, Name = u.UserName, Role = r.Name }
                         ).Where(a => a.Role != "Admin" && a.Role != "Supplier" && a.Role != "test").Select(b => new getListofRoles { Value = b.Id, Name = b.Role }).ToList();
                      
           
            return query;
        }


    }
}