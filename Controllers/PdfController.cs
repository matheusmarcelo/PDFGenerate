using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Mvc;
using PdfGenerate.Models;
using PdfGenerate.Service;

namespace PdfGenerate.Controllers
{
    [ApiController]
    [Route("v1/pdf")]
    public class PdfController : ControllerBase
    {
        private readonly PDFService _pdfService;
        static List<Pessoa> pessoas = new List<Pessoa>();

        public PdfController(PDFService pdfService)
        {
            _pdfService = pdfService;
        }

        // static void Deserializable()
        // {
        //     if (File.Exists("pessoas.json"))
        //     {
        //         using var sr = new StreamReader("pessoas.json");
        //         var dados = sr.ReadToEnd();
        //         pessoas = JsonSerializer.Deserialize(dados, typeof(List<Pessoa>)) as List<Pessoa>;
        //     }
        // }

        [HttpGet, Route("ola")]
        public ActionResult Ola()
        {
            return Ok("Ola");
        }


        [HttpGet, Route("generate")]
        public ActionResult GerarRelatorioEmPDF()
        {
            _pdfService.GerarRelatorioEmPDF(100);

            return Ok();
        }
    }
}