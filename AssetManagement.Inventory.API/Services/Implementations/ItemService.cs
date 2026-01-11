using AssetManagement.Inventory.API.Domain.Entities;
using AssetManagement.Inventory.API.DTOs.Item;
using AssetManagement.Inventory.API.Exceptions;
using AssetManagement.Inventory.API.Infrastructure.Data;
using AssetManagement.Inventory.API.Services.Interfaces;
using ClosedXML.Excel;
using Microsoft.EntityFrameworkCore;
using iText.Layout;
using iText.Kernel.Pdf;
using iText.Layout.Element;
using System.IO;
using iText.Layout.Properties;
using iText.IO.Font;
using iText.IO.Image;
using iText.Kernel.Colors;
using iText.Kernel.Font;


namespace AssetManagement.Inventory.API.Services.Implementations
{
    public class ItemService : IItemService
    {
        private readonly InventoryDbContext _context;
        private readonly IWebHostEnvironment _env;

        public ItemService(InventoryDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public async Task<ItemResponseDto> CreateAsync(CreateItemDto dto)
        {
            var area = await _context.Areas
                .FirstOrDefaultAsync(a => a.Id == dto.AreaId);

            if (area == null)
                throw new Exception("Área informada não existe.");

            var exists = await _context.Items.AnyAsync(i =>
                i.Name.ToLower() == dto.Name.ToLower() &&
                i.AreaId == dto.AreaId);

            if (exists)
                throw new Exception("Item já cadastrado nesta área.");

            var item = new Item
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                Description = dto.Description,
                Quantity = dto.Quantity,
                AreaId = dto.AreaId,
                ValorMedio = dto.ValorMedio,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Items.Add(item);
            await _context.SaveChangesAsync();

            return new ItemResponseDto
            {
                Id = item.Id,
                Name = item.Name,
                Description = item.Description,
                Quantity = item.Quantity,
                AreaId = area.Id,
                AreaName = area.Name,
                ValorMedio = item.ValorMedio,
                CreatedAt = item.CreatedAt,
                UpdatedAt = item.UpdatedAt
            };
        }

        public async Task<IEnumerable<ItemResponseDto>> GetAllAsync()
        {
            return await _context.Items
                .Include(i => i.Area)
                .Select(i => new ItemResponseDto
                {
                    Id = i.Id,
                    Name = i.Name,
                    Description = i.Description,
                    Quantity = i.Quantity,
                    AreaId = i.AreaId,
                    AreaName = i.Area.Name,
                    ValorMedio = i.ValorMedio,
                    NotaFiscalCaminho = i.NotaFiscalCaminho,
                    CreatedAt = i.CreatedAt,
                    UpdatedAt = i.UpdatedAt
                })
                .ToListAsync();
        }

        public async Task<ItemResponseDto?> GetByIdAsync(Guid id)
        {
            return await _context.Items
                .Include(i => i.Area)
                .Where(i => i.Id == id)
                .Select(i => new ItemResponseDto
                {
                    Id = i.Id,
                    Name = i.Name,
                    Description = i.Description,
                    Quantity = i.Quantity,
                    AreaId = i.AreaId,
                    AreaName = i.Area.Name,
                    ValorMedio = i.ValorMedio,
                    NotaFiscalCaminho = i.NotaFiscalCaminho,
                    CreatedAt = i.CreatedAt,
                    UpdatedAt = i.UpdatedAt
                })
                .FirstOrDefaultAsync();
        }

        public async Task<string> UploadNotaFiscalAsync(Guid itemId, IFormFile file)
        {
            var item = await _context.Items.FindAsync(itemId);

            if (item == null)
                throw new AppException("Item não encontrado.", 404);

            var dir = Path.Combine(_env.WebRootPath, "uploads", "invoices");

            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            var fileName = $"{Guid.NewGuid()}_{file.FileName}";
            var filePath = Path.Combine(dir, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
                await file.CopyToAsync(stream);

            item.NotaFiscalCaminho = $"/uploads/invoices/{fileName}";

            await _context.SaveChangesAsync();

            return item.NotaFiscalCaminho;
        }

        public async Task<byte[]> ExportExcelAsync()
        {
            var items = await _context.Items
                .Include(i => i.Area)
                .ToListAsync();

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Lista_de_Itens");

                // === CONFIGURAÇÕES INICIAIS ===
                worksheet.SheetView.FreezeRows(1); // Congela cabeçalho

                // === CABEÇALHO ===
                worksheet.Cell(1, 1).Value = "Item";
                worksheet.Cell(1, 2).Value = "Descrição";
                worksheet.Cell(1, 3).Value = "Quantidade";
                worksheet.Cell(1, 4).Value = "Área";
                worksheet.Cell(1, 5).Value = "Criado em";
                worksheet.Cell(1, 6).Value = "Atualizado em";

                var headerRange = worksheet.Range("A1:F1");
                headerRange.Style.Font.Bold = true;
                headerRange.Style.Fill.BackgroundColor = XLColor.FromHtml("#1F4E78"); // Azul corporativo
                headerRange.Style.Font.FontColor = XLColor.White;
                headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                // === DADOS ===
                int row = 2;
                foreach (var item in items)
                {
                    var range = worksheet.Range($"A{row}:F{row}");

                    worksheet.Cell(row, 1).Value = item.Name;
                    worksheet.Cell(row, 2).Value = item.Description ?? "-";
                    worksheet.Cell(row, 3).Value = item.Quantity;
                    worksheet.Cell(row, 4).Value = item.Area?.Name ?? "-";

                    worksheet.Cell(row, 5).Value = item.CreatedAt;
                    worksheet.Cell(row, 5).Style.DateFormat.Format = "dd/MM/yyyy HH:mm";

                    worksheet.Cell(row, 6).Value = item.UpdatedAt;
                    worksheet.Cell(row, 6).Style.DateFormat.Format = "dd/MM/yyyy HH:mm";

                    // Estilo zebra nas linhas
                    if (row % 2 == 0)
                        range.Style.Fill.BackgroundColor = XLColor.FromHtml("#F2F2F2"); // Cinza claro

                    row++;
                }

                // === BORDAS NA TABELA ===
                var tableRange = worksheet.Range($"A1:F{row - 1}");
                tableRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                tableRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

                // === AUTO AJUSTAR COLUNAS ===
                worksheet.Columns().AdjustToContents();

                // === FILTRO AUTOMÁTICO ===
                worksheet.Range("A1:F1").SetAutoFilter();

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    return stream.ToArray();
                }
            }
        }

