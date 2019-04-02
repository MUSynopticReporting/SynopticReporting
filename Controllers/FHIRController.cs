using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using Hl7.Fhir.Model;
using Hl7.Fhir.Rest;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SmartFhirApplication.Controllers
{

  public class Meta
  {
    public string versionId { get; set; }
    public DateTime lastUpdated { get; set; }
  }

  public class Text
  {
    public string status { get; set; }
    public string div { get; set; }
  }

  public class Identifier
  {
    public string use { get; set; }
    public string system { get; set; }
    public string value { get; set; }
  }

  public class Coding
  {
    public string system { get; set; }
    public string code { get; set; }
  }

  public class Category
  {
    public List<Coding> coding { get; set; }
  }

  public class Coding2
  {
    public string system { get; set; }
    public string code { get; set; }
  }

  public class Code
  {
    public List<Coding2> coding { get; set; }
    public string text { get; set; }
  }

  public class Subject
  {
    public string reference { get; set; }
  }

  public class RootObject
  {
    public string resourceType { get; set; }
    public string id { get; set; }
    public Meta meta { get; set; }
    public Text text { get; set; }
    public List<Identifier> identifier { get; set; }
    public string status { get; set; }
    public Category category { get; set; }
    public Code code { get; set; }
    public Subject subject { get; set; }
    public string effectiveDateTime { get; set; }
    public DateTime issued { get; set; }
    public string conclusion { get; set; }
  }

  public class User
  {
    public User(string json)
    {
      JObject jObject = JObject.Parse(json);
      JToken jUser = jObject["text"];
      status = (string)jUser["status"];
      //teamname = (string)jUser["teamname"];
      //email = (string)jUser["email"];
      //players = jUser["players"].ToArray();
    }

    public string status { get; set; }
    public string teamname { get; set; }
    public string email { get; set; }
    public Array players { get; set; }
  }

  public class Accession
  {
    public Accession(string json)
    {
      JObject jObject = JObject.Parse(json);
      value = jObject["identifier"][0]["value"].ToString();
      system = jObject["identifier"][0]["system"].ToString();
    }
    public string ans { get; set; }
    public string system { get; set; }
    public string value { get; set; }
  }

  public class CodeSystem
  {
    public CodeSystem(string json)
    {
      JObject jObject = JObject.Parse(json);
      code = jObject["code"]["coding"][0]["code"].ToString();
      system = jObject["code"]["coding"][0]["system"].ToString();
    }
    public string system { get; set; }
    public string code { get; set; }
  }

    public class resources
    {
        public resources(string json)
        {
            JObject jObject = JObject.Parse(json);
            type = jObject["resourceType"].ToString();
            gender = jObject["gender"].ToString();
            birthDate = jObject["birthDate"].ToString();
        }
        public string type { get; set; }
        public string gender { get; set; }
        public string birthDate { get; set; }
    }

    public class identifier
    {
        public identifier(string json)
        {
            JObject jObject = JObject.Parse(json);
            use = jObject["identifier"][0]["use"].ToString();
            system = jObject["identifier"][0]["system"].ToString();
            value = jObject["identifier"][0]["value"].ToString();
            start = jObject["identifier"][0]["period"]["start"].ToString();
            display = jObject["identifier"][0]["assigner"]["display"].ToString();
        }
        public string use { get; set; }
        public string system { get; set; }
        public string value { get; set; }
        public string start { get; set; }
        public string display { get; set; }
    }

    public class name
    {
        public name(string json)
        {
            JObject jObject = JObject.Parse(json);
            family = jObject["name"][0]["family"].ToString();
            given = jObject["name"][1]["given"].ToString();
        }
        public string family { get; set; }
        public string given { get; set; }
    }

    public class telecom
    {
        public telecom(string json)
        {
            JObject jObject = JObject.Parse(json);
            value = jObject["telecom"][1]["value"].ToString();
        }
        public string value { get; set; }
    }

    public class FHIRController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [Route("FHIRController/FindPatients")]
        public JsonResult FindPatients()
        {
            var result = OpenFhirClient();
            var Patients = result.Search<Patient>().Entry.Select(pat => pat.Resource.IdElement);
            ViewData["PatList"] = Json(Patients);
            return Json(Patients);
        }
        private const string apikey = "f64cabaa-6baf-48aa-b1ed-7857813c5c07";
        public string responseText;
        private static string text { get; set; }

        public static class PatientClass
        {
            private const string PatURL = "http://hackathon.siim.org/fhir/Patient/";

        }

        public JsonResult GetDiagnosticReport(string AccessionId, string Title)
        {
          string URL = "http://hackathon.siim.org/fhir/DiagnosticReport/" + AccessionId;
          HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URL);
          request.Headers["apikey"] = apikey;
          HttpWebResponse response = (HttpWebResponse)request.GetResponse();


          WebHeaderCollection header = response.Headers;
          Trace.Listeners.Add(new TextWriterTraceListener(Console.Out));
          var encoding = ASCIIEncoding.ASCII;
          RootObject RO = new RootObject();
          using (var reader = new System.IO.StreamReader(response.GetResponseStream(), encoding))
          {
                string responseText = reader.ReadToEnd();
                Trace.WriteLine(responseText);

                //User user = new User(responseText);
                //Trace.WriteLine(user.status);
                Accession id = new Accession(responseText);
                Trace.WriteLine(id.value);
                Trace.WriteLine(id.system);
                CodeSystem loinc = new CodeSystem(responseText);
                Trace.WriteLine(loinc.code);
                Trace.WriteLine(loinc.system);
                RO = JsonConvert.DeserializeObject<RootObject>(responseText);
            }
            //Trace.WriteLine("{0} ({1})", (int)response.StatusCode, response.ReasonPhrase);
            Console.WriteLine(RO.conclusion);
            response.Close();
            if (Title.Equals(RO.code.text))
            {
                return Json(RO);
            }
            else
            {
                return Json(null);
            }
        }
        private static FhirClient OpenFhirClient()
        {
            string PatURL = "http://hackathon.siim.org/fhir/";

            var client = new FhirClient(PatURL)
            {
                PreferredFormat = ResourceFormat.Json
            };
            client.OnBeforeRequest += (object sender, BeforeRequestEventArgs e) => {
                e.RawRequest.Headers.Add("apikey", apikey); //requires environment variable to match
            };
            return client;
        }
        [Route("FHIRController/CreateDiagnosticReport")]
        public DiagnosticReport CreateDiagnosticReport()
        {
            Hl7.Fhir.Model.Coding coding = new Hl7.Fhir.Model.Coding
            {
                System = "http://loinc.org",
                Code = "51990-0",
                Display = "Basic Metabolic Panel"
            };
            List<Hl7.Fhir.Model.Coding> CodingList = new List<Hl7.Fhir.Model.Coding>
            {
                coding
            };
            var code = new CodeableConcept
            {
                Coding = CodingList

            };
            ResourceReference reff = new ResourceReference
            {
                Reference = "Patient/099e7de7-c952-40e2-9b4e-0face78c9d80"
            };
            DiagnosticReport dr = new DiagnosticReport
            {
                Code = code,
                Status = DiagnosticReport.DiagnosticReportStatus.Final,
                Subject = reff,
            };
            //dr.ResourceType = "DiagnosticReport";
            return dr;
        }
    }
}