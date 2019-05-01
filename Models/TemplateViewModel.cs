using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace SmartFhirApplication.Models
{
  // TemplateViewModel = TemplateNode
  public class TemplateViewModel
  {
    //If IsLeaf Then Result, code, etc will be not-null but still check for nulls
    //Otherwise use ChildNodes list and Header
    public bool IsLeaf { get; set; }
    public List<TemplateViewModel> ChildNodes { get; set; }
    public string Result { get; set; }
    public string Code { get; set; }
    public string Header { get; set; }
  }

  public class XmlModel
    {
        [XmlRoot("ClinicalDocument")]
        public class Data
        {
            [XmlElement("component")]
            public TemplateViewModel TemplateViewModel { get; set; }
           // public templateId templateId { get; set; }
        }
      //  [SerializableAttribute()]
      //  public class TemplateViewModel
    //    {
   //         [XmlElementAttribute()]
     //       public 
    //    }
    }
    
}
