using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using POTracker.Models;
using POTracker.CustomFilters;
using Microsoft.AspNet.Identity;

namespace POTracker.Controllers
{
    public class HomeController : Controller
    {
        //[AuthLog(Roles = "Admin,InspectionTeam")]
        public ActionResult AUIndex()
        {
            
            var auPurchaseOrders = GetRoleBasedPO();
            return View(auPurchaseOrders);
        }
        //  public ActionResult AUIndex(string searchBy, string searchValue)
        public ActionResult GetAUTrackerGrid(string searchBy, string searchValue, string country)
        {
          
               var dbContext = new POTrackerDBEntities();
          var loginUser = User.Identity.GetUserName();
            List<AUPurchaseOrder> auPurchaseOrders = new List<AUPurchaseOrder>();
            List<UKPurchaseOrder> ukPurchaseOrders = new List<UKPurchaseOrder>();

            List<string> ListOfSupplier = new List<string>();
            if (country.Equals("UK"))
            {
                ukPurchaseOrders = UKGetRoleBasedPO();
            }
            else
            {
                auPurchaseOrders = GetRoleBasedPO();
            }
            if (!String.IsNullOrEmpty(searchBy) && !String.IsNullOrEmpty(searchValue))
            {
                searchValue = searchValue.Trim().ToLower();
                if (country.Equals("AU"))
                {
                   

                    switch (searchBy)
                    {

                        case "Stratum":
                            auPurchaseOrders.ForEach(p =>
                            {

                                p.OrderLines = dbContext.VAUPurchaseOrderLines.Where(l => l.po_order_no == p.po_order_no && l.po_backorder_flag == p.po_backorder_flag && l.ValidPOL && l.stock_group.ToLower().Contains(searchValue)).ToList();
                                p.OrderComments = dbContext.AUOrderTrackerComments.Where(c => c.OrderNumber == p.po_order_no).ToList();
                            }
                            );
                            auPurchaseOrders = auPurchaseOrders.Where(p => p.OrderLines.Count() > 0).ToList();
                            break;
                        case "Purchase Order":
                            auPurchaseOrders = auPurchaseOrders.Where(p => String.Format("{0}{1}", p.po_order_no, p.po_backorder_flag.Trim()).ToLower().Contains(searchValue)).ToList();
                            auPurchaseOrders.ForEach(p =>
                            {
                                p.OrderLines = dbContext.VAUPurchaseOrderLines.Where(l => l.po_order_no == p.po_order_no && l.po_backorder_flag == p.po_backorder_flag && l.ValidPOL).ToList();
                                p.OrderComments = dbContext.AUOrderTrackerComments.Where(c => c.OrderNumber == p.po_order_no).ToList();
                            }
                             );
                            break;
                        case "User":
                            auPurchaseOrders = auPurchaseOrders.Where(p => p.po_user_name.ToLower().Contains(searchValue)).ToList();
                            auPurchaseOrders.ForEach(p =>
                            {
                                p.OrderLines = dbContext.VAUPurchaseOrderLines.Where(l => l.po_order_no == p.po_order_no && l.po_backorder_flag == p.po_backorder_flag && l.ValidPOL).ToList();
                                p.OrderComments = dbContext.AUOrderTrackerComments.Where(c => c.OrderNumber == p.po_order_no).ToList();
                            }
                            );
                            break;
                        case "Supplier":
                            auPurchaseOrders = auPurchaseOrders.Where(p => p.cre_accountcode.ToLower().Contains(searchValue)).ToList();
                            auPurchaseOrders.ForEach(p =>
                            {
                                p.OrderLines = dbContext.VAUPurchaseOrderLines.Where(l => l.po_order_no == p.po_order_no && l.po_backorder_flag == p.po_backorder_flag && l.ValidPOL).ToList();
                                p.OrderComments = dbContext.AUOrderTrackerComments.Where(c => c.OrderNumber == p.po_order_no).ToList();
                            }
                            );
                            break;
                        case "Whse Code":
                            auPurchaseOrders = auPurchaseOrders.Where(p => p.po_whse_code.ToLower().Contains(searchValue)).ToList();
                            auPurchaseOrders.ForEach(p =>
                            {
                                p.OrderLines = dbContext.VAUPurchaseOrderLines.Where(l => l.po_order_no == p.po_order_no && l.po_backorder_flag == p.po_backorder_flag).ToList();
                                p.OrderComments = dbContext.AUOrderTrackerComments.Where(c => c.OrderNumber == p.po_order_no).ToList();

                            }
                            );
                            break;
                    } //Switch case ending 
                }
                if (country.Equals("UK"))
                {
                

                    switch (searchBy)
                    {

                        case "Stratum":
                            ukPurchaseOrders.ForEach(p =>
                            {

                                p.UKOrderLines = dbContext.VUKPurchaseOrderLines.Where(l => l.po_order_no == p.po_order_no && l.po_backorder_flag == p.po_backorder_flag && l.ValidPOL && l.stock_group.ToLower().Contains(searchValue)).ToList();
                                p.UKOrderComments = dbContext.UKOrderTrackerComments.Where(c => c.OrderNumber == p.po_order_no).ToList();
                            }
                            );
                            ukPurchaseOrders = ukPurchaseOrders.Where(p => p.UKOrderLines.Count() > 0).ToList();
                            break;
                        case "Purchase Order":
                            ukPurchaseOrders = ukPurchaseOrders.Where(p => String.Format("{0}{1}", p.po_order_no, p.po_backorder_flag.Trim()).ToLower().Contains(searchValue)).ToList();
                            ukPurchaseOrders.ForEach(p =>
                            {
                                p.UKOrderLines = dbContext.VUKPurchaseOrderLines.Where(l => l.po_order_no == p.po_order_no && l.po_backorder_flag == p.po_backorder_flag && l.ValidPOL).ToList();
                                p.UKOrderComments = dbContext.UKOrderTrackerComments.Where(c => c.OrderNumber == p.po_order_no).ToList();
                            }
                             );
                            break;
                        case "User":
                            ukPurchaseOrders = ukPurchaseOrders.Where(p => p.po_user_name.ToLower().Contains(searchValue)).ToList();
                            ukPurchaseOrders.ForEach(p =>
                            {
                                p.UKOrderLines = dbContext.VUKPurchaseOrderLines.Where(l => l.po_order_no == p.po_order_no && l.po_backorder_flag == p.po_backorder_flag && l.ValidPOL).ToList();
                                p.UKOrderComments = dbContext.UKOrderTrackerComments.Where(c => c.OrderNumber == p.po_order_no).ToList();
                            }
                            );
                            break;
                        case "Supplier":
                            ukPurchaseOrders = ukPurchaseOrders.Where(p => p.cre_accountcode.ToLower().Contains(searchValue)).ToList();
                            ukPurchaseOrders.ForEach(p =>
                            {
                                p.UKOrderLines = dbContext.VUKPurchaseOrderLines.Where(l => l.po_order_no == p.po_order_no && l.po_backorder_flag == p.po_backorder_flag && l.ValidPOL).ToList();
                                p.UKOrderComments = dbContext.UKOrderTrackerComments.Where(c => c.OrderNumber == p.po_order_no).ToList();
                            }
                            );
                            break;
                        case "Whse Code":
                            ukPurchaseOrders = ukPurchaseOrders.Where(p => p.po_whse_code.ToLower().Contains(searchValue)).ToList();
                            ukPurchaseOrders.ForEach(p =>
                            {
                                p.UKOrderLines = dbContext.VUKPurchaseOrderLines.Where(l => l.po_order_no == p.po_order_no && l.po_backorder_flag == p.po_backorder_flag).ToList();
                                p.UKOrderComments = dbContext.UKOrderTrackerComments.Where(c => c.OrderNumber == p.po_order_no).ToList();

                            }
                            );
                            break;
                    }
                }
            }
            else
            {
                if (country.Equals("AU"))
                {
                    auPurchaseOrders.ForEach(p =>
                    {
                        p.OrderLines = dbContext.VAUPurchaseOrderLines.Where(l => l.po_order_no == p.po_order_no && l.po_backorder_flag == p.po_backorder_flag && l.ValidPOL).ToList();
                        p.OrderComments = dbContext.AUOrderTrackerComments.Where(c => c.OrderNumber == p.po_order_no).ToList();

                    }
                    );
                }
                if (country.Equals("UK"))
                {
                    ukPurchaseOrders.ForEach(p =>
                    {
                        p.UKOrderLines = dbContext.VUKPurchaseOrderLines.Where(l => l.po_order_no == p.po_order_no && l.po_backorder_flag == p.po_backorder_flag && l.ValidPOL).ToList();
                        p.UKOrderComments = dbContext.UKOrderTrackerComments.Where(c => c.OrderNumber == p.po_order_no).ToList();

                    }
                    );
                }
            }
            if (country.Equals("UK"))
            {
               return PartialView("_UKIndex", ukPurchaseOrders);
            }
             else
            {
                return PartialView("_test", auPurchaseOrders);
            }
           
            //return RedirectToAction("AUIndex", auPurchaseOrders);
        }
        public ActionResult POReport()
        {
            return Redirect("~/Reports/ViewReport.aspx");
        }

