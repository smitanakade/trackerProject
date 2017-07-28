using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Web;

namespace POTracker.Models
{
    public partial class AUPurchaseOrder
    {
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public Nullable<System.DateTime> OrderDate
        {
            get
            {
                return po_order_date;
            }
        }
        public virtual POApproval Approval { get; set; }

        public virtual IEnumerable<VAUPurchaseOrderLine> OrderLines { get; set; }
        public virtual IEnumerable<AUOrderTrackerComment> OrderComments { get; set; }

        public virtual IEnumerable<VUKPurchaseOrderLine> UKOrderLines { get; set; }
        public virtual IEnumerable<UKOrderTrackerComment> UKOrderComments { get; set; }


        //public String ChatComment
        //{
        //    get
        //    {
        //        var result = "";
        //        if(OrderComments.Count>0)
        //        {
        //            result= String.Join(Environment.NewLine, OrderComments.OrderBy(o=>o.CommentDate).Select(c => c.UserName + ": " + c.Comment +Environment.NewLine+ " on " + c.CommentDate.ToString("dd/MM/yyyy h:mm:ss tt")));
        //        }
        //        return result;
        //    }

        //}
    }
    public partial class VAUPurchaseOrderLine
    {
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public Nullable<System.DateTime> ActionDate
        {
            get
            {
                return pol_user_only_date1;
            }
        }

    }
    public partial class NZPurchaseOrder
    {
        public List<VNZPurchaseOrderLine> OrderLines { get; set; }
        public List<NZOrderTrackerComment> OrderComments { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public Nullable<System.DateTime> OrderDate
        {
            get
            {
                return po_order_date;
            }
        }
        
        //public String ChatComment
        //{
        //    get
        //    {
        //        var result = "";
        //        if (OrderComments.Count > 0)
        //        {
        //            result = String.Join(Environment.NewLine, OrderComments.OrderBy(o => o.CommentDate).Select(c => c.UserName + ": " + c.Comment + " on " + c.CommentDate.ToString("dd-MM-yyyy")));
        //        }
        //        return result;
        //    }

        //}
    }
    public partial class VNZPurchaseOrderLine
    {
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public Nullable<System.DateTime> ActionDate
        {
            get
            {
                return pol_user_only_date1;
            }
        }

    }
    public partial class UKPurchaseOrder
    {
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public Nullable<System.DateTime> OrderDate
        {
            get
            {
                return po_order_date;
            }
        }
        public virtual POApproval Approval { get; set; }

            public virtual IEnumerable<VUKPurchaseOrderLine> UKOrderLines { get; set; }
        public virtual IEnumerable<UKOrderTrackerComment> UKOrderComments { get; set; }


        //public String ChatComment
        //{
        //    get
        //    {
        //        var result = "";
        //        if (OrderComments.Count > 0)
        //        {
        //            result = String.Join(Environment.NewLine, OrderComments.OrderBy(o => o.CommentDate).Select(c => c.UserName + ": " + c.Comment + " on " + c.CommentDate.ToString("dd-MM-yyyy")));
        //        }
        //        return result;
        //    }

        //}
    }
    public partial class VUKPurchaseOrderLine
    {
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public Nullable<System.DateTime> ActionDate
        {
            get
            {
                return pol_user_only_date1;
            }
        }

    }
    public partial class AspNetRoles
    {
        public List<AspNetUsers>Users { get; set; }
        public List<AspNetUserRoles> UserInRole { get; set; }
    }
}