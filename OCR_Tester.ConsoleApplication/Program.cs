using Serilog;

namespace OCR_Tester
{
    // Einstiegspunkt der OCR_Tester-Anwendung.
    // Initialisiert die benötigten Komponenten und startet das Hauptmenü.
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.File(
                    Path.Combine(AppContext.BaseDirectory, "logs/log.txt"),
                    rollingInterval: RollingInterval.Day,
                    retainedFileCountLimit: 7
                )
                .CreateLogger();

            var _MainMenu = new ConsoleUI.MainMenu();
            await _MainMenu.ShowAsync();
        }
    }
}
