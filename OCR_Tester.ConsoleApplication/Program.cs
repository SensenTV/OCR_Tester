namespace OCR_Tester
{
    // Einstiegspunkt der OCR_Tester-Anwendung.
    // Initialisiert die benötigten Komponenten und startet das Hauptmenü.
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var _MainMenu = new ConsoleUI.MainMenu();
            await _MainMenu.ShowAsync();
        }
    }
}
