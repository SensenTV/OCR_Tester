# Implementierung

**Name:** OCR_Tester
**Sprache:** C#
**Art:** Konsolenanwendung

## Vergleichsmodelle

* **Tesseract OCR** – läuft lokal auf der Maschine.
* **GLM-OCR** – läuft auf dem KI-PC. 
* Die Kommunikation mit GLM-OCR erfolgt über eine fest konfigurierte bzw. hardcodierte Verbindung über OpenAI API

---

## Grundidee

Die Anwendung wird als Konsolenprogramm umgesetzt. Nach dem Start wird dem Benutzer ein Hauptmenü angezeigt:

1. **Vergleich starten**
2. **Beenden**

### 1. Vergleich starten

Wählt der Benutzer die Option **„Vergleich starten“**, wird zunächst nach dem Pfad zu einem Ordner gefragt.

In diesem Ordner befinden sich:

* die zu verarbeitenden Bilder im **BMP-Format**
* die zugehörigen **Ground-Truth-Daten** in einer JSON Datei

Eine eigenständige Extraktion der Ground Truth ist **nicht Bestandteil des Programms**.


Anschließend werden alle vorhandenen Bilder nacheinander von beiden OCR-Modellen verarbeitet:

* **Tesseract** <br> 
**TODO:**  Tesseract-Modus bzw. Page Segmentation Mode (PSM) festlegen



* **GLM-OCR** mit dem Prompt **„Text Recognition“**

Die erkannten Texte werden sowohl in der Konsole ausgegeben als auch zur späteren Auswertung in einer separaten Datei gespeichert.

Entscheidung über das Ausgabeformat:

* JSON für alle ergebnisse | results.json
* Text für die Zusammenfassung der Erbegnisse(Werden auch in konsole angezeigt) | console-summary.txt

---

## Vergleich und Auswertung

Die von den beiden OCR-Modellen erkannten Texte werden mit der jeweiligen Ground Truth verglichen.

Dabei sollen insbesondere folgende Werte ermittelt und festgehalten werden:

### Character Error Rate (CER)

Für jedes Bild wird ermittelt:

* Anzahl der falsch erkannten Zeichen
* Anzahl der fehlenden Zeichen
* ggf. Anzahl der zusätzlich erkannten Zeichen
* **Character Error Rate (CER)** in Prozent

Die Ergebnisse sollen sowohl für jedes einzelne Bild als auch für den gesamten Datensatz erfasst werden.

---

## Laufzeitmessung

Für beide OCR-Modelle wird die benötigte Verarbeitungszeit gemessen.

Erfasst werden:

* **Verarbeitungszeit pro Bild**
* **Gesamte Verarbeitungszeit für alle Bilder**
* **Durchschnittliche Verarbeitungszeit pro Bild**

Die Laufzeiten von Tesseract und GLM-OCR sollen anschließend miteinander verglichen werden.

---

## Kostenabschätzung

Zusätzlich soll, soweit möglich, eine Kostenabschätzung für die Nutzung der beiden Modelle erstellt werden.

### Tesseract

Da Tesseract lokal ausgeführt wird, sollen insbesondere die benötigten Hardware-Ressourcen berücksichtigt werden.

### GLM-OCR

Für GLM-OCR sollen – sofern verfügbar – folgende Werte erfasst werden:

* Anzahl der verwendeten Tokens
* geschätzte Kosten pro Bild
* Gesamtkosten für die Verarbeitung aller Bilder

Die Kostenberechnung soll auf den tatsächlich erfassten bzw. verfügbaren Daten basieren.

---

## Hardwarebedarf

Der Hardwarebedarf der beiden Systeme soll ebenfalls dokumentiert werden.

Dieser wird **nicht innerhalb des Programms ermittelt**, sondern als zusätzliche Information in der Ergebnisdatei festgehalten (Hardcoded).

Beispielsweise:

* verwendete CPU
* verwendete GPU
* RAM
* ggf. VRAM
* weitere relevante Hardwareinformationen

---

## Ergebnis

Am Ende eines Vergleichsdurchlaufs soll eine übersichtliche Ergebnisdatei erstellt werden, die mindestens folgende Informationen enthält:

| Kategorie             | Tesseract | GLM-OCR |
| --------------------- | --------- | ------- |
| Erkannter Text        | ✓         | ✓       |
| CER pro Bild          | ✓         | ✓       |
| Gesamte CER           | ✓         | ✓       |
| Zeit pro Bild         | ✓         | ✓       |
| Gesamtlaufzeit        | ✓         | ✓       |
| Durchschnittszeit     | ✓         | ✓       |
| Tokenanzahl           | –         |  ✓      |
| Kostenabschätzung     | ✓         | ✓       |
| Hardwareinformationen | ✓         | ✓       |