        public ActionResult AddComment(string id)
        {
            List<AspNetRoles> RoleName = new List<AspNetRoles>();
            var dbContext = new POTrackerDBEntities();
            RoleName = dbContext.AspNetRoles.ToList();

            RoleName.ForEach(r =>
            {
                var UserIds = dbContext.AspNetUserRoles.Where(ru => ru.RoleId == r.Id).Select(ru => ru.UserId).ToList();

                r.Users = dbContext.AspNetUsers.Where(u => UserIds.Contains(u.Id)).ToList();
            }
            );
            string data=null;
            
            data = "<input type='text' value='test' name='test'><input type='button' name='submit'value='submit'>";
            return Json(data, JsonRequestBehavior.AllowGet); 
        }

        public ActionResult RefreshPO()
        {
            try
            {
                var dbContext = new POTrackerDBEntities();
                dbContext.RefreshPurchaseOrderCache("AU");
                var context = ((System.Data.Entity.Infrastructure.IObjectContextAdapter)dbContext).ObjectContext;
                var refreshableObjects = (from entry in context.ObjectStateManager.GetObjectStateEntries(
                                                           System.Data.Entity.EntityState.Added
                                                           | System.Data.Entity.EntityState.Deleted
                                                           | System.Data.Entity.EntityState.Modified
                                                           | System.Data.Entity.EntityState.Unchanged)
                                          where entry.EntityKey != null
                                          select entry.Entity).ToList();

                context.Refresh(System.Data.Entity.Core.Objects.RefreshMode.StoreWins, refreshableObjects);
                var auPurchaseOrders = GetRoleBasedPO();
                return PartialView("_test", auPurchaseOrders);
            }
            catch
            {
                return Json(false);

            }
        }

