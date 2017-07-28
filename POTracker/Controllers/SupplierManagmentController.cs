using POTracker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace POTracker.Controllers
{
    public class SupplierManagmentController : Controller
    {
        // GET: SupplierManagment
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public  ActionResult Index(SupplierManageModel model)
        {

            /* Client Side Validation -- Start Here --*/
            if (String.IsNullOrEmpty(model.SupplierName))
            {
                ModelState.AddModelError("RepName", "Name is required.");

            }
            
         
            if (String.IsNullOrEmpty(model.Country))
            {
                ModelState.AddModelError("Country", "Country is required.");
            }
            if (String.IsNullOrEmpty(model.Email))
            {
                ModelState.AddModelError("Email", "Valid Email Address Required.");

            }
            if (!String.IsNullOrEmpty(model.Email) && !Regex.IsMatch(model.Email, @"^(?("")(""[^""]+?""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
            @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9]{2,17}))$", RegexOptions.IgnoreCase))
            {
                ModelState.AddModelError("Email", "Valid Email Address Required.");

            }

            /* Client Side Validation -- End Here --*/


            if (ModelState.IsValid)
            {
                var dbContext = new POTrackerDBEntities();

                /*Checking if record already present than dont enter into db*/

                //    if (dbContext.SupplierLoginMaster.Any(R => R.SupplierName == model.SupplierName && R.Email == model.Email))
                //    {
                //        ViewBag.Message = "Supplier Record Allready Present!!! \n Please Try Again.";
                //        ModelState.Clear();
                //        return View("Index");
                //    }
                //    else
                //    {
                //        SupplierLoginMaster Supplier = new SupplierLoginMaster()
                //        {
                //            SupplierName = model.SupplierName,
                //            Email = model.Email,
                //            SupplierStatus = "ACTIVE",
                //            Designation = model.Designation,
                //            Country = model.Country,
                //            Updatedby = "Smita",//User.Identity.Name,
                //            UpdatedOn = DateTime.Now,
                //            LoginId = null,
                //            Manage = null,
                //            Register="NO"

                //        };
                //        dbContext.SupplierLoginMaster.Add(Supplier);
                //        dbContext.SaveChanges();

                //        var Id = dbContext.SupplierLoginMaster.Where(R => R.SupplierName == model.SupplierName &&  R.Email == model.Email).Select(R => R.SupplierId).FirstOrDefault();

                //        ModelState.Clear();
                //        ViewBag.Message = "Rep Details Added Successfully.";
                //        return RedirectToAction("EditRepDetails", new { id = Id });

                //    }
                //}
                //else
                //{
                //return View(model);
                //}
            }
            return View(model);

        }

        [HttpGet]
        public ActionResult EditSupplier(string code)
        {
            /*Genrating edit rep detail page */
            ShowSupplier model = new ShowSupplier();
            var Suppliercode = code;
            var context = new POTrackerDBEntities();
            var GetSupplierDetails = context.VSupplierByCountry.Where(R => R.cre_accountcode == Suppliercode);

            model.ShowSuppList = GetSupplierDetails.Select(R => new ShowSupplier
            {
               suppliercode = R.cre_accountcode,
               suppliierName = R.cr_shortname
               //country =R.Country,
               
            }).ToList();

            model.ListofInspectionTeam = GetListofInspectionTeam();
           

            return View("EditSupplier", model);
        }
        [HttpPost]
        public ActionResult EditSupplier(ShowSupplier model)
        {
            //var dbcontext = new Entities();
            //var update  = dbcontext.
            return View();
        }

        private IEnumerable<SelectListItem> GetSupplierStatus()
        {

            List<SelectListItem> item = new List<SelectListItem>();

            item.Insert(0, (new SelectListItem { Text = "ACTIVE", Value = "ACTIVE", Selected = true }));
            item.Insert(1, (new SelectListItem { Text = "DISABLE", Value = "DISABLE", Selected = true }));


            return item;
        }
        private IEnumerable<SelectListItem> IsRegister()
        {

            List<SelectListItem> item = new List<SelectListItem>();

            item.Insert(0, (new SelectListItem { Text = "Yes", Value = "Yes", Selected = true }));
            item.Insert(1, (new SelectListItem { Text = "No", Value = "No", Selected = true }));


            return item;
        }

        private List<SelectListItem> GetCountryList()
        {
            List<SelectListItem> item = new List<SelectListItem>();
            item.Insert(0, (new SelectListItem { Text = "AU", Value = "AU", Selected = true }));
            item.Insert(1, (new SelectListItem { Text = "NZ", Value = "NZ", Selected = true }));
            item.Insert(2, (new SelectListItem { Text = "UK", Value = "UK", Selected = true }));
            return item;
        }
        private IEnumerable<SelectListItem> GetListofInspectionTeam()
        {
            var context = new POTrackerDBEntities();
            var query = (from u in context.AspNetUsers
                         join ur in context.AspNetUserRoles on u.Id equals ur.UserId
                         join r in context.AspNetRoles on ur.RoleId equals r.Id
                         select new { Id = u.Id, Name = u.UserName, Role = r.Name }
                         ).Where(a => a.Role != "Admin" && a.Role != "Supplier" && a.Role != "test").Select(b => new SelectListItem { Value = b.Id, Text = b.Role }).ToList();


            return query;
        }

        //private IEnumerable<SelectListItem> GetListofSupplier(int SupplierID)
        //{
        //    var dbContext = new Entities();
        //    var SupplierList = dbContext.SupplierLoginMaster.Where(R => R.SupplierStatus == "ACTIVE" && R.SupplierId != SupplierID && R.Designation == "Supplier").Select(R => new SelectListItem { Value=R.SupplierName, Text=R.SupplierName}).ToList();
        //    return SupplierList;


        //}

        public ActionResult RegisterSupplier()
        {
            var dbcontext = new POTrackerDBEntities();
            var model = new ShowSupplier();
             model.ShowSuppList = dbcontext.VSupplierByCountry.Select(l=> new ShowSupplier
             {

                 suppliercode= l.cre_accountcode,
                 suppliierName=l.cr_shortname,
                 country=l.Country
             }).ToList();
            return View(model);
        }
        [HttpGet]
        public ActionResult AssignGroup()

        {
            var Model = new ShowSupplier();
            var dbcontext = new POTrackerDBEntities();

            
    
            Model.ShowSuppList = dbcontext.VSupplierByCountry.Select(l => new ShowSupplier
            {
                suppliercode = l.cre_accountcode,
                suppliierName = l.cr_shortname,
                country = l.Country,
                isChecked = false
            }).Where(l => l.country == "AU").ToList();

            Model.ListofInspectionTeam = GetListofInspectionTeam();
            Model.CountryList = GetCountryList();
            return View(Model);

        }


        [HttpPost]
        public ActionResult AssignGroup(ShowSupplier Model)
        {
            var dbcontext = new POTrackerDBEntities();
            // var Model = new ShowSupplier();
            var country = Model.country;
            Model.ShowSuppList = dbcontext.VSupplierByCountry.Select(l => new ShowSupplier
            {
                suppliercode = l.cre_accountcode,
                suppliierName = l.cr_shortname,
                country = l.Country,
                isChecked = false
            }).Where(l => l.country == country).ToList();

            Model.ListofInspectionTeam = GetListofInspectionTeam();
            Model.CountryList = GetCountryList();

            return View(Model);
        }
        [HttpPost]
        public ActionResult AssignInspector(string[] selected, string Inspec,string cnty)
        {
            ShowSupplier Model = new ShowSupplier();

            SupplierInspectionRelation data = new SupplierInspectionRelation();
            var dbcontext = new POTrackerDBEntities();

            for (var i = 0; i < selected.Count(); i++)
            {


                data = new SupplierInspectionRelation()
                {
                    InspectionTeam = Inspec,
                    SupplierCode = selected[i],
                    Country = cnty
                };
              
               }
            try
            {
                dbcontext.SupplierInspectionRelation.Add(data);
                dbcontext.SaveChanges();
                ModelState.AddModelError("Sucess", "Supplier added Sucessfully!!");
            }
            catch
            {
                ModelState.AddModelError("Error", "Something Went Wrong Try Again!");
            }

            Model.country = cnty;
            Model.Inspector = Inspec;
     
            return RedirectToAction("AssignGroup", Model);
        }
        public ActionResult GetSupplierByCountry(string cnty)
        {
            var Model = new ShowSupplier();
            var dbcontext = new POTrackerDBEntities();
            if (cnty != null)
            {
                Model.ShowSuppList = dbcontext.VSupplierByCountry.Select(l => new ShowSupplier
                {
                    suppliercode = l.cre_accountcode,
                    suppliierName = l.cr_shortname,
                    country = l.Country,
                    isChecked = false
                }).Where(l => l.country == cnty).ToList();

                Model.country = cnty;
                Model.ListofInspectionTeam = GetListofInspectionTeam();
                Model.CountryList = GetCountryList();
            }
            else
            {
                ModelState.AddModelError("RepName", "Please Select Country !!");

            }

            return RedirectToAction("AssignGroup", Model);
        }
    }
}