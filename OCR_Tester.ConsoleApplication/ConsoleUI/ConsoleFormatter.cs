using OCR_Tester.Domain.Models;
using Spectre.Console;

namespace OCR_Tester.ConsoleUI
{
    /// <summary>
    /// Verantwortlich für die formatierte Darstellung von Informationen
    /// in der Konsolenanwendung.
    /// </summary>
    public class ConsoleFormatter
    {
        private const int Width = 70;

        /// <summary>
        /// Gibt eine große Überschrift aus.
        /// </summary>
        public void PrintHeader(string title)
        {
            AnsiConsole.Write(new Rule($"[bold cyan]{title}[/]").RuleStyle("cyan").Centered());

            AnsiConsole.WriteLine();
        }

        /// <summary>
        /// Gibt eine Erfolgsmeldung aus.
        /// </summary>
        public void PrintSuccess(string message)
        {
            AnsiConsole.MarkupLine($"[green]✔ {Markup.Escape(message)}[/]");
        }

        /// <summary>
        /// Gibt eine Fehlermeldung aus.
        /// </summary>
        public void PrintError(string message)
        {
            AnsiConsole.MarkupLine($"[red]✖ {Markup.Escape(message)}[/]");
        }

        /// <summary>
        /// Gibt eine Warnung aus.
        /// </summary>
        public void PrintWarning(string message)
        {
            AnsiConsole.MarkupLine($"[yellow]⚠ {Markup.Escape(message)}[/]");
        }

        /// <summary>
        /// Gibt eine Informationsmeldung aus.
        /// </summary>
        public void PrintInfo(string message)
        {
            AnsiConsole.MarkupLine($"[cyan]ℹ {Markup.Escape(message)}[/]");
        }

        /// <summary>
        /// Gibt eine Eingabeaufforderung aus.
        /// </summary>
        public void PrintPrompt(string message)
        {
            AnsiConsole.Markup($"[cyan]{Markup.Escape(message)}[/]");
        }

        /// <summary>
        /// Wartet auf eine Benutzereingabe.
        /// </summary>
        public void WaitForKey()
        {
            AnsiConsole.MarkupLine("[grey]Drücken Sie eine beliebige Taste, um fortzufahren...[/]");

            Console.ReadKey(true);
        }

        /// <summary>
        /// Gibt den Fortschritt der Bildverarbeitung aus.
        /// </summary>
        public void PrintProgress(int current, int total, string imageName)
        {
            AnsiConsole.MarkupLine(
                $"[grey][{current}/{total}][/] "
                    + $"[cyan]Verarbeite:[/] {Markup.Escape(imageName)}"
            );
        }

        /// <summary>
        /// Gibt das Ergebnis eines einzelnen OCR-Vergleichs aus.
        /// </summary>
        public void PrintComparisonResult(
            string modelName,
            string imageName,
            double processingTimeMs,
            double cerInPercent
        )
        {
            var table = new Table().Border(TableBorder.Rounded).BorderColor(Color.Cyan);

            table.AddColumn("[bold]Eigenschaft[/]");
            table.AddColumn("[bold]Wert[/]");

            table.AddRow("Modell", $"[cyan]{Markup.Escape(modelName)}[/]");

            table.AddRow("Bild", Markup.Escape(imageName));

            table.AddRow("Verarbeitungszeit", $"[yellow]{processingTimeMs:F2} ms[/]");

            table.AddRow("CER", GetCerMarkup(cerInPercent));

            AnsiConsole.Write(table);
            AnsiConsole.WriteLine();
        }

        /// <summary>
        /// Gibt eine allgemeine Statusbox aus.
        /// </summary>
        public void PrintStatus(string title, string message)
        {
            var panel = new Panel(new Markup(Markup.Escape(message)))
            {
                Header = new PanelHeader($"[bold cyan]{Markup.Escape(title)}[/]"),
                Border = BoxBorder.Rounded,
                BorderStyle = new Style(Color.Cyan),
                Padding = new Padding(2, 1),
            };

            AnsiConsole.Write(panel);
            AnsiConsole.WriteLine();
        }