        [HttpPost]
        public ActionResult DeleteComment(string id)
        {
            var comtId = Convert.ToInt32(id);
            var dbContext = new POTrackerDBEntities();
            var removeRecord = dbContext.AUOrderTrackerComments.SingleOrDefault(r => r.CommentsId == comtId);
            var result = false;
            if(removeRecord != null)
            {
                try
                {
                    dbContext.AUOrderTrackerComments.Remove(removeRecord);
                    dbContext.SaveChanges();
                    result = true;
                }
                catch
                {

                }

            }
            return Json(result);
        }
        [HttpPost]
        [AuthLog(Roles = "Admin,PurchasingAdmin")]
        public ActionResult ApprovePO(int po,string poSuffix)
        {
            var result = false;
            if (User.IsInRole("Admin") || User.IsInRole("PurchasingAdmin"))
            {
                var dbContext = new POTrackerDBEntities();
                var poApproval = dbContext.POApprovals.Where(p => p.PONum == po && p.POSuffix == poSuffix).FirstOrDefault();
                if (poApproval == null)
                {
                    try
                    {
                        poApproval = new POApproval
                        {
                            PONum = po,
                            POSuffix = poSuffix,
                            CountryCode = "AU",
                            ApprovedOn = DateTime.Now,
                            ApprovedBy = User.Identity.Name
                        };
                        dbContext.POApprovals.Add(poApproval);
                        dbContext.SaveChanges();
                        result = true;
                    }
                    catch
                    {

                    }
                }
            }
            return Json(result);
        }