        public async Task<byte[]> ExportPdfAsync()
        {
            var areas = await _context.Areas
                .Include(a => a.Items)
                .OrderBy(a => a.Name)
                .ToListAsync();

            var stream = new MemoryStream();

            var writer = new PdfWriter(stream);
            var pdf = new PdfDocument(writer);
            var document = new Document(pdf);

            // ===== Fonte Unicode =====
            var fontPath = Path.Combine(_env.WebRootPath, "fonts", "DejaVuSans.ttf");
            if (!File.Exists(fontPath))
                throw new Exception("Fonte PDF não encontrada em wwwroot/fonts.");

            var font = PdfFontFactory.CreateFont(
                fontPath,
                PdfEncodings.IDENTITY_H,
                PdfFontFactory.EmbeddingStrategy.PREFER_EMBEDDED
            );
            document.SetFont(font);

            // ===== Logo =====
            var logoPath = Path.Combine(_env.WebRootPath, "uploads", "logo", "marca.png");
            if (File.Exists(logoPath))
            {
                var logo = new Image(ImageDataFactory.Create(logoPath))
                    .SetWidth(90)
                    .SetHorizontalAlignment(HorizontalAlignment.CENTER)
                    .SetMarginBottom(10);

                document.Add(logo);
            }

            // ===== Título =====
            document.Add(
                new Paragraph("Inventário Patrimonial")
                    .SetFontSize(20)
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetBold()
                    .SetMarginBottom(20)
            );

            // ===== Conteúdo =====
            foreach (var area in areas)
            {
                // Quebra de página entre áreas (exceto a primeira)
                //if (pdf.GetNumberOfPages() > 0)
                   // document.Add(new AreaBreak());

                document.Add(
                    new Paragraph($"Área: {area.Name}")
                        .SetFontSize(16)
                        .SetBold()
                        .SetMarginBottom(10)
                );

                var items = area.Items?.OrderBy(i => i.Name).ToList();

                if (items == null || !items.Any())
                {
                    document.Add(
                        new Paragraph("(Nenhum item cadastrado)")
                            .SetItalic()
                            .SetMarginBottom(15)
                    );
                    continue;
                }

                // ===== Tabela =====
                var table = new Table(new float[] { 3, 4, 2, 2 })
                    .UseAllAvailableWidth();

                // ==== Estilo do cabeçalho ====
                void AddHeaderCell(string text)
                {
                    var cell = new Cell()
                        .Add(new Paragraph(text)
                            .SetBold()
                            .SetFontSize(11)
                            .SetTextAlignment(TextAlignment.CENTER))
                        .SetBackgroundColor(ColorConstants.LIGHT_GRAY)
                        .SetVerticalAlignment(VerticalAlignment.MIDDLE)
                        .SetPaddingTop(5)
                        .SetPaddingBottom(5)
                        .SetPaddingLeft(4)
                        .SetPaddingRight(4)
                        .SetMinHeight(12);

                    table.AddHeaderCell(cell);
                }

                AddHeaderCell("Item");
                AddHeaderCell("Descrição");
                AddHeaderCell("Quantidade");
                AddHeaderCell("Valor Médio");

                // ==== Estilo das linhas do corpo ====
                foreach (var item in items)
                {
                    table.AddCell(
                        new Cell()
                            .Add(new Paragraph(item.Name ?? "-")
                                .SetFontSize(9))
                            .SetPadding(4)
                    );

                    table.AddCell(
                        new Cell()
                            .Add(new Paragraph(item.Description ?? "-")
                                .SetFontSize(9))
                            .SetPadding(4)
                    );

                    table.AddCell(
                        new Cell()
                            .Add(new Paragraph(item.Quantity.ToString())
                                .SetFontSize(9))
                            .SetTextAlignment(TextAlignment.CENTER)
                            .SetPadding(4)
                    );

                    table.AddCell(
                        new Cell()
                            .Add(new Paragraph(item.ValorMedio?.ToString("C") ?? "-")
                                .SetFontSize(9))
                            .SetTextAlignment(TextAlignment.CENTER)
                            .SetPadding(4)
                    );
                }

                document.Add(table);

            }

            document.Close();
            return stream.ToArray();
        }
    }
}
