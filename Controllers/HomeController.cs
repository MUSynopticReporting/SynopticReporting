using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using SmartFhirApplication.Models;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace SmartFhirApplication.Controllers
{
  public class HomeController : Microsoft.AspNetCore.Mvc.Controller
  {
    public IActionResult Index()
    {
      return View();
    }

    public IActionResult Launch()
    {
      return View();
    }
    public IActionResult LoadTemplate(string path)
    {
      //var whatever = @Html.Raw(File.ReadAllText(Server.MapPath("~/Views/Home/_TSCalendar.html")));
      //return View("~/Views/Templates/" + path + ".html", "text/html");
        return View("~/Views/Templates/" + path + ".cshtml");
      // return View("Views/home/Contact.cshtml");
    }
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
      return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }


    public void PDFSingle(string title,
                       List<List<string>> Procedure,
                       List<List<string>> ClinicalInformation,
                       List<List<string>> Comparison,
                       List<List<string>> Findings,
                       List<List<string>> Impression,
                       List<string> FindingsList)
    {
      using (FileStream fs = new FileStream(
      "results/" + title + " for Patient_01.pdf", FileMode.Create, FileAccess.Write, FileShare.None))
      using (Document doc = new Document(PageSize.LETTER))
      using (PdfWriter writer = PdfWriter.GetInstance(doc, fs))
      {
        var TitleFont = new Font(Font.FontFamily.COURIER, 18f, Font.NORMAL);
        var DefaultFont = new Font(Font.FontFamily.COURIER, 11f, Font.NORMAL);
        var SubtitleFont = new Font(Font.FontFamily.COURIER, 14f, Font.NORMAL);
        doc.Open();
        Paragraph titleParagraph = new Paragraph(title, TitleFont);
        Paragraph procedureParagraph = new Paragraph(Procedure[0][0], DefaultFont);
        Paragraph ciParagraph = new Paragraph(ClinicalInformation[0][0], DefaultFont);
        Paragraph comparisonParagraph = new Paragraph(Comparison[0][0], DefaultFont);
        Paragraph impressionParagraph = new Paragraph(Impression[0][0], DefaultFont);
        Paragraph findingsParagraph = new Paragraph("Findings:", DefaultFont);
        // Setting paragraph's text alignment using iTextSharp.text.Element class
        titleParagraph.Alignment = Element.ALIGN_CENTER;
        procedureParagraph.Alignment = Element.ALIGN_JUSTIFIED;
        ciParagraph.Alignment = Element.ALIGN_JUSTIFIED;
        // Adding this 'para' to the Document object
        doc.Add(titleParagraph);
        doc.Add(procedureParagraph);
        doc.Add(ciParagraph);
        doc.Add(comparisonParagraph);
        Paragraph tempGraph = new Paragraph("Findings", SubtitleFont)
        {
          SpacingAfter = 2
        };
        doc.Add(tempGraph);
        doc.Add(new Paragraph("\n"));
        PdfPTable table = new PdfPTable(2)
        {
          SpacingAfter = 3
        };
        PdfPCell cat = new PdfPCell(new Phrase("Category"));
        cat.BackgroundColor = new BaseColor(211, 211, 211);
        PdfPCell fin = new PdfPCell(new Phrase("Finding"));
        fin.BackgroundColor = new BaseColor(211, 211, 211);
        table.AddCell(cat);
        table.AddCell(fin);
        for (int j = 0; j < Findings.Count; j++)
        {
          var index = Findings[j].IndexOf(": ");
          if (index != -1)
          {
            table.AddCell(Findings[j][0].Substring(0, index));
            table.AddCell(Findings[j][0].Substring(index + 2));
          }
          else
          {
            table.AddCell("");
            table.AddCell(Findings[j][0]);
          }

        }
        doc.Add(table);

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



    public void Create(string title,
                       TemplateViewModel Procedure,
                       TemplateViewModel ClinicalInformation,
                       TemplateViewModel Comparison,
                       TemplateViewModel Findings,
                       TemplateViewModel Impression,
                       List<string> FindingsList,
                       string Location)
    {
      CreatePDF(title, Procedure, ClinicalInformation, Comparison, Findings, Impression, FindingsList);

    }
    public void CreatePDF(string title,
                       TemplateViewModel Procedure,
                       TemplateViewModel ClinicalInformation,
                       TemplateViewModel Comparison,
                       TemplateViewModel Findings,
                       TemplateViewModel Impression,
                       List<string> FindingsList)
    {

      using (FileStream fs = new FileStream(
      "results/" + title + " for Patient_01.pdf", FileMode.Create, FileAccess.Write, FileShare.None))
      using (Document doc = new Document(PageSize.LETTER))
      using (PdfWriter writer = PdfWriter.GetInstance(doc, fs))
      {
        var TitleFont = new Font(Font.FontFamily.COURIER, 18f, Font.NORMAL);
        var DefaultFont = new Font(Font.FontFamily.COURIER, 11f, Font.NORMAL);
        var SubtitleFont = new Font(Font.FontFamily.COURIER, 14f, Font.NORMAL);
        doc.Open();
        Paragraph titleParagraph = new Paragraph(title, TitleFont);
        Paragraph procedureParagraph = ParagraphConstruct(Procedure);
        Paragraph ciParagraph = ParagraphConstruct(ClinicalInformation);
        Paragraph comparisonParagraph = ParagraphConstruct(Comparison);
        Paragraph impressionParagraph = ParagraphConstruct(Impression);
        Paragraph findingsParagraph = ParagraphConstruct(Findings);
        // Setting paragraph's text alignment using iTextSharp.text.Element class
        titleParagraph.Alignment = Element.ALIGN_CENTER;
        procedureParagraph.Alignment = Element.ALIGN_JUSTIFIED;
        ciParagraph.Alignment = Element.ALIGN_JUSTIFIED;
        // Adding this 'para' to the Document object
        doc.Add(titleParagraph);
        doc.Add(procedureParagraph);
        doc.Add(ciParagraph);
        doc.Add(comparisonParagraph);
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
      string workingDirectory = Environment.CurrentDirectory;
      string URL = workingDirectory + "/Views/Templates/" + path + ".cshtml";
      List<string> allLines = new List<string>(System.IO.File.ReadAllLines(URL));
      var usefulLines = new List<(string, string, string)>();
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


    private Paragraph ParagraphConstruct(TemplateViewModel node)
    {
      var defaultFont = new Font(Font.FontFamily.COURIER, 11f, Font.NORMAL);
      var output = new Paragraph(node.Header, defaultFont);
      if (node.IsLeaf)
      {
        //Not sure how to handle this lol
      }
      else
      {
        //PdfPTable table = new PdfPTable(2);
        // Create a table for all the things if they're leaves
        // Otherwise recurse bcz we want to group the tables
        if (node.ChildNodes[0].IsLeaf)
        {
          //Make a table to add
          PdfPTable table = Tablify(node);
          output.Add(table);
        }
        else
        {
          for (int i = 0; i < node.ChildNodes.Count; i++)
          {
            output.Add(ParagraphConstruct(node.ChildNodes[i]));
          }

        }

      }
      return output;
    }

    private PdfPTable Tablify(TemplateViewModel node)
    {
      PdfPTable output = new PdfPTable(2)
      {
        SpacingAfter = 3
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
      for(int i = 0; i < node.ChildNodes.Count; i++)
      {
        output.AddCell(node.Header);
        output.AddCell(node.Result);
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