        public String GetSearchValues(string id, string country)
        {
            var searchBy = id;
            var dbContext = new POTrackerDBEntities();
            List<string> ListOfSupplier = new List<string>();
            List<String> li = new List<String>();
            List<AUPurchaseOrder> auPurchaseOrders = new List<AUPurchaseOrder>();
            List<UKPurchaseOrder> ukPurchaseOrders = new List<UKPurchaseOrder>();

            if (country == "AU")
            {
                 auPurchaseOrders = GetRoleBasedPO();
                switch (searchBy)
                {
                    case "Stratum":
                        li = auPurchaseOrders.Select(p => p.OrderLines).SelectMany(l => l).GroupBy(l => l.stock_group.Trim()).Select(l => l.Key).ToList();
                        break;
                    case "Purchase Order":
                        li = auPurchaseOrders.GroupBy(p => p.po_order_no).Select(l => l.Key.ToString()).ToList();
                        break;
                    case "User":
                        li = auPurchaseOrders.GroupBy(p => p.po_user_name.Trim()).Select(l => l.Key).ToList();
                        break;
                    case "Supplier":
                        li = auPurchaseOrders.GroupBy(p => p.cre_accountcode.Trim()).Select(l => l.Key).ToList();
                        break;
                    case "Whse Code":
                        li = auPurchaseOrders.GroupBy(p => p.po_whse_code.Trim()).Select(l => l.Key).ToList();
                        break;
                }
            }
            else
            {
                 ukPurchaseOrders = UKGetRoleBasedPO();
                switch (searchBy)
                {
                    case "Stratum":
                        li = ukPurchaseOrders.Select(p => p.UKOrderLines).SelectMany(l => l).GroupBy(l => l.stock_group.Trim()).Select(l => l.Key).ToList();
                        break;
                    case "Purchase Order":
                        li = ukPurchaseOrders.GroupBy(p => p.po_order_no).Select(l => l.Key.ToString()).ToList();
                        break;
                    case "User":
                        li = ukPurchaseOrders.GroupBy(p => p.po_user_name.Trim()).Select(l => l.Key).ToList();
                        break;
                    case "Supplier":
                        li = ukPurchaseOrders.GroupBy(p => p.cre_accountcode.Trim()).Select(l => l.Key).ToList();
                        break;
                    case "Whse Code":
                        li = ukPurchaseOrders.GroupBy(p => p.po_whse_code.Trim()).Select(l => l.Key).ToList();
                        break;
                }
            }

            
            
            
            var searchValues = li.Select(l => String.Format("<option value='{0}'>{0}</option>", l)).ToList();
            var htmlData = String.Join(Environment.NewLine, searchValues.ToArray());
            ViewBag.SearchInputHtml = htmlData;
            return htmlData;
        }

        private List<String> GetArtworkProducts()
        {
            var serverDirs = new List<String>();
            var parentPath = @"\\192.1.1.15\a r c h i v e\ZFTP_Final Art";
            try
            {
                
                serverDirs = System.IO.Directory.EnumerateDirectories(parentPath).Where(d=>d.Contains(" Final Art")).Select(f=>System.IO.Path.GetFileName(f).Replace(" Final Art","")).ToList();
            }
            catch
            {
                using (UNCAccessWithCredentials unc = new UNCAccessWithCredentials())
                {
                    if (unc.NetUseWithCredentials(parentPath, "design", "arlec", "apple"))
                    {
                        serverDirs = System.IO.Directory.GetDirectories(parentPath).Where(d => d.Contains(" Final Art")).Select(f => System.IO.Path.GetFileName(f).Replace(" Final Art", "")).ToList();
                    }
                }
            }
            return serverDirs;
        }