        /// <summary>
        /// Gibt eine horizontale Trennlinie aus.
        /// </summary>
        public void PrintSeparator()
        {
            AnsiConsole.Write(new Rule().RuleStyle("grey"));
        }

        /// <summary>
        /// Gibt eine einfache Liste aus.
        /// </summary>
        public void PrintList(IEnumerable<string> items)
        {
            foreach (var item in items)
            {
                AnsiConsole.MarkupLine($"[cyan]•[/] {Markup.Escape(item)}");
            }

            AnsiConsole.WriteLine();
        }

        /// <summary>
        /// Gibt eine Startmeldung für einen Benchmark aus.
        /// </summary>
        public void PrintBenchmarkStart(int imageCount, int engineCount)
        {
            var panel = new Panel(
                new Markup(
                    $"[white]Bilder:[/] [cyan]{imageCount}[/]\n"
                        + $"[white]OCR-Engines:[/] [cyan]{engineCount}[/]\n"
                        + $"[white]Vergleiche:[/] [cyan]{imageCount * engineCount}[/]"
                )
            )
            {
                Header = new PanelHeader("[bold]Benchmark gestartet[/]"),
                Border = BoxBorder.Double,
                BorderStyle = new Style(Color.Green),
                Padding = new Padding(2, 1),
            };

            AnsiConsole.Write(panel);
            AnsiConsole.WriteLine();
        }

        /// <summary>
        /// Gibt eine Abschlussmeldung des Benchmarks aus.
        /// </summary>
        public void PrintBenchmarkFinished(string resultsDirectory)
        {
            var panel = new Panel(
                new Markup(
                    "[green]Der Benchmark wurde erfolgreich abgeschlossen.[/]\n\n"
                        + $"[grey]Ergebnisse:[/] {Markup.Escape(resultsDirectory)}"
                )
            )
            {
                Header = new PanelHeader("[bold green]Benchmark abgeschlossen[/]"),
                Border = BoxBorder.Double,
                BorderStyle = new Style(Color.Green),
                Padding = new Padding(2, 1),
            };

            AnsiConsole.Write(panel);
            AnsiConsole.WriteLine();
        }

        /// <summary>
        /// Gibt eine Tabelle mit den Benchmark-Ergebnissen aus.
        /// </summary>
        public void PrintSummary(IEnumerable<BenchmarkModelSummary> summaries)
        {
            var table = new Table()
                .Border(TableBorder.Rounded)
                .BorderColor(Color.Cyan)
                .Title("[bold cyan]OCR Benchmark - Ergebnisse[/]");

            table.AddColumn("Modell");
            table.AddColumn("Bilder");
            table.AddColumn("Gesamtzeit");
            table.AddColumn("Ø Zeit");
            table.AddColumn("Ø CER");
            table.AddColumn("Input Tokens");
            table.AddColumn("Output Tokens");

            foreach (var summary in summaries)
            {
                table.AddRow(
                    Markup.Escape(summary.ModelName),
                    summary.TotalImagesProcessed.ToString(),
                    $"{summary.TotalProcessingTimeMs} ms",
                    $"{summary.AverageProcessingTimeMs:F2} ms",
                    GetCerMarkup(summary.OverallCharacterErrorRate),
                    summary.TotalInputTokens.ToString(),
                    summary.TotalOutputTokens.ToString()
                );
            }

            AnsiConsole.Write(table);
            AnsiConsole.WriteLine();
        }

        /// <summary>
        /// Gibt einen farblich passenden CER-Wert zurück.
        /// </summary>
        private static string GetCerMarkup(double cer)
        {
            if (cer <= 5)
            {
                return $"[green]{cer:F2} %[/]";
            }

            if (cer <= 15)
            {
                return $"[yellow]{cer:F2} %[/]";
            }

            return $"[red]{cer:F2} %[/]";
        }
    }
}
