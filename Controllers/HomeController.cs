//This file is part of MUSynopticReporting.

//MUSynopticReporting is free software: you can redistribute it and/or modify
//    it under the terms of the GNU Affero General Public License as published by
//    the Free Software Foundation, either version 3 of the License, or
//    (at your option) any later version.

//    MUSynopticReporting is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
//    GNU Affero General Public License for more details.

//    You should have received a copy of the GNU Affero General Public License
//    along with MUSynopticReporting.If not, see<https://www.gnu.org/licenses/>.

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
            //createXML1();
            XMLtheFinalFrontier(title);

            return Convert.ToBase64String(bitArr); 
        }
        
        public void createXML1 () { 
           
            XmlDocument xmlDoc = new XmlDocument();
  
            XmlNode ClinicalDocROOT = xmlDoc.CreateElement("ClinicalDocument");
            xmlDoc.AppendChild(ClinicalDocROOT);
            XmlNode ProcedureNode = xmlDoc.CreateElement("Procedure");
            ClinicalDocROOT.AppendChild(ProcedureNode);
            XmlNode comp1 = xmlDoc.CreateElement("component");
            XmlNode StructBody1 = xmlDoc.CreateElement("StructuredBody");
            XmlNode comp2 = xmlDoc.CreateElement("component");
            comp2.InnerText = "data goes here";
            StructBody1.AppendChild(comp2);
            
            comp1.AppendChild(StructBody1);
            ClinicalDocROOT.AppendChild(comp1);
           // xmlDoc.Save("D:\\testReport.xml");
            xmlDoc.Save("C:\\Users\\Natalie\\Source\\repos\\SynopticReporting\\SynopticReporting-1\\Results\\testing.xml");
        }

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
                doc.Add(ciParagraph);
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

        //EVERYTHING AFTER THIS POINT IS FOR XML
        //Equivenlent to a XML main function 
        public void XMLtheFinalFrontier(string title)
        {
            string fileLocation = "C:\\Users\\Natalie\\Source\\repos\\SynopticReporting\\SynopticReporting-1\\Results\\xml_" + title + ".xml";
            XmlDocument xmlDoc = new XmlDocument();
            //Create an XML-declaration using XmlDocument.CreateXmlDeclaration Method:

            XmlNode docNode = xmlDoc.CreateXmlDeclaration("1.0", "UTF-8", null);
            xmlDoc.AppendChild(docNode);
            xmlDoc.AppendChild(xmlDoc.CreateProcessingInstruction("xml-stylesheet",
                                "type='text/xsl' href='CDA-DIR.xsl'"));
            XmlNode RootClinDoc = xmlDoc.CreateElement("ClinicalDocument");

            //Add general header part here 
            GenHeader_xml(xmlDoc, RootClinDoc, title);
            //start of all Sections 
            XmlNode component = xmlDoc.CreateElement("component");
            XmlNode structBody = xmlDoc.CreateElement("structuredBody");

            //call each section (with structBody)
            ClinInfo_xml(xmlDoc, structBody);
            ImagProc_xml(xmlDoc, structBody);
            Findings_xml(xmlDoc, structBody);
            Impressions_xml(xmlDoc, structBody);

            //end of Sections 
            component.AppendChild(structBody);
            RootClinDoc.AppendChild(component);
            xmlDoc.AppendChild(RootClinDoc);
            xmlDoc.Save(fileLocation);
            //xmlDoc.Save("D:\\xmlOutput.xml");
            //xmlDoc.Save("C:\\Users\\Natalie\\Source\\repos\\SynopticReporting\\SynopticReporting-1\\Results\\testing.xml");
        }

        public void GenHeader_xml(XmlDocument xmlDoc, XmlNode Root, string title)
        {
            string timing = DateTime.Now.ToString("yyyyMMddHHmmss");

            //templateId
            XmlElement templateIdElement = xmlDoc.CreateElement("templateId");
            XmlAttribute rootAtt = xmlDoc.CreateAttribute("root");
            rootAtt.Value = "1.2.840.10008.9.20";    //Value may change.    
            templateIdElement.Attributes.Append(rootAtt);
            Root.AppendChild(templateIdElement);
            //typeId
            XmlElement TypeidElement = xmlDoc.CreateElement("TypeId");
            rootAtt = xmlDoc.CreateAttribute("root");
            rootAtt.Value = "2.16.840.1.113883.​1.3";    //Value may change. 
            TypeidElement.Attributes.Append(rootAtt);
            XmlAttribute extension = xmlDoc.CreateAttribute("extension");
            extension.Value = "POCD_HD000040";
            TypeidElement.Attributes.Append(extension);    
            Root.AppendChild(TypeidElement);
            //id
            XmlElement idElement = xmlDoc.CreateElement("id");
            rootAtt = xmlDoc.CreateAttribute("root");
            rootAtt.Value = "temp";    //Value will change. 
            idElement.Attributes.Append(rootAtt);
            Root.AppendChild(idElement);
            //title 
            XmlNode titleNode = xmlDoc.CreateElement("title");
            titleNode.InnerText = title;
            Root.AppendChild(titleNode);
            //Creation time
            XmlElement effTime = xmlDoc.CreateElement("effectiveTime");
            XmlAttribute timeValue = xmlDoc.CreateAttribute("value");
            timeValue.Value = timing;
            effTime.Attributes.Append(timeValue);
            Root.AppendChild(effTime);
            //Confidentiality
            XmlElement confEl = xmlDoc.CreateElement("confidentialityCode");
            Root.AppendChild(confEl); //should have information in this node but cannot determine what value 
            //Language Code 
            XmlElement langCode = xmlDoc.CreateElement("languageCode");
            XmlAttribute codeAtt = xmlDoc.CreateAttribute("code");
            codeAtt.Value = "en-US";    //for now, assume all are English    
            langCode.Attributes.Append(codeAtt);
            Root.AppendChild(langCode);
            //Patient 
            //Author 

            //Recipient 
        }

        //Clinical Information Section 
        public void ClinInfo_xml(XmlDocument xmlDoc, XmlNode Root_struct)
        {
            //initialize values for SectionHeader_xml
            string templateId_val = "1.2.840.10008.9.2";
            string id_val = "1.2.840.10213.2.62.994948294044785528";    //will change 
            string code_val = "55752-0";
            string dispName_val = "Clinical Information";
            string title_val = "Clinical Information";
            string text_val = "This case... will need to be read";

            XmlNode component = xmlDoc.CreateElement("component");
         //   XmlElement startNote = xmlDoc.CreateElement("ClinicalInformationSection");//might not be the correct type for the section header
          //  component.AppendChild(startNote);
            //section 
            XmlNode section = xmlDoc.CreateElement("section");
            XmlAttribute classAtt = xmlDoc.CreateAttribute("classCode");
            classAtt.Value = "DOCSECT";
            section.Attributes.Append(classAtt);
            XmlAttribute moodAtt = xmlDoc.CreateAttribute("moodCode");
            moodAtt.Value = "EVN";
            section.Attributes.Append(moodAtt);

            //call Section header 
            SectionHeader_xml(xmlDoc, section, templateId_val, id_val, code_val, dispName_val, title_val, text_val);
            component.AppendChild(section);
        //    XmlElement endNote = xmlDoc.CreateElement("End of Clinical Information Section");
        //    component.AppendChild(endNote);
            //close out Clinical Information component
            Root_struct.AppendChild(component);
        }

        //Imaging Procedure Description Section
        public void ImagProc_xml(XmlDocument xmlDoc, XmlNode Root_struct)
        {
            string templateId_val = "1.2.840.10008.9.3";
            string id_val = "1.2.840.10213.2.62.994948294044785528";    //value will change 
            string code_val = "55111-9";
            string dispName_val = "Current Imaging Procedure Description";
            string title_val = "Imaging Procedure Description";
            string text_val = "This case... will need to be read";

            XmlNode component = xmlDoc.CreateElement("component");
        //    XmlElement startNote = xmlDoc.CreateElement("Imaging Procedure Description Section");//might not be the correct type for the section header
        //    component.AppendChild(startNote);
            //section 
            XmlNode section = xmlDoc.CreateElement("section");
            XmlAttribute classAtt = xmlDoc.CreateAttribute("classCode");
            classAtt.Value = "DOCSECT";
            section.Attributes.Append(classAtt);
            XmlAttribute moodAtt = xmlDoc.CreateAttribute("moodCode");
            moodAtt.Value = "EVN";
            section.Attributes.Append(moodAtt);

            //call Section header 
            SectionHeader_xml(xmlDoc, section, templateId_val, id_val, code_val, dispName_val, title_val, text_val);
            component.AppendChild(section);
        //    XmlElement endNote = xmlDoc.CreateElement("End of Imaging Procedure Description Section");
        //    component.AppendChild(endNote);
            //close out Imaging Procedure Description component
            Root_struct.AppendChild(component);
        }

        //Findings Section 
        public void Findings_xml(XmlDocument xmlDoc, XmlNode Root_struct)
        {
            string templateId_val = "2.16.840.1.113883.10.20.6.1.2";
            string id_val = "1.2.840.10213.2.62.994948294044785528";    //this value will change
            string code_val = "59776-5";
            string dispName_val = "Procedure Findings";
            string title_val = "Findings";
            string text_val = "This case... will need to be read";

            XmlNode component = xmlDoc.CreateElement("component");
        //    XmlElement startNote = xmlDoc.CreateElement("Findings Section");//might not be the correct type for the section header
        //    component.AppendChild(startNote);
            //section --might need to move this up since reference section later 
            XmlNode section = xmlDoc.CreateElement("section");
            XmlAttribute classAtt = xmlDoc.CreateAttribute("classCode");
            classAtt.Value = "DOCSECT";
            section.Attributes.Append(classAtt);
            XmlAttribute moodAtt = xmlDoc.CreateAttribute("moodCode");
            moodAtt.Value = "EVN";
            section.Attributes.Append(moodAtt);

            //call Section header 
            SectionHeader_xml(xmlDoc, section, templateId_val, id_val, code_val, dispName_val, title_val, text_val);
            component.AppendChild(section);
        //    XmlElement endNote = xmlDoc.CreateElement("End of Findings Section");
        //    component.AppendChild(endNote);
            //close out Findings component
            Root_struct.AppendChild(component);
        }

        /*
          Impressions Section
          Standard followed from DICOM part20 table in section 9.6 
        */
        public void Impressions_xml(XmlDocument xmlDoc, XmlNode Root_struct)
        {
            string templateId_val = "1.2.840.10008.9.5";    //value may change 
            string id_val = "1.2.840.10213.2.62.994948294044785528";
            string code_val = "19005-8";
            string dispName_val = "Impressions";
            string title_val = "Impressions";
            string text_val = "This case... will need to be read";

            XmlNode component = xmlDoc.CreateElement("component");
        //    XmlElement startNote = xmlDoc.CreateElement("Impressions Section");//might not be the correct type for the section header
        //    component.AppendChild(startNote);
            //section --might need to move this up since reference section later 
            XmlNode section = xmlDoc.CreateElement("section");
            XmlAttribute classAtt = xmlDoc.CreateAttribute("classCode");
            classAtt.Value = "DOCSECT";
            section.Attributes.Append(classAtt);
            XmlAttribute moodAtt = xmlDoc.CreateAttribute("moodCode");
            moodAtt.Value = "EVN";
            section.Attributes.Append(moodAtt);

            //call Section header 
            SectionHeader_xml(xmlDoc, section, templateId_val, id_val, code_val, dispName_val, title_val, text_val);
            component.AppendChild(section);
         //   XmlElement endNote = xmlDoc.CreateElement("End of Impressions Section");
        //    component.AppendChild(endNote);
            //close out Impressions component
            Root_struct.AppendChild(component);
        }

        public void SectionHeader_xml(XmlDocument xmlDoc, XmlNode section, string templateId_val,
                                        string id_val, string code_val, string dispName_val,
                                        string title_val, string text_val)
        {
            //templateId
            XmlElement templateIdElement = xmlDoc.CreateElement("templateId");
            XmlAttribute rootAtt = xmlDoc.CreateAttribute("root");
            rootAtt.Value = templateId_val;    //Value may change.    
            templateIdElement.Attributes.Append(rootAtt);
            section.AppendChild(templateIdElement);
            //id
            XmlElement idElement = xmlDoc.CreateElement("id");
            rootAtt = xmlDoc.CreateAttribute("root");
            rootAtt.Value = id_val;    //Value will change. 
            idElement.Attributes.Append(rootAtt);
            section.AppendChild(idElement);
            //code 
            XmlElement codeElement = xmlDoc.CreateElement("code");
            XmlAttribute codeAtt = xmlDoc.CreateAttribute("code");
            codeAtt.Value = code_val;   //unique per section  
            codeElement.Attributes.Append(codeAtt);
            codeAtt = xmlDoc.CreateAttribute("codeSystem");
            codeAtt.Value = "2.16.840.1.113883.6.1";    //All LOINC have the same system code 
            codeElement.Attributes.Append(codeAtt);
            codeAtt = xmlDoc.CreateAttribute("codeSystemName");
            codeAtt.Value = "LOINC";  //All are LOINC
            codeElement.Attributes.Append(codeAtt);
            codeAtt = xmlDoc.CreateAttribute("displayName");
            codeAtt.Value = dispName_val;    //put in value call here for loop and such to get value 
            codeElement.Attributes.Append(codeAtt);
            section.AppendChild(codeElement);
            //title 
            XmlNode titleNode = xmlDoc.CreateElement("title");
            titleNode.InnerText = title_val;
            section.AppendChild(titleNode);
            //Text
            XmlNode textNode = xmlDoc.CreateElement("text");
            textNode.InnerText = text_val; //will need to be read in from template 
            section.AppendChild(textNode);
        }


    }
}
