using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Xml;
using SmartFhirApplication.Models;

namespace SmartFhirApplication.Controllers
{
    public class XMLController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        //My attempt at creating new for Clinical Information.  
        //This is mostly hard coded at this point and will need to be implemented more dynamically 
        public void ReportDocumentXML()
        {
            XmlDocument xmlDoc = new XmlDocument();
            // try { xmlDoc.Load("testReport.xml");  }
            // catch (System.IO.FileNotFoundException)

            XmlNode rootNode = xmlDoc.CreateElement("section");
            xmlDoc.AppendChild(rootNode);   //where should this go?  

            XmlAttribute classAtt = xmlDoc.CreateAttribute("classCode");
            classAtt.Value = "DOCSECT"; //might need to change 
            rootNode.Attributes.Append(classAtt);
            XmlAttribute moodAtt = xmlDoc.CreateAttribute("moodCode");
            moodAtt.Value = "EVN"; //might need to change 
            rootNode.Attributes.Append(moodAtt);
            //templateId
            XmlElement templateIdElement = xmlDoc.CreateElement("templateId");
            //templateIdElement.SetAttribute("root", value);  //number will change
            //use above when making modular??
            XmlAttribute rootAtt = xmlDoc.CreateAttribute("root");
            rootAtt.Value = "4";    //put in value call here for loop and such to get value 
            templateIdElement.Attributes.Append(rootAtt);
            rootNode.AppendChild(templateIdElement);
            //ID
            XmlElement idElement = xmlDoc.CreateElement("id");
            //rootAtt = xmlDoc.CreateAttribute("root");   //is this line needed or could you just reset the value?
            rootAtt.Value = "12";   //modular call to actual value 
            idElement.Attributes.Append(rootAtt);
            rootNode.AppendChild(idElement);
            //Code 
            XmlElement codeElement = xmlDoc.CreateElement("code");
            XmlAttribute codeAtt = xmlDoc.CreateAttribute("code");
            codeAtt.Value = "55752-0";    //put in value call here for loop and such to get value 
            codeElement.Attributes.Append(codeAtt);
            codeAtt = xmlDoc.CreateAttribute("codeSystem");
            codeAtt.Value = "2.16";    //put in value call here for loop and such to get value 
            codeElement.Attributes.Append(codeAtt);
            codeAtt = xmlDoc.CreateAttribute("codeSystemName");
            codeAtt.Value = "LOINC";    //put in value call here for loop and such to get value 
            codeElement.Attributes.Append(codeAtt);
            codeAtt = xmlDoc.CreateAttribute("displayName");
            codeAtt.Value = "Clinical Information";    //put in value call here for loop and such to get value 
            codeElement.Attributes.Append(codeAtt);
            rootNode.AppendChild(codeElement);
            //Title Node 
            XmlNode titleNode = xmlDoc.CreateElement("title");
            titleNode.InnerText = "Clinical Information";
            rootNode.AppendChild(titleNode);
            //Text
            XmlNode textNode = xmlDoc.CreateElement("text");
            titleNode.InnerText = "The patient did stuff...blah ";
            rootNode.AppendChild(textNode);
            

            xmlDoc.Save("testReport.xml");
        }
        //Now I am looking at the pre-existing templates and will try to load in the information and edit the it and then maybe save it into a new location 
        //How is that done?   
        public void loadTempXML ()
        {
            XmlDocument doc = new XmlDocument();
            doc.Load("test.xml");//make this name better for each patient and the study done 
          //  loadView; //want to use this function from site.js but how to implement?  
          //need to figure out how to pull the information from template 

        } 
           /* string title,
                       TemplateViewModel Procedure,
                       TemplateViewModel ClinicalInformation,
                       TemplateViewModel Comparison,
                       TemplateViewModel Findings,
                       TemplateViewModel Impression)
                       */
    

    }
}