        private List<AUPurchaseOrder> GetRoleBasedPO()
        {
            var dbContext = new POTrackerDBEntities();
            var loginUser = User.Identity.GetUserName();
            List<string> ListOfSupplier = new List<string>();
            List<AUPurchaseOrder> auPurchaseOrders = new List<AUPurchaseOrder>();

           

                if (!User.IsInRole("Admin") && !User.IsInRole("Supplier") && !User.IsInRole("Staff") && !User.IsInRole("Sales"))
                {
                    ListOfSupplier = dbContext.SupplierInspectionRelation.Where(I => I.InspectionTeam.Contains(loginUser)).Select(s => s.SupplierCode).ToList();
                    auPurchaseOrders = dbContext.AUPurchaseOrders.Where(p => p.ValidPO && ListOfSupplier.Contains(p.cre_accountcode)).ToList();
                    if (auPurchaseOrders.Count == 0)
                    {
                        ModelState.AddModelError("Error", "No Record Found Under Your Login");
                    }
                }
                if (User.IsInRole("Supplier"))
                {
                    var supplierId = User.Identity.GetUserId();

                    ListOfSupplier = dbContext.ManageSupplierLogin.Where(I => I.SupplierLoginId.Contains(supplierId)).Select(s => s.Suppliercode).ToList();
                    //auPurchaseOrders = dbContext.AUPurchaseOrders.Where(p => p.ValidPO && ListOfSupplier.Contains(p.cre_accountcode)).ToList();
                    auPurchaseOrders = dbContext.AUPurchaseOrders.Join(dbContext.POApprovals, p => p.po_order_no, a => a.PONum, (p, a) => p)
                        .Where(p => p.ValidPO && ListOfSupplier.Contains(p.cre_accountcode)).ToList();
                    if (auPurchaseOrders.Count == 0)
                    {
                        ModelState.AddModelError("Error", "No Record Found Under Your Login");
                    }
                }
                else if (User.IsInRole("Admin") || User.IsInRole("PurchasingAdmin") || User.IsInRole("Sales"))
                {
                    auPurchaseOrders = dbContext.AUPurchaseOrders.Where(p => p.ValidPO).ToList();
                    if (auPurchaseOrders.Count == 0)
                    {
                        ModelState.AddModelError("Error", "No Record Found Under Your Login");
                    }
                }
                // var auPurchaseOrders = dbContext.AUPurchaseOrders.Where(p => p.ValidPO).ToList();
                if (auPurchaseOrders.Count > 0)
                {
                    auPurchaseOrders.ForEach(p =>
                    {
                        p.OrderLines = dbContext.VAUPurchaseOrderLines.Where(l => l.po_order_no == p.po_order_no && l.po_backorder_flag == p.po_backorder_flag && l.ValidPOL);
                        p.OrderComments = dbContext.AUOrderTrackerComments.Where(c => c.OrderNumber == p.po_order_no);
                        p.Approval = dbContext.POApprovals.Where(a => a.PONum == p.po_order_no && a.POSuffix == p.po_backorder_flag).FirstOrDefault();
                    }
                    );
                    Session["ArtworkProducts"] = GetArtworkProducts();
                }
                return auPurchaseOrders;
            
           
        }

