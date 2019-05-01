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
        public static void ReportDocumentXML()
        {
            XmlDocument xmlDoc = new XmlDocument();
            // try { xmlDoc.Load("testReport.xml");  }
            // catch (System.IO.FileNotFoundException)
            //loaded from example DICOM part 20
            xmlDoc.LoadXml("<?xml version= \"1.0\" encoding=\"utf-8\"?>"
                    + "<?xml-stylesheet type=\"text/xsl\" href=\"CDA-DIR.xsl\"?>"
                    + "<ClinicalDocument xmlns=\"urn: hl7 - org:v3\"" 
                    + "xmlns:voc = \"urn:hl7-org:v3/voc\""
                    + "xmlns: xsi = \"http://www.w3.org/2001/XMLSchema-instance \""
                    + "xmlns: ps3 - 20 = \"urn:dicom-org:ps3-20\""
                    + "xsi: schemaLocation = \"urn:hl7-org:v3 CDA.xsd\" >"
                    + "<realmCode code=\"UV\"/>");
            // beginningSection(xmlDoc);
            XmlNode node1 = xmlDoc.CreateElement("component");
            xmlDoc.AppendChild(node1);
            XmlNode node2 = xmlDoc.CreateElement("structuredBody");
            node1.AppendChild(node2);
            XmlNode node3 = xmlDoc.CreateElement("component");
            node2.AppendChild(node3);

            clientInfoXML(xmlDoc, node3);
         

            xmlDoc.Save("C:\\testReport.xml");
        }

        public void beginningSection(XmlDocument xmlDoc)
        {
            //kind of counfused what goes here...?
            XmlNode rootNode = xmlDoc.CreateElement("ClinicalDocument");
        }

        //Clinical Information section 
        public static void clientInfoXML(XmlDocument xmlDoc, XmlNode componentNode)
        {
            //how to add comment clinical information section?
            XmlNode rootNode = xmlDoc.CreateElement("section");
            componentNode.AppendChild(rootNode);    //does this go here?  
            //templateId
            XmlElement templateIdElement = xmlDoc.CreateElement("templateId");
            XmlAttribute rootAtt = xmlDoc.CreateAttribute("root");
            //templateIdElement.SetAttribute("root", value);  //number will change
            //use above when making modular??
            rootAtt.Value = "4";    //put in value call here for loop and such to get value //VALUE WILL CHANGE
            templateIdElement.Attributes.Append(rootAtt);
            rootNode.AppendChild(templateIdElement);
          
            CodeSubSubSection(xmlDoc, rootNode);
            //Title Node 
            XmlNode titleNode = xmlDoc.CreateElement("title");
            titleNode.InnerText = "Clinical Information";
            rootNode.AppendChild(titleNode);

            //Component before procedure indications subsection
            XmlNode comp1Node = xmlDoc.CreateElement("component");
            rootNode.AppendChild(comp1Node);
            //put in Start of Procedure Indications Subsections comment here
            //there is a comment in the example about mapping to reson procedure is requested 
            XmlNode sectionNode1 = xmlDoc.CreateElement("section");
            comp1Node.AppendChild(sectionNode1);
            sectionSubNode(xmlDoc, sectionNode1);
            //Add comment "End of Procedure Indications Subsection"

            //History Subsection
            comp1Node = xmlDoc.CreateElement("component");
            rootNode.AppendChild(comp1Node);
            //Add comment "History Subsection"
            sectionNode1 = xmlDoc.CreateElement("section");
            comp1Node.AppendChild(sectionNode1);
            sectionSubNode(xmlDoc, sectionNode1);//for this call of sectionSubNode, will need to add <paragraph> and other sub elements to text !!!
            //entry
            XmlNode entryNode = xmlDoc.CreateElement("entry");
            sectionNode1.AppendChild(entryNode);
            //add comment "History Report element (TEXT)"
            XmlNode observNode = xmlDoc.CreateElement("observation");
            XmlAttribute classCodeAtt = xmlDoc.CreateAttribute("classCode");
            classCodeAtt.Value = "OBS"; //value needs to be read in 
            observNode.Attributes.Append(classCodeAtt);
            XmlAttribute moodCodeAtt = xmlDoc.CreateAttribute("moodCode");
            moodCodeAtt.Value = "EVN"; //value needs to be read in 
            observNode.Attributes.Append(moodCodeAtt);
            //templateID root element 
            XmlElement tempIdElement = xmlDoc.CreateElement("templateId");
            XmlAttribute rootAtt1 = xmlDoc.CreateAttribute("root");
            rootAtt1.Value = "4";    //put in value call here for loop and such to get value 
            tempIdElement.Attributes.Append(rootAtt1);
            observNode.AppendChild(tempIdElement);
            //code Element 
            CodeSubSubSection(xmlDoc, observNode);
            
            //value node 


            //Add comment end of history subsection 
            //Add comment end of clinical information section 
        }

        public static void ImagProcDescrip(XmlDocument xmlDoc, XmlNode rootNode)
        {
            //Title Node 
            XmlNode titleNode = xmlDoc.CreateElement("title");
            titleNode.InnerText = "Clinical Information";
            rootNode.AppendChild(titleNode);


        }

        public static void FindingsSection (XmlDocument xmlDoc, XmlNode rootNode)
        {
            XmlNode titleNode = xmlDoc.CreateElement("title");
            titleNode.InnerText = "Findings";
            rootNode.AppendChild(titleNode);
        }

        //subsection called "section" that is often called in the xml so it seemed easier to make one function for it 
        //the .Value needs to be loaded in better than this since it changes depending on where it is called
        public static void sectionSubNode(XmlDocument xmlDoc, XmlNode rootNode)
        {
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
            rootAtt = xmlDoc.CreateAttribute("root");   //is this line needed or could you just reset the value?
            rootAtt.Value = "12";   //modular call to actual value 
            idElement.Attributes.Append(rootAtt);
            rootNode.AppendChild(idElement);
            //code
            CodeSubSubSection(xmlDoc, rootNode);


            //Title Node 
            XmlNode titleNode = xmlDoc.CreateElement("title");
            titleNode.InnerText = "Clinical Information";
            rootNode.AppendChild(titleNode);
            //Text
            XmlNode textNode = xmlDoc.CreateElement("text");
            titleNode.InnerText = "The patient did stuff...blah ";
            rootNode.AppendChild(textNode);
        }

        public static void CodeSubSubSection(XmlDocument xmlDoc, XmlNode rootNode)
        {
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
        }

        //ORIGINAL XML CODE ATTEMPT
        //not actually utilized but should keep for reference 
        public void ClinicalInformation(XmlDocument xmlDoc)
        {
            XmlNode rootNode = xmlDoc.CreateElement("section");
            xmlDoc.AppendChild(rootNode);

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