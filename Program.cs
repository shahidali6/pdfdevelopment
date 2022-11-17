using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ConsoleApp15PDFMerge
{
    class Program
    {
        static void Main(string[] args)
        {
           var listof =  Directory.GetFiles(@"C:\Users\msaddique\Downloads\PDF emails folders");
            MargeMultiplePDF(listof, "1.pdf");

        }
        public static void MargeMultiplePDF(string[] PDFfileNames, string OutputFile)
        {
            int loopCounter = 0;
            // Create document object  
            iTextSharp.text.Document PDFdoc = new iTextSharp.text.Document();
            // Create a object of FileStream which will be disposed at the end  
            using (System.IO.FileStream MyFileStream = new System.IO.FileStream(OutputFile+loopCounter+".pdf", System.IO.FileMode.Create))
            {
                // Create a PDFwriter that is listens to the Pdf document  
                iTextSharp.text.pdf.PdfCopy PDFwriter = new iTextSharp.text.pdf.PdfCopy(PDFdoc, MyFileStream);
                if (PDFwriter == null)
                {
                    return;
                }
                // Open the PDFdocument  
                PDFdoc.Open();
                foreach (string fileName in PDFfileNames)
                {
                    // Create a PDFreader for a certain PDFdocument  
                    iTextSharp.text.pdf.PdfReader PDFreader = new iTextSharp.text.pdf.PdfReader(fileName);
                    PDFreader.ConsolidateNamedDestinations();
                    // Add content  
                    for (int i = 1; i <= PDFreader.NumberOfPages; i++)
                    {
                        try
                        {
                            iTextSharp.text.pdf.PdfImportedPage page = PDFwriter.GetImportedPage(PDFreader, i);
                            PDFwriter.AddPage(page);
                        }
                        catch (Exception ex)
                        {
                            File.AppendAllText("Errors.txt", "PDF Page addition Failed" + fileName + "@" + ex.Message + "@" + Environment.NewLine);
                        }
                    }
                    iTextSharp.text.pdf.PRAcroForm form = PDFreader.AcroForm;
                    if (form != null)
                    {

                        try
                        {
                            PDFwriter.AddDocument(PDFreader);
                        }
                        catch (Exception ex)
                        {
                            File.AppendAllText("Errors.txt", "PDF Writing Failed"+fileName + "@" + ex.Message + "@"+Environment.NewLine);
                            loopCounter++;
                            //MessageBox.Show(ex.Message);
                        }
                    }
                    // Close PDFreader  
                    PDFreader.Close();
                }
                // Close the PDFdocument and PDFwriter  
                PDFwriter.Close();
                PDFdoc.Close();
            }// Disposes the Object of FileStream  
        }

        //private Loasdfsadfsaf ()
        //{
        //    try
        //    {
        //        string FPath = "";
        //        // Create For loop for get/create muliple report on single click based on row of gridview control  
        //        for (int j = 0; j < Gridview1.Rows.Count; j++)
        //        {
        //            // Return datatable for data  
        //            DataTable dtDetail = new My_GlobalClass().GetDataTable(Convert.ToInt32(Gridview1.Rows[0]["JobId"]));

        //            int i = Convert.ToInt32(Gridview1.Rows[0]["JobId"]);
        //            if (dtDetail.Rows.Count > 0)
        //            {
        //                // Create Object of ReportDocument  
        //                ReportDocument cryRpt = new ReportDocument();
        //                //Store path of .rpt file  
        //                string StrPath = Application.StartupPath + "\\RPT";
        //                StrPath = StrPath + "\\";
        //                StrPath = StrPath + "rptCodingvila_Articles_Report.rpt";
        //                cryRpt.Load(StrPath);
        //                // Assign Report Datasource  
        //                cryRpt.SetDataSource(dtDetail);
        //                // Assign Reportsource to Report viewer  
        //                CryViewer.ReportSource = cryRpt;
        //                CryViewer.Refresh();
        //                // Store path/name of pdf file one by one   
        //                string StrPathN = Application.StartupPath + "\\Temp" + "\\Codingvila_Articles_Report" + i.ToString() + ".Pdf";
        //                FPath = FPath == "" ? StrPathN : FPath + "," + StrPathN;
        //                // Export Report in PDF  
        //                cryRpt.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, StrPathN);
        //            }
        //        }
        //        if (FPath != "")
        //        {
        //            // Check for File Existing or Not  
        //            if (System.IO.File.Exists(Application.StartupPath + "\\Temp" + "\\Codingvila_Articles_Report.pdf"))
        //                System.IO.File.Delete(Application.StartupPath + "\\Temp" + "\\Codingvila_Articles_Report.pdf");
        //            // Split and store pdf input file  
        //            string[] files = FPath.Split(',');
        //            //  Marge Multiple PDF File  
        //            MargeMultiplePDF(files, Application.StartupPath + "\\Temp" + "\\Codingvila_Articles_Report.pdf");
        //            // Open Created/Marged PDF Output File  
        //            Process.Start(Application.StartupPath + "\\Temp" + "\\Codingvila_Articles_Report.pdf");
        //            // Check and Delete Input file  
        //            foreach (string item in files)
        //            {
        //                if (System.IO.File.Exists(item.ToString()))
        //                    System.IO.File.Delete(item.ToString());
        //            }

        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        XtraMessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //    }
        //}
    }

}
