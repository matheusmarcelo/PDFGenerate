using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata;
using System.Text.Json;
using System.Threading.Tasks;
using iTextSharp.text;
using iTextSharp.text.pdf;
using PdfGenerate.Models;
using Document = iTextSharp.text.Document;

namespace PdfGenerate.Service
{
    public class PDFService
    {
        static List<Pessoa> pessoas = new List<Pessoa>();
        static BaseFont fonteBase = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, false);
        public PDFService()
        {
        }


        static void Deserializable()
        {
            if (File.Exists("pessoas.json"))
            {
                using var sr = new StreamReader("pessoas.json");
                var dados = sr.ReadToEnd();
                pessoas = JsonSerializer.Deserialize(dados, typeof(List<Pessoa>)) as List<Pessoa>;
            }
        }


        public void GerarRelatorioEmPDF(int qtdePessoas)
        {
            Deserializable();
            var pessoaSelecioanada = pessoas.Take(qtdePessoas).ToList();
            if (pessoaSelecioanada.Count > 0)
            {
                // Configuração do documento pdf
                var pxPorMm = 72 / 25.2F;
                var pdf = new Document(PageSize.A4, 15 * pxPorMm, 15 * pxPorMm, 15 * pxPorMm, 20 * pxPorMm);
                var nomeArquivo = $"pessoas.{DateTime.Now.ToString("yyyy.MM.dd.HH.mm.ss")}.pdf";
                var arquivo = new FileStream($"C:\\ProjetosProgramação\\APIs com ASP.NET\\PdfGenerate\\bin\\Debug\\net8.0\\{nomeArquivo}", FileMode.Create);
                var writer = PdfWriter.GetInstance(pdf, arquivo);
                pdf.Open();

                // adição do título
                var fonteParagrafo = new iTextSharp.text.Font(fonteBase, 32, iTextSharp.text.Font.NORMAL, BaseColor.Black);
                var titulo = new Paragraph("Relatório de passoas\n\n", fonteParagrafo);
                titulo.Alignment = Element.ALIGN_LEFT;
                titulo.SpacingAfter = 4;
                pdf.Add(titulo);

                // adição de imagem
                var caminhoImagem = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "img\\youtube.png");
                if (File.Exists(caminhoImagem))
                {
                    iTextSharp.text.Image logo = iTextSharp.text.Image.GetInstance(caminhoImagem);
                    float razaoAlturaLargura = logo.Width / logo.Height;
                    float alturaLogo = 32;
                    float larguraLogo = alturaLogo * razaoAlturaLargura;
                    logo.ScaleToFit(larguraLogo, alturaLogo);
                    var margemEsquerda = pdf.PageSize.Height - pdf.RightMargin - larguraLogo;
                    var margemTopo = pdf.PageSize.Height - pdf.TopMargin - 54;
                    logo.SetAbsolutePosition(margemEsquerda, margemTopo);
                    writer.DirectContent.AddImage(logo, false);
                }

                // adição de tabela
                var tabela = new PdfPTable(5);
                float[] largurasColunas = { 0.6f, 2f, 1.5f, 1f, 1f };
                tabela.SetWidths(largurasColunas);
                tabela.DefaultCell.BorderWidth = 0;
                tabela.WidthPercentage = 100;

                // Adição das celulas de titulos das colunas
                MontarCelulaTexto(tabela, "Código", PdfCell.ALIGN_CENTER, true);
                MontarCelulaTexto(tabela, "Nome", PdfCell.ALIGN_LEFT, true);
                MontarCelulaTexto(tabela, "Profissão", PdfCell.ALIGN_CENTER, true);
                MontarCelulaTexto(tabela, "Salário", PdfCell.ALIGN_CENTER, true);
                MontarCelulaTexto(tabela, "Empregada", PdfCell.ALIGN_CENTER, true);

                foreach (var p in pessoas)
                {
                    MontarCelulaTexto(tabela, p.IdPessoa.ToString("D6"), PdfPCell.ALIGN_CENTER);
                    MontarCelulaTexto(tabela, p.Nome + " " + p.Sobrenome);
                    MontarCelulaTexto(tabela, p.Profissao.Nome, PdfPCell.ALIGN_CENTER);
                    MontarCelulaTexto(tabela, p.Salario.ToString("C2"), PdfPCell.ALIGN_RIGHT);
                    // MontarCelulaTexto(tabela, p.Empregado ? "Sim" : "Não", PdfPCell.ALIGN_CENTER);
                    var caminhoImagemCelula = p.Empregado ? "img\\emoji_feliz.png" : "img\\emoji_triste.png";
                    caminhoImagemCelula = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, caminhoImagemCelula);
                    CriarCelulaImagem(tabela, caminhoImagemCelula, 20, 20);
                }

                pdf.Add(tabela);

                pdf.Close();
                arquivo.Close();

                // abre o PDF no visualizador padrão
                // var caminhoPDF = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, nomeArquivo);
                // if (File.Exists(caminhoPDF))
                // {
                //     Process.Start(new ProcessStartInfo()
                //     {
                //         Arguments = $"/c start chrome {caminhoPDF}",
                //         FileName = "cmd.exe",
                //         CreateNoWindow = true
                //     });
                // }
            }
        }

        static void MontarCelulaTexto(PdfPTable tabela, string texto, int alinhamentoHorz = PdfPCell.ALIGN_LEFT, bool negrito = false, bool italico = false, int tamanhoFonte = 12, int alturaCelula = 25)
        {
            int estilo = iTextSharp.text.Font.NORMAL;
            if (negrito && italico)
            {
                estilo = Font.BOLDITALIC;
            }
            else if (negrito)
            {
                estilo = Font.BOLD;
            }
            else if (italico)
            {
                estilo = Font.ITALIC;
            }

            var bgColor = BaseColor.White;
            if (tabela.Rows.Count % 2 == 1)
            {
                bgColor = new BaseColor(0.95F, 0.95F, 0.95F);
            }

            var fonteCelula = new Font(fonteBase, tamanhoFonte, estilo, BaseColor.Black);
            var celula = new PdfPCell(new Phrase(texto, fonteCelula));
            celula.HorizontalAlignment = alinhamentoHorz;
            celula.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
            celula.Border = 0;
            celula.BorderWidthBottom = 1;
            celula.FixedHeight = alturaCelula;
            celula.PaddingBottom = 5;
            celula.BackgroundColor = bgColor;
            tabela.AddCell(celula);
        }

        static void CriarCelulaImagem(PdfPTable tabela, string caminhoImagem, int larguraImagem, int alturaImagem, int alturaCelula = 25)
        {
            var bgColor = BaseColor.White;
            if (tabela.Rows.Count % 2 == 1)
            {
                bgColor = new BaseColor(0.95F, 0.95F, 0.95F);
            }

            if (File.Exists(caminhoImagem))
            {
                Image imagem = Image.GetInstance(caminhoImagem);
                imagem.ScaleToFit(larguraImagem, alturaImagem);

                var celula = new PdfPCell(imagem);
                celula.HorizontalAlignment = PdfCell.ALIGN_CENTER;
                celula.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                celula.Border = 0;
                celula.BorderWidthBottom = 1;
                celula.FixedHeight = alturaCelula;
                celula.BackgroundColor = bgColor;
                tabela.AddCell(celula);
            }
        }
    }
}