Die Ergebnisse sollen zusätzlich während der Verarbeitung bzw. nach Abschluss des Vergleichs übersichtlich in der Konsole dargestellt werden.

---

## Programmablauf

Der grundlegende Ablauf der Anwendung sieht somit folgendermaßen aus:

1. Programm starten
2. Hauptmenü anzeigen
3. **„Vergleich starten“** oder **„Beenden“** auswählen
4. Bei Auswahl von „Vergleich starten“:

   * Ordnerpfad abfragen
   * Bilder im BMP-Format einlesen
   * zugehörige Ground-Truth-Daten laden
5. Jedes Bild an Tesseract übergeben
6. Jedes Bild an GLM-OCR übergeben
7. Ergebnisse und Verarbeitungszeiten speichern
8. OCR-Ergebnisse mit der Ground Truth vergleichen
9. CER berechnen
10. Laufzeiten und ggf. Tokenverbrauch/Kosten auswerten
11. Ergebnisse in einer separaten Datei speichern
12. Zusammenfassung in der Konsole ausgeben
13. Zum Hauptmenü zurückkehren
14. Bei Auswahl von „Beenden“ das Programm schließen

---

## Offene Entscheidungen / TODOs

Vor der eigentlichen Implementierung müssen noch folgende Punkte festgelegt werden:

* [ ] Tesseract-Modus bzw. **Page Segmentation Mode (PSM)** festlegen
* [ ] Klären, wie die **Tokenanzahl** von GLM-OCR ermittelt wird
* [ ] Grundlage für die **Kostenberechnung** von GLM-OCR festlegen
* [ ] Festlegen, welche **Hardwareinformationen** in der Ergebnisdatei dokumentiert werden

```Text
OCR_Tester/
│
├── OCR_Tester.sln
│
├── src/
│   └── OCR_Tester/
│       │
│       ├── Program.cs
│       │
│       ├── Application/
│       │   ├── ComparisonService.cs
│       │   └── ComparisonRunner.cs
│       │
│       ├── Domain/
│       │   ├── Models/
│       │   │   ├── OcrResult.cs
│       │   │   ├── ComparisonResult.cs
│       │   │   ├── GroundTruth.cs
│       │   │   ├── ImageTestCase.cs
│       │   │   └── BenchmarkSummary.cs
│       │   │
│       │   └── Interfaces/
│       │       ├── IOcrEngine.cs
│       │       ├── IGroundTruthReader.cs
│       │       ├── IResultWriter.cs
│       │       └── IEvaluationService.cs
│       │
│       ├── OCR/
│       │   ├── Tesseract/
│       │   │   └── TesseractOcrEngine.cs
│       │   │
│       │   └── GLM/
│       │       └── GlmOcrEngine.cs
│       │
│       ├── Evaluation/
│       │   ├── CerCalculator.cs
│       │   └── ResultEvaluator.cs
│       │
│       ├── Input/
│       │   ├── GroundTruth/
│       │   │   ├── JsonGroundTruthReader.cs
│       │   │   └── CsvGroundTruthReader.cs
│       │   │
│       │   └── ImageLoader.cs
│       │
│       ├── Output/
│       │   ├── JsonResultWriter.cs
│       │   └── CsvResultWriter.cs
│       │
│       ├── Infrastructure/
│       │   ├── Configuration/
│       │   │   └── AppSettings.cs
│       │   │
│       │   ├── Timing/
│       │   │   └── BenchmarkTimer.cs
│       │   │
│       │   └── Hardware/
│       │       └── HardwareInfo.cs
│       │
│       ├── ConsoleUI/
│       │   ├── MainMenu.cs
│       │   ├── ComparisonMenu.cs
│       │   └── ConsoleFormatter.cs
│       │
│       └── Configuration/
│           └── appsettings.json
│
├── tests/
│   └── OCR_Tester.Tests/
│       ├── CerCalculatorTests.cs
│       ├── ResultEvaluatorTests.cs
│       └── GroundTruthReaderTests.cs
│
├── data/
│   └── example/
│       ├── images/
│       │   ├── image_001.bmp
│       │   ├── image_002.bmp
│       │   └── image_003.bmp
│       │
│       └── ground-truth.json
│
├── results/
│   └── .gitkeep
│
├── tessdata/
│   └── ...
│
├── .gitignore
└── README.md
```