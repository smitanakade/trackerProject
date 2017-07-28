using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading;
using Microsoft.Office.Interop.Word;
using System.IO;

public static class PDFConvertionClass
{
    public static bool ConvertDocument(string sourceDocPath, string targetFilePath, Microsoft.Office.Interop.Word.WdExportFormat targetFormat, string username = "")
    {
        var converted = false;

        object paramMissing = Type.Missing;
        object paramSourceDocPath = sourceDocPath;
        var paramExportFilePath = targetFilePath;

        var paramOpenAfterExport = false;
        var paramDocStructureTags = true;
        var paramBitmapMissingFonts = true;
        var paramUseISO19005_1 = false;
        object readOnly = true;
        object isVisible = false;

        var paramStartPage = 0;
        var paramEndPage = 0;

        var paramIncludeDocProps = true;
        var paramKeepIRM = true;

        Microsoft.Office.Interop.Word.Application wordApplication = new Microsoft.Office.Interop.Word.Application();
       
        Microsoft.Office.Interop.Word.Documents wordDocuments = wordApplication.Documents;
        Microsoft.Office.Interop.Word.Document wordDocument = null;

        Microsoft.Office.Interop.Word.WdExportOptimizeFor paramExportOptimizeFor = Microsoft.Office.Interop.Word.WdExportOptimizeFor.wdExportOptimizeForOnScreen;
        Microsoft.Office.Interop.Word.WdExportRange paramExportRange = Microsoft.Office.Interop.Word.WdExportRange.wdExportAllDocument;
       
        Microsoft.Office.Interop.Word.WdExportItem paramExportItem = Microsoft.Office.Interop.Word.WdExportItem.wdExportDocumentContent;
      
        Microsoft.Office.Interop.Word.WdExportCreateBookmarks paramCreateBookmarks = Microsoft.Office.Interop.Word.WdExportCreateBookmarks.wdExportCreateWordBookmarks;
        
        try
        {
            // Open the source document.
            wordDocument = wordApplication.Documents.Open(ref paramSourceDocPath, ref paramMissing, ref readOnly, ref paramMissing,
            ref paramMissing, ref paramMissing, ref paramMissing, ref paramMissing, ref paramMissing, ref paramMissing,
            ref paramMissing, ref isVisible, ref paramMissing, ref paramMissing, ref paramMissing, ref paramMissing);

            // Export it in the specified format.
            if (wordDocument != null)
            {
                wordDocument.PageSetup.PaperSize = Microsoft.Office.Interop.Word.WdPaperSize.wdPaperA4;
                wordDocument.ExportAsFixedFormat(paramExportFilePath, targetFormat, paramOpenAfterExport, paramExportOptimizeFor,
                paramExportRange, paramStartPage, paramEndPage, paramExportItem, paramIncludeDocProps, paramKeepIRM, paramCreateBookmarks, paramDocStructureTags,
                paramBitmapMissingFonts, paramUseISO19005_1, ref paramMissing);
                converted = true;            
			 }
        }
        catch (Exception ex)
        {
            //poEmailClass.sendErrorEmail(ex,username);
            //errMsg.Text = ex.ToString();
        }
        finally
        {
            object doNotSaveChanges = Microsoft.Office.Interop.Word.WdSaveOptions.wdDoNotSaveChanges;
            ((Microsoft.Office.Interop.Word._Document)wordDocument).Close(ref doNotSaveChanges, ref paramMissing, ref paramMissing);
            System.Runtime.InteropServices.Marshal.FinalReleaseComObject(wordDocument);
            System.Runtime.InteropServices.Marshal.FinalReleaseComObject(wordDocuments);
            
            ((Microsoft.Office.Interop.Word._Application)wordApplication).Quit(ref paramMissing, ref paramMissing, ref paramMissing);
            System.Runtime.InteropServices.Marshal.FinalReleaseComObject(wordApplication); //Added to release the WINWORD.EXE Process.
            wordApplication = null;
	        wordDocument = null;
            /*
            if (wordDocument != null)
            {
                //Microsoft.Office.Interop.Word._Document.Close(ref paramMissing, ref paramMissing, ref paramMissing);
                wordDocument.Close(ref paramMissing, ref paramMissing, ref paramMissing);
                wordDocument = null;
            }
            */
            //GC.Collect();
            //GC.WaitForPendingFinalizers(); //Commented to prevent deadlocks in rare circumstances.
        }
        return converted;
    }
}
