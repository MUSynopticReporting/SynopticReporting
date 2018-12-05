using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SmartFhirApplication.Models;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace SmartFhirApplication.Controllers
{
  public class HomeController : Controller
  {
    public IActionResult Index()
    {
      return View();
    }

    public IActionResult About()
    {
      ViewData["Message"] = "Your application description page.";

      return View();
    }

    public IActionResult Contact()
    {
      ViewData["Message"] = "Your contact page.";

      return View();
    }

    public IActionResult Privacy()
    {
      return View();
    }
    public IActionResult Launch()
    {
      return View();
    }
    public IActionResult LoadTemplate(string path)
    {
       return View("~/Views/Templates/"+path+".cshtml");
     // return View("Views/home/Contact.cshtml");
    }
    public IActionResult Nineteen()
    {
      return View();
    }
    public IActionResult Sixteen()
    {
      return View();
    }
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
      return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    public void CreateSingle(string title,
                       List<string> Procedure,
                       List<string> ClinicalInformation,
                       List<string> Comparison,
                       List<string> Findings,
                       List<string> Impression,
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
        Paragraph procedureParagraph = new Paragraph(Procedure[0], DefaultFont);
        Paragraph ciParagraph = new Paragraph(ClinicalInformation[0], DefaultFont);
        Paragraph comparisonParagraph = new Paragraph(Comparison[0], DefaultFont);
        Paragraph impressionParagraph = new Paragraph(Impression[0], DefaultFont);
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
            table.AddCell(Findings[j].Substring(0, index));
            table.AddCell(Findings[j].Substring(index + 2));
          }
          else
          {
            table.AddCell("");
            table.AddCell(Findings[j]);
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
                       List<string> Procedure,
                       List<string> ClinicalInformation,
                       List<string> Comparison,
                       List<List<string>> Findings,
                       List<string> Impression,
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
        Paragraph procedureParagraph = new Paragraph(Procedure[0], DefaultFont);
        Paragraph ciParagraph = new Paragraph(ClinicalInformation[0], DefaultFont);
        Paragraph comparisonParagraph = new Paragraph(Comparison[0], DefaultFont);
        Paragraph impressionParagraph = new Paragraph(Impression[0], DefaultFont);
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
        for (int i = 0; i < FindingsList.Count; i++)
        {
          Paragraph tempGraph = new Paragraph(FindingsList[i], SubtitleFont)
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
          for (int j = 0; j < Findings[i].Count; j++)
          {
            var index = Findings[i][j].IndexOf(": ");
            if (index != -1)
            {
              table.AddCell(Findings[i][j].Substring(0, index));
              table.AddCell(Findings[i][j].Substring(index + 2));
            }
            else
            {
              table.AddCell("");
              table.AddCell(Findings[i][j]);
            }

          }
          doc.Add(table);
        }

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
