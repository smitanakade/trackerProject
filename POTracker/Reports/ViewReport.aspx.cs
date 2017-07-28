using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace POTracker.Views.Shared
{
    public partial class ViewReport : System.Web.Mvc.ViewPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var poNumber = Request.QueryString["po"];
                if (!string.IsNullOrEmpty(poNumber))
                {
                    var suffix = String.Format("{0}", Request.QueryString["boSuffix"] ?? " ");
                    suffix = String.IsNullOrEmpty(suffix) ? " " : suffix;
                    // Set the processing mode for the ReportViewer to Remote  
                    ReportViewer1.ProcessingMode = ProcessingMode.Remote;

                    ServerReport serverReport = ReportViewer1.ServerReport;

                    // Set the report server URL and report path  
                    serverReport.ReportServerUrl =
                        new Uri("http://AAWS-SQL03/reportserver");
                    serverReport.ReportPath =
                        "/PO Tracker/PurchaseOrder";

                    // Create the sales order number report parameter  
                    var purchaseOrderNumber = new ReportParameter();
                    purchaseOrderNumber.Name = "OrderNo";
                    purchaseOrderNumber.Values.Add(poNumber.Trim());
                    var purchaseOrderBOSuffix = new ReportParameter();
                    purchaseOrderBOSuffix.Name = "BOFlag";
                    purchaseOrderBOSuffix.Values.Add(suffix.Trim());
                    // Set the report parameters for the report  
                    ReportViewer1.ShowParameterPrompts = false;
                    ReportViewer1.ServerReport.SetParameters(
                        new ReportParameter[] { purchaseOrderNumber, purchaseOrderBOSuffix });
                }


            }
        }
    }
}