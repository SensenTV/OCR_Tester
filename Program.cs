using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OCR_Tester
{
    // Einstiegspunkt der OCR_Tester-Anwendung.
    // Initialisiert die benötigten Komponenten und startet das Hauptmenü.
    internal class Program
    {
        static void Main(string[] args)
        {
            var _MainMenu = new ConsoleUI.MainMenu();
            _MainMenu.ShowAsync().Wait();
        }
    }
}
