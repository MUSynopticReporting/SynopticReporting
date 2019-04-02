using iTextSharp.text;
using NUnit;
using NUnit.Framework;
using SmartFhirApplication.Controllers;
using SmartFhirApplication.Models;

namespace SmartFhirApplication.Testing
{
    [TestFixture]
    public class HomeTest
    {
        [Test]
        public void NullTablify()
        {
            TemplateViewModel asdf = new TemplateViewModel();
            HomeController control = new HomeController();
            var a = control.CreatePDF("Null", asdf, asdf, asdf, asdf, asdf, asdf);
            Assert.IsNotNull(a);
        }
    }
}
