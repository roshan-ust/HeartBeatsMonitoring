using System.Collections;
using System.IO;
using System.Windows.Controls;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using HeartBeats.Models;

namespace HeartBeats.Utils
{
    public static class WordDocumentExporter
    {
        public static void ExportHeartBeatToWord(IEnumerable itemsSource, string outputPath)
        {
            using (WordprocessingDocument doc = WordprocessingDocument.Create(outputPath, WordprocessingDocumentType.Document))
            {
                MainDocumentPart mainPart = doc.AddMainDocumentPart();
                mainPart.Document = new Document();
                Body body = mainPart.Document.AppendChild(new Body());

                // Add the table
                Table table = new Table();
                TableProperties tableProperties = new TableProperties(
                    new TableBorders(
                        new TopBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 4 },
                        new BottomBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 4 },
                        new LeftBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 4 },
                        new RightBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 4 },
                        new InsideHorizontalBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 4 },
                        new InsideVerticalBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 4 })
                );
                table.AppendChild(tableProperties);

                // Add the title row
                TableRow titleRow = new TableRow();
                TableCell titleCell = GenerateCell("Heartbeats Report", true, 12);
                TableCellProperties titleCellProperties = new TableCellProperties(new GridSpan { Val = 4 });
                titleCell.AppendChild(titleCellProperties);
                titleRow.AppendChild(titleCell);
                table.AppendChild(titleRow);

                // Add table header row
                TableRow headerRow = new TableRow();
                AddHeaderCell(headerRow, "Date (EST)");
                AddHeaderCell(headerRow, "Name");
                AddHeaderCell(headerRow, "Message");
                AddHeaderCell(headerRow, "Comments");
                table.AppendChild(headerRow);

                // Add table data rows
                foreach (var item in itemsSource)
                {
                    if (item is HeartBeatItem heartbeat)
                    {
                        TableRow dataRow = new TableRow();
                        AddDataCell(dataRow, heartbeat.Date.ToString());
                        AddDataCell(dataRow, heartbeat.Name);
                        AddDataCell(dataRow, heartbeat.Message);
                        AddDataCell(dataRow, heartbeat.Status);
                        table.AppendChild(dataRow);
                    }
                }

                body.Append(table);
                mainPart.Document.Save();
            }
        }

        private static void AddHeaderCell(TableRow row, string text, int fontSize = 10)
        {
            row.AppendChild(GenerateCell(text, true, fontSize));
        }

        private static void AddDataCell(TableRow row, string text, int fontSize = 10)
        {
            row.AppendChild(GenerateCell(text, false, fontSize));
        }

        private static TableCell GenerateCell(string text, bool isHeader = false, int fontSize = 10)
        {
            TableCell cell = new TableCell();
            Paragraph paragraph = new Paragraph();
            Run run = new Run();

            RunProperties runProperties = new RunProperties();
            if (isHeader)
            {
                Bold bold = new Bold();
                runProperties.Append(bold);
            }

            FontSize size = new FontSize() { Val = (fontSize * 2).ToString() }; // FontSize is in half-points
            runProperties.Append(size);

            run.Append(runProperties);
            run.Append(new Text(text));
            paragraph.Append(run);
            cell.Append(paragraph);

            TableCellProperties cellProperties = new TableCellProperties(
                new TableCellWidth { Type = TableWidthUnitValues.Dxa, Width = "2400" }, // Adjust width as needed
                new TableCellBorders(
                    new TopBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 4 },
                    new BottomBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 4 },
                    new LeftBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 4 },
                    new RightBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 4 }),
                new TableCellMargin(
                    new LeftMargin { Width = "100", Type = TableWidthUnitValues.Dxa }, // Adjust padding as needed
                    new RightMargin { Width = "100", Type = TableWidthUnitValues.Dxa },
                    new TopMargin { Width = "100", Type = TableWidthUnitValues.Dxa },
                    new BottomMargin { Width = "100", Type = TableWidthUnitValues.Dxa })
            );
            cell.AppendChild(cellProperties);

            return cell;
        }
    } 
}
