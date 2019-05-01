using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using SmartFhirApplication.Models;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.Linq;
using System.Web;
using static SmartFhirApplication.Controllers.FHIRController;
using Microsoft.AspNetCore.Http;
using System.Text;

namespace SmartFhirApplication.Controllers
{
    public class HomeController : Microsoft.AspNetCore.Mvc.Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        // Don't worry about it
        public IActionResult Launch()
        {
      
            return View();
        }
        // Called when "Load Template" button is pressed, converts an html file into a view ActionResult for display by MVC system
        public IActionResult LoadTemplate(string path)
        {
            //ViewData["Msg"] = path;
            //return View("~/Views/Home/TemplateLauncher.cshtml");// + path + ".html");
            return View("~/Views/Templates/" + path + ".html");
        }

        // Test function used to produce a list of all templates in local filesystem. Might try to replace w/ an internet search if possible
        // Not currently in use
        public JsonResult FindFiles()
        {
            string workingDirectory = Environment.CurrentDirectory;
            DirectoryInfo templatesFolder = null;
            FileInfo[] files = null;
            string templatePath = workingDirectory + "/Views/Templates/";
            templatesFolder = new DirectoryInfo(templatePath);
            files = templatesFolder.GetFiles();
            var asdf = files.Select(f => f.Name);
            return Json(asdf);
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        // Function which gets called after report is filled in
        // Takes in a title, location, and nodes for each report section
        // Calls each desired action (PDF, XML, etc)
        public string Create(string title,
                            TemplateViewModel Procedure,
                            TemplateViewModel ClinicalInformation,
                            TemplateViewModel Comparison,
                            TemplateViewModel Findings,
                            TemplateViewModel Impression,
                            TemplateViewModel PatientInfo,
                            string Location)
        {
            //CreatePDF(title, Procedure, ClinicalInformation, Comparison, Findings, Impression, PatientInfo);
            var result = CreatePDF(title, Procedure, ClinicalInformation, Comparison, Findings, Impression, PatientInfo);
            string workingDirectory = Environment.CurrentDirectory;
            var bitArr = GetBinaryFile("results/" + title + " for Patient_01.pdf");
            //Make XMLdoc
            //createXML1(title);
            createXML1();
            CreateXMLtake2(title, Procedure, ClinicalInformation, Comparison, Findings, Impression, PatientInfo);
            //XMLController.ReportDocumentXML();

            return Convert.ToBase64String(bitArr); 
        }
        /*
        public void createXMLtry3()
        {
          //  xmlUrl Url = new xmlUrl();
          //  Url.Url = fileName.Text;
          //  List<xmlUrl> Urls = new List<xmlUrl>();
          //  Urls.Add(Url);
            XmlSerializer ser = new XmlSerializer(typeof(T));
            StringBuilder sb = new StringBuilder();
            StringWriter writer = new StringWriter(sb);
            ser.Serialize(writer, Urls);
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(sb.ToString());

            var fileName = DateTime.Now.ToString("ddmmyyyyhhss") + ".xml";

            doc.WriteTo(XmlWriter);
            XmlWriter.Flush();
            XmlWriter.Close();

            byte[] byteArray = stream.ToArray();

            Response.Clear();
           // Response.AppendHeader("Content-Disposition", "attachment; filename=" + lblFile.Text + ".xml");
            Response.Headers.Add("content-disposition", "attachment; filename=" + fileName);
            Response.Headers.Add("Content-Length", byteArray.Length.ToString());
            Response.ContentType = "text/xml";
            Response.ContentEncoding = Encoding.UTF8;
            Response.BinaryWrite(byteArray);
            Response.
        }  */

      //  public XmlDocument createXML1 (string title )
        public void createXML1 () { 
            //string docName = "D:\\SeniorDesign\\" +title + "testReport.xml"; 
           // var xmlDoc = new XmlDocument();
            XmlDocument xmlDoc = new XmlDocument();
           // XmlModel xmlModelData = new XmlModel(); //idea from https://stackoverflow.com/questions/20365143/generate-xml-file-using-data-in-asp-net-mvc-form 
            //Starting nodes 
            
            XmlNode ClinicalDocROOT = xmlDoc.CreateElement("ClinicalDocument");
            xmlDoc.AppendChild(ClinicalDocROOT);
            XmlNode ProcedureNode = xmlDoc.CreateElement("Procedure");
            ClinicalDocROOT.AppendChild(ProcedureNode);
            XmlNode comp1 = xmlDoc.CreateElement("component");
            XmlNode StructBody1 = xmlDoc.CreateElement("StructuredBody");
            XmlNode comp2 = xmlDoc.CreateElement("component");
            comp2.InnerText = "testing";
            StructBody1.AppendChild(comp2);

            comp1.AppendChild(StructBody1);
            ClinicalDocROOT.AppendChild(comp1);
            //xmlDoc.Save("D:\\SeniorDesign\testReport.xml");
            xmlDoc.Save("D:\\testReport.xml");
         //   return xmlDoc;
        }


        [HttpPost]
          public ActionResult CreateXMLtake2(string title,
                              TemplateViewModel Procedure,
                              TemplateViewModel ClinicalInformation,
                              TemplateViewModel Comparison,
                              TemplateViewModel Findings,
                              TemplateViewModel Impression,
                              TemplateViewModel PatientInfo)
          {
                var xmlModelData = new XmlModel();
             //  Create elements in XmlModel object using values in rData
                 return new XmlResult<XmlModel>()
                 {
                     Data = xmlModelData
                  };
          }
        public class XmlResult<T> : ActionResult
        {
            public T Data { private get; set; }
            
            public void ExecuteResult(ControllerContext context) // override void ExecuteResult(ControllerContext context)
            {
                //HttpContext httpContextBase = context.HttpContext;
                context.HttpContext.Response.Clear();
                context.HttpContext.Response.ContentType = "text/xml";
                
               // context.HttpContext.Response.ContentEncoding = Encoding.UTF8;

                var fileName = DateTime.Now.ToString("ddmmyyyyhhss") + ".xml";
                context.HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=" + fileName);
                
                //httpContextBase.Response.ContentType = "text/xml";

                using (StringWriter writer = new StringWriter())
                {
                    XmlSerializer xml = new XmlSerializer(typeof(T));
                    xml.Serialize(writer, Data);
                    String outputString = writer.ToString();
                    context.HttpContext.Response.WriteAsync(outputString, default(System.Threading.CancellationToken)); //(Encoding.UTF8, writer, Encoding.UTF8); //.ToString(writer);
  
                }
            } 
        }
  /*      //Another try
        public class XmlActionResult : ActionResult
        {
            
            public object HttpContext { get; private set; }
            public XmlActionResult(object data)
            {
                Data = data;
            }
            public object Data { get; private set; }

            public void ExecuteResult(ControllerContext context) // override void ExecuteResult(ControllerContext context)
            {
                //HttpContext httpContextBase = context.HttpContext;
                context.HttpContext.Response.Clear();
                context.HttpContext.Response.ContentType = "text/xml";

                // context.HttpContext.Response.ContentEncoding = Encoding.UTF8;

                var fileName = DateTime.Now.ToString("ddmmyyyyhhss") + ".xml";
                context.HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=" + fileName);

                using (StringWriter writer = new StringWriter())
                {
                    //  XmlSerializer xml = new XmlSerializer(typeof(T));
                    XmlSerializer xml = new XmlSerializer(typeof());
                    xml.Serialize(writer, Data);
                    String outputString = writer.ToString();
                    context.HttpContext.Response.WriteAsync(outputString, default(System.Threading.CancellationToken)); 
                    //(Encoding.UTF8, writer, Encoding.UTF8); //.ToString(writer);                                                                                               
                // context.HttpContext.Response.WriteAsync( //(writer); //Write(writer);
                  // to serialize the model to the response stream :
                 // context.HttpContext.Response.OnCompleted()

                }
            }
        }
*/
        // Takes in a title and a series of nodes
        public Document CreatePDF(string title,
                            TemplateViewModel Procedure,
                            TemplateViewModel ClinicalInformation,
                            TemplateViewModel Comparison,
                            TemplateViewModel Findings,
                            TemplateViewModel Impression,
                            TemplateViewModel PatientInfo)
        {
            // Place a file in results/title for Patient_01.pdf, and specify the file type and format
            using (FileStream fs = new FileStream(
                "results/" + title + " for Patient_01.pdf", FileMode.Create, FileAccess.Write, FileShare.None))
            using (Document doc = new Document(PageSize.LETTER))
            using (PdfWriter writer = PdfWriter.GetInstance(doc, fs))
            {
                var TitleFont = new Font(Font.FontFamily.COURIER, 18f, Font.NORMAL);
                var DefaultFont = new Font(Font.FontFamily.COURIER, 11f, Font.NORMAL);
                doc.Open();
                // Create each paragraph in the report
                Paragraph titleParagraph = new Paragraph(title, TitleFont);
                Paragraph procedureParagraph = ParagraphConstruct(Procedure);
                Paragraph ciParagraph = ParagraphConstruct(ClinicalInformation);
                Paragraph comparisonParagraph = ParagraphConstruct(Comparison);
                Paragraph impressionParagraph = ParagraphConstruct(Impression);
                Paragraph findingsParagraph = ParagraphConstruct(Findings);
                Paragraph patientParagraph = ParagraphConstruct(PatientInfo);

                // Setting paragraph's text alignment using iTextSharp.text.Element class
                titleParagraph.Alignment = Element.ALIGN_CENTER;
                procedureParagraph.Alignment = Element.ALIGN_JUSTIFIED;

                // Add the actual paragraphs to the report
                doc.Add(titleParagraph);
                doc.Add(patientParagraph);
                doc.Add(procedureParagraph);
                doc.Add(comparisonParagraph);
                doc.Add(findingsParagraph);
                doc.Add(impressionParagraph);

                // Add metadata
                doc.AddTitle(title);
                doc.AddSubject(title + " Report for Patient 001");
                doc.AddKeywords(title + ", + Synoptic, Reporting");
                doc.AddCreator("Synoptic Reporting Thing");
                doc.AddAuthor("Synoptic Reporting Services");
                doc.AddHeader("Nothing", "No Header");
                doc.Close();
                return doc;
            }
            // Response.Redirect("~/results/test.pdf");
      
        }


        /// <summary>
        /// RetreiveFirstCode: string -> List of (string, string, string)
        /// Takes in the string path of a file and extracts the Radlex codes and names associated with them
        /// Where List[0] = Code List[1] = String List[2] = ID found in HTML
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        List<(string, string, string)> RetreiveFirstCode(string path)
        {
            //Find all the lines in a file in a particular folder
            string workingDirectory = Environment.CurrentDirectory;
            string URL = workingDirectory + "/Views/Templates/" + path + ".cshtml";
            List<string> allLines = new List<string>(System.IO.File.ReadAllLines(URL));
            var usefulLines = new List<(string, string, string)>();
            // Search for the lines which contain trigger values and add the relevant bits to a list
            for (int i = 0; i < allLines.Count; i++)
            {
                string line = allLines[i];
                if (line.Contains("//template_attributes"))
                {
                    break;
                }
                else if (line.Contains("code meaning") && allLines[i - 2].Contains("ORIGTXT"))
                {
                    usefulLines.Add((Between(line, "value=\"", "\""), Between(line, "meaning=\"", "\""), Between(allLines[i - 2], "ORIGTXT=\"", "\"")));
                    Console.WriteLine(line);
                }
            }

            for (int i = 0; i < usefulLines.Count; i++)
            {
                Console.WriteLine(usefulLines[i]);
            }
            return usefulLines;
        }

        /// <summary>
        /// Get string value between [first] a and [last] b.
        /// </summary>
        private string Between(string value, string a, string b)
        {
            int posA = value.IndexOf(a);
            int posB = value.IndexOf(b, posA + a.Length);
            if (posA == -1)
            {
                return "";
            }
            if (posB == -1)
            {
                return "";
            }
            int adjustedPosA = posA + a.Length;
            if (adjustedPosA >= posB)
            {
                return "";
            }
            return value.Substring(adjustedPosA, posB - adjustedPosA);
        }

        // TemplateViewModel -> Paragraph
        private Paragraph ParagraphConstruct(TemplateViewModel node)
        {
            var defaultFont = new Font(Font.FontFamily.COURIER, 11f, Font.NORMAL);
            var output = new Paragraph(String.Empty, defaultFont);
            // If we're looking at a data node, then it must have been by itself and therefore is a text field
            // Therefore just add to a paragraph and call it good
            if (node.IsLeaf)
            {
                output = new Paragraph(node.Header + ": " + node.Result + "\n", defaultFont);
            }
            else
            {
            // Create a table for all the things if they're leaves
            // Otherwise recurse bcz we want to group the tables
                if (node.ChildNodes[0].IsLeaf && node.ChildNodes.Count > 1)
                {
                    //Make a table to add
                    output.Add(new Paragraph(node.Header + "\n", defaultFont));
                    PdfPTable table = Tablify(node);
                    output.Add(table);
                }
                else
                {
                    // Create a paragraph for each of the children
                    // This path is primarily followed to deal with nested sections (not text fields/inputs)
                    for (int i = 0; i < node.ChildNodes.Count; i++)
                    {
                        output.Add(ParagraphConstruct(node.ChildNodes[i]));
                    }

                }

            }
            return output;
        }

        // TemplateViewModel -> PdfTable
        // Create table from a node with multiple children
        private PdfPTable Tablify(TemplateViewModel node)
        {
            // Setup
            PdfPTable output = new PdfPTable(2)
            {
                SpacingAfter = 3,
                SpacingBefore = 6
            };
            PdfPCell cat = new PdfPCell(new Phrase("Category"))
            {
                BackgroundColor = new BaseColor(211, 211, 211)
            };
            PdfPCell fin = new PdfPCell(new Phrase("Finding"))
            {
                BackgroundColor = new BaseColor(211, 211, 211)
            };
            output.AddCell(cat);
            output.AddCell(fin);
      
            // Add each of the children's headers and results
            // Default to empty string since null strings are sometime bad
            for(int i = 0; i < node.ChildNodes.Count; i++)
            {
                output.AddCell(node.ChildNodes[i].Header ?? String.Empty);
                output.AddCell(node.ChildNodes[i].Result ?? String.Empty);
            }
            return output;
        }

        /// <summary>
        /// Yup
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        private byte[] GetBinaryFile(string filename)
        {
            byte[] bytes;
            using (FileStream file = new FileStream(filename, FileMode.Open, FileAccess.Read))
            {
                bytes = new byte[file.Length];
                file.Read(bytes, 0, (int)file.Length);
            }
            return bytes;
        }
    }
}
