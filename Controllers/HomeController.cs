using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using SmartFhirApplication.Models;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Linq;
using static SmartFhirApplication.Controllers.FHIRController;

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
        public IActionResult LoadTemplate(string path, string name)
        {
            ViewData["Msg"] = path;
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
        public void Create(string title,
                            TemplateViewModel PatientInfo,
                            TemplateViewModel Procedure,
                            TemplateViewModel ClinicalInformation,
                            TemplateViewModel Comparison,
                            TemplateViewModel Findings,
                            TemplateViewModel Impression,
                            string Location)
        {
            CreatePDF(title, PatientInfo, Procedure, ClinicalInformation, Comparison, Findings, Impression);
            //Maybe call make XML here? 
        }

        // Takes in a title and a series of nodes
        public void CreatePDF(string title,
                            TemplateViewModel PatientInfo,
                            TemplateViewModel Procedure,
                            TemplateViewModel ClinicalInformation,
                            TemplateViewModel Comparison,
                            TemplateViewModel Findings,
                            TemplateViewModel Impression)
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
                Paragraph patientParagraph = ParagraphConstruct(PatientInfo);
                Paragraph procedureParagraph = ParagraphConstruct(Procedure);
                Paragraph ciParagraph = ParagraphConstruct(ClinicalInformation);
                Paragraph comparisonParagraph = ParagraphConstruct(Comparison);
                Paragraph impressionParagraph = ParagraphConstruct(Impression);
                Paragraph findingsParagraph = ParagraphConstruct(Findings);
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
                doc.AddTitle(title);
                doc.AddSubject(title + " Report for Patient 001");
                doc.AddKeywords(title + ", + Synoptic, Reporting");
                doc.AddCreator("Synoptic Reporting Thing");
                doc.AddAuthor("Synoptic Reporting Services");
                doc.AddHeader("Nothing", "No Header");
                doc.Close();

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

        private Paragraph autofill(Paragraph clin)
            {
                var defaultFont = new Font(Font.FontFamily.COURIER, 11f, Font.NORMAL);
                var ans = new Paragraph(String.Empty, defaultFont);
                string info = PatientClass.PatientMethod("siimravi", "Clinical information: ");
                ans = new Paragraph(info, defaultFont);
                return ans;
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
    }
}



//Ideas for outputting without storing locally? In case the Smart application is obnoxious with this stuff
//using (MemoryStream ms = new MemoryStream())
//{
//    Document doc = new Document(PageSize.A4, 60, 60, 10, 10);
//PdfWriter pw = PdfWriter.GetInstance(doc, ms);
//    //your code to write something to the pdf
//    return ms.ToArray();
//}

