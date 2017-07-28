using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace POTracker.Models
{
    public class SupplierManageModel
    {
        public int SupplierId { get; set; }
        public string SupplierName { get; set; }
        public string Email { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedOn { get; set; }
        public string LoginId { get; set; }
        public string SupplierStatus { get; set; }
        public string Designation { get; set; }
        public string Manage { get; set; }

        public string Country { get; set; }

        public string Register { get; set; }
       
        public IEnumerable<SelectListItem> IsRegister { get; set; }
        public IEnumerable<SelectListItem> SupplierTitle { get; set; }
        public IEnumerable<SelectListItem> ListOfSupplier { get; set; }

        public IEnumerable<SelectListItem> StatusOfSupplier { get; set; }

      
        public List<SupplierManageModel> SupplierDetails { get; set; }

    }
   public class ShowSupplier
    {
        public string suppliercode { get; set; }
        public string suppliierName { get; set; }
        public int Regester { get; set; }
        public string country { get; set; }
        public string Inspector { get; set; }
        public Boolean isChecked { get; set; }
        public List<SelectListItem> CountryList { get; set; }
        public List<ShowSupplier> ShowSuppList { get; set; }
        public IEnumerable<SelectListItem> ListofInspectionTeam { get; set; }
    }

 

}