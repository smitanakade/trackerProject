using POTracker.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace POTracker.Reports
{
    public partial class ViewSpecifications : System.Web.UI.Page
    {
        public static string _SpecFileDirectory =@"\\arlec.com.au\DFS\R&D\PRODEV\SPECS\ARLECSPC\";
        public static string _TempFolder; //@"C:\inetpub\wwwroot\NewSpex\tempFolder\";
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var product = Request.QueryString["p"];
                var supplier= Request.QueryString["s"];
                var dbcontext   = new POTrackerDBEntities();
                var specifications=dbcontext.VProductSpecifications.Where(s => s.ProductCode == product).ToList();
                if (specifications.Count > 0)
                {
                    var supplierSpecs = new List<VProductSpecification>();
                    if (supplier != null)
                    {
                        supplierSpecs = specifications.Where(s => s.sid == supplier).ToList();
                    }
                    if (supplierSpecs.Count > 0)
                    {
                        foreach (var specification in supplierSpecs)
                        {
                            var specs = String.Format("{0}-{1}", specification.Specification_No, specification.Prod_Desc);
                            select1.Items.Add(new ListItem(specs, specification.Specification_No));
                        }
                    }
                    else
                    {
                        foreach (var specification in specifications)
                        {

                            var specs = String.Format("{0}-{1}", specification.Specification_No, specification.Prod_Desc);
                            select1.Items.Add(new ListItem(specs, specification.Specification_No));
                        }
                    }

                    select1.SelectedIndex = 0;
                    ViewSpecs();

                }
                else
                {
                    iframe1.Attributes["src"] = "NoPDF.aspx";
                }

            }
        }

        protected void Submit_Click(object sender, EventArgs e)
        {
            ViewSpecs();
        }
        private void ViewSpecs()
        {
            var spec = select1.Items[select1.SelectedIndex].Value;
            _TempFolder = Server.MapPath("~/Temp/");
            var filesToDelete= Directory.GetFiles(_TempFolder, "*.pdf");
            foreach (var file in filesToDelete)
            {
                try
                {
                    File.Delete(file);
                }
                catch
                { }
            }
            var docUrl = String.Format("{0}{1}.doc", _SpecFileDirectory , spec );
            if (File.Exists(docUrl))
            {
                var pdf_file = String.Format("{0}{1}{2}.pdf", _TempFolder, spec, DateTime.Now.ToString("ddMMyyyyHHmmssttt"));
                var currenturl = String.Format("~/Temp/{0}", Path.GetFileName(pdf_file));
                if (PDFConvertionClass.ConvertDocument(docUrl, pdf_file, Microsoft.Office.Interop.Word.WdExportFormat.wdExportFormatPDF))
                {

                    iframe1.Attributes["src"] = currenturl;
                    Label1.Text = select1.Items[select1.SelectedIndex].Text;
                }
            }
            else
            {
                iframe1.Attributes["src"] = "NoPDF.aspx";
                Label1.Text = "";
            }
        }
    }
}