using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace POTracker.Models
{
    public class AUPOtrackerHeader
    {
        public AUPOtrackerHeader(DataRow row)
        {
            PO_order_no = row.Field<string>("po_order_no");
            Supplier = row.Field<string>("cre_accountcode");
            OrderDate = row.Field<string>("po_order_date");
            Note_oldPONo = row.Field<string>("po_notes");
            revision_No = row.Field<string>("po_revision_no");
            Whse_code = row.Field<string>("po_whse_code");
            OtherIssue = row.Field<int>("otherIssue");
            SpecialOrder = row.Field<int>("SpecialOrder");
        }
        public AUPOtrackerHeader()
        { }
        public string PO_order_no { get; set; }

        public string Supplier { get; set; }

        public string OrderDate { get; set; }

        public string Note_oldPONo { get; set; }

        public string revision_No { get; set; }


        public string Whse_code { get; set; }

        public string pdf { get; set; }

        public string Comment { get; set; }
        public int OtherIssue { get; set; }
        public int SpecialOrder { get; set; }

    }
    public class AUPOtrackerLines
    {
        public AUPOtrackerLines(DataRow row)
        {
            PO_order_no = row.Field<string>("po_order_no");
            Supplier = row.Field<string>("cre_accountcode");
            OrderDate = row.Field<string>("po_order_date");
            Note_oldPONo = row.Field<string>("po_notes");
            revision_No = row.Field<string>("po_revision_no");
            Whse_code = row.Field<string>("po_whse_code");
            ProductCode = row.Field<string>("stock_code");
            OS_Qty = row.Field<int>("po_order_qty");
            ActionDate = row.Field<string>("pol_user_only_date1");
            LineSeq = row.Field<string>("po_l_seq");
            Flag = row.Field<string>("po_backorder_flag");
            SupplierStatus = row.Field<string>("SupplierStatus");


        }
        public AUPOtrackerLines()
        { }
        public string PO_order_no { get; set; }

        public string Supplier { get; set; }

        public string OrderDate { get; set; }

        public string Note_oldPONo { get; set; }

        public string revision_No { get; set; }

        public string ProductCode { get; set; }

        public int OS_Qty { get; set; }

        public string ActionDate { get; set; }

        public string SupplierStatus { get; set; }

        public string LineSeq { get; set; }
        public string Whse_code { get; set; }
        
        public string pdf { get; set; }


        public string Flag { get; set; }

  
        public string Comment { get; set; }

       
    }

    public class AUPOtrackerComments
    {

        public AUPOtrackerComments(DataRow row)
        {
            user = row.Field<string>("username");
            comment = row.Field<string>("note");
            commentId = row.Field<int>("id");
            date = row.Field<DateTime>("date");
            OrderNumber = row.Field<string>("ordernumber");
        }
        public string user { get; set; }
        public string comment { get; set; }
        public int commentId { get; set; }
        public DateTime date { get; set; }
        public string OrderNumber { get; set; }
    }

    public class AUPOTrackers
    {
        public string ordertype { get; set; }
        public string PONumber { get; set; }
        public List<AUPOtrackerHeader> AUPOtrackerHeaders { get; set; }
        public List<AUPOtrackerLines> AUPOtrackerLines { get; set; }
        public List<AUPOtrackerComments> AUPOtrackerComments { get; set; }
    }
   
         
}