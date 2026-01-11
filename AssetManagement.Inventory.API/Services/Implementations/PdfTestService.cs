using iText.Kernel.Exceptions;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;

namespace AssetManagement.Inventory.API.Services.Implementations
{
    public class PdfTestService
    {
        public void GenerateTestPdf()
        {
            try
            {
                // Caminho do PDF de teste
                string outputPath = Path.Combine(Directory.GetCurrentDirectory(), "teste.pdf");

                // Cria o PdfWriter
                using (PdfWriter writer = new PdfWriter(outputPath))
                {
                    // Cria o PdfDocument
                    using (PdfDocument pdf = new PdfDocument(writer))
                    {
                        // Cria o documento Layout
                        Document document = new Document(pdf);

                        // Adiciona um parágrafo de teste
                        document.Add(new Paragraph("PDF gerado com sucesso usando iText7!"));

                        document.Close();
                    }
                }

                Console.WriteLine($"PDF gerado em: {outputPath}");
            }
            catch (PdfException ex)
            {
                Console.WriteLine("Erro ao gerar PDF: " + ex.Message);
            }
            catch (NotSupportedException ex)
            {
                Console.WriteLine("Erro de dependência: " + ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erro inesperado: " + ex.Message);
            }
        }
    }
}