        private List<UKPurchaseOrder> UKGetRoleBasedPO()
        {
            var dbContext = new POTrackerDBEntities();
            var loginUser = User.Identity.GetUserName();
            List<string> ListOfSupplier = new List<string>();
            List<UKPurchaseOrder> ukPurchaseOrders = new List<UKPurchaseOrder>();
            if (!User.IsInRole("Admin") && !User.IsInRole("Supplier") && !User.IsInRole("Staff") && !User.IsInRole("Sales"))
            {
                ListOfSupplier = dbContext.SupplierInspectionRelation.Where(I => I.InspectionTeam.Contains(loginUser)).Select(s => s.SupplierCode).ToList();
                ukPurchaseOrders = dbContext.UKPurchaseOrders.Where(p => p.ValidPO && ListOfSupplier.Contains(p.cre_accountcode)).ToList();
                if (ukPurchaseOrders.Count == 0)
                {
                    ModelState.AddModelError("Error", "No Record Found Under Your Login");
                }
            }
            if (User.IsInRole("Supplier"))
            {
                var supplierId = User.Identity.GetUserId();

                ListOfSupplier = dbContext.ManageSupplierLogin.Where(I => I.SupplierLoginId.Contains(supplierId)).Select(s => s.Suppliercode).ToList();
                //auPurchaseOrders = dbContext.AUPurchaseOrders.Where(p => p.ValidPO && ListOfSupplier.Contains(p.cre_accountcode)).ToList();
                ukPurchaseOrders = dbContext.UKPurchaseOrders.Join(dbContext.POApprovals, p => p.po_order_no, a => a.PONum, (p, a) => p)
                    .Where(p => p.ValidPO && ListOfSupplier.Contains(p.cre_accountcode)).ToList();
                if (ukPurchaseOrders.Count == 0)
                {
                    ModelState.AddModelError("Error", "No Record Found Under Your Login");
                }
            }
            else if (User.IsInRole("Admin") || User.IsInRole("PurchasingAdmin") || User.IsInRole("Sales"))
            {
                ukPurchaseOrders = dbContext.UKPurchaseOrders.Where(p => p.ValidPO).ToList();
                if (ukPurchaseOrders.Count == 0)
                {
                    ModelState.AddModelError("Error", "No Record Found Under Your Login");
                }
            }
            // var auPurchaseOrders = dbContext.AUPurchaseOrders.Where(p => p.ValidPO).ToList();
            if (ukPurchaseOrders.Count > 0)
            {
                ukPurchaseOrders.ForEach(p =>
                {
                    p.UKOrderLines = dbContext.VUKPurchaseOrderLines.Where(l => l.po_order_no == p.po_order_no && l.po_backorder_flag == p.po_backorder_flag && l.ValidPOL);
                    p.UKOrderComments = dbContext.UKOrderTrackerComments.Where(c => c.OrderNumber == p.po_order_no);
                    p.Approval = dbContext.POApprovals.Where(a => a.PONum == p.po_order_no && a.POSuffix == p.po_backorder_flag).FirstOrDefault();
                }
                );
                Session["ArtworkProducts"] = GetArtworkProducts();
            }
            return ukPurchaseOrders;
        }
        /*  
        [Authorize]
        [HttpGet]
        public ActionResult Index()
        {
            //var data = new DataTable();
            //var comments = ProntoDatabase.FetchComments();
            //var Model  = new AUPOTrackers();
            //     data = ProntoDatabase.GetOrderswithLines(null, null);
            //    var auTrackers = from x in data.AsEnumerable()
            //                     group x by new
            //                     {
            //                         po_order_no = x.Field<String>("po_order_no"),
            //                         cre_accountcode = x.Field<String>("cre_accountcode"),
            //                         po_order_date = x.Field<string>("po_order_date"),
            //                         po_notes = x.Field<String>("po_notes"),
            //                         po_whse_code = x.Field<String>("po_whse_code")
            //                     } into grp
            //                     select new AUPOtrackerHeader { PO_order_no = grp.Key.po_order_no, Supplier = grp.Key.cre_accountcode, OrderDate = grp.Key.po_order_date, Note_oldPONo = grp.Key.po_notes, Whse_code = grp.Key.po_whse_code };

            //    Model.AUPOtrackerHeaders = auTrackers.ToList();
            //    Model.AUPOtrackerLines = data.AsEnumerable().Select(r => new AUPOtrackerLines(r)).ToList();
            //    Model.AUPOtrackerComments = comments.AsEnumerable().Select(c => new AUPOtrackerComments(c)).ToList();


            //return View(Model);
            return View();
        }
        [HttpPost]
        public ActionResult Index(AUPOTrackers Model)
        {
            var data = new DataTable();
            var comments = ProntoDatabase.FetchComments();
            data = ProntoDatabase.GetOrderswithLines(null, null);
            var auTrackers = from x in data.AsEnumerable()
                             group x by new
                             {
                                 po_order_no = x.Field<String>("po_order_no"),
                                 cre_accountcode = x.Field<String>("cre_accountcode"),
                                 po_order_date = x.Field<string>("po_order_date"),
                                 po_notes = x.Field<String>("po_notes"),
                                 po_whse_code = x.Field<String>("po_whse_code")
                             } into grp
                             select new AUPOtrackerHeader { PO_order_no = grp.Key.po_order_no, Supplier = grp.Key.cre_accountcode, OrderDate = grp.Key.po_order_date, Note_oldPONo = grp.Key.po_notes, Whse_code = grp.Key.po_whse_code };

            if(Model.ordertype == null && Model.PONumber == null ) {
                Model.AUPOtrackerHeaders = auTrackers.ToList();

            }
            if (Model.ordertype != null || TempData["ordertype"] ==null)                
            {
                if (Model.ordertype == "otherIssues") {
                    Model.AUPOtrackerHeaders = auTrackers.Where(o => o.OtherIssue==1).ToList();

                }
                else {
                    Model.AUPOtrackerHeaders = auTrackers.Where(o => o.SpecialOrder ==1).ToList();
                }

                TempData["ordertype"] = Model.ordertype;

            }
            if(Model.PONumber != null ||   TempData["PONumber"] != null)
            {
                Model.AUPOtrackerHeaders = auTrackers.Where(o => o.PO_order_no == Model.PONumber).ToList();

            }
            Model.AUPOtrackerLines = data.AsEnumerable().Select(r => new AUPOtrackerLines(r)).ToList();
            Model.AUPOtrackerComments = comments.AsEnumerable().Select(c => new AUPOtrackerComments(c)).ToList();


            return View(Model);
        }

        public ActionResult DeleteNoteByID(int ID)
        {
            var po_num = ProntoDatabase.GetPOByCommentsID(ID);
            var comments = ProntoDatabase.GetComments(po_num);
            var result = ProntoDatabase.DeleteNoteByID(ID);
            var Model = comments.AsEnumerable().Select(c => new AUPOtrackerComments(c)).ToList();
            ViewData["po"] = po_num;
            return PartialView("Comments",Model);
        }

        */


        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}