# OCR_Tester

`OCR_Tester` ist eine C#-Konsolenanwendung zum **Vergleichen und Benchmarken verschiedener OCR-Engines**.

Das Projekt wurde mit dem Ziel entwickelt, unterschiedliche OCR-Modelle und -Engines anhand eines einheitlichen Testdatensatzes objektiv miteinander vergleichen zu können.

Neben der Erkennung des Textes werden unter anderem folgende Kennzahlen erfasst:

* erkannter Text
* Verarbeitungszeit
* Character Error Rate (CER)
* Anzahl der Zeichenfehler
* Input Tokens
* Output Tokens
* Gesamtzahl der Tokens
* verwendete GPU
* zusammengefasste Benchmark-Ergebnisse pro OCR-Modell

Das Projekt ist modular aufgebaut, sodass weitere OCR-Engines möglichst einfach integriert werden können.

---

## Inhaltsverzeichnis

- [OCR\_Tester](#ocr_tester)
  - [Inhaltsverzeichnis](#inhaltsverzeichnis)
- [Ziele](#ziele)
    - [1. Vergleichbarkeit](#1-vergleichbarkeit)
    - [2. Objektive Bewertung](#2-objektive-bewertung)
    - [3. Erweiterbarkeit](#3-erweiterbarkeit)
    - [4. Konfigurierbarkeit](#4-konfigurierbarkeit)
    - [5. Reproduzierbare Benchmarks](#5-reproduzierbare-benchmarks)
    - [6. Trennung der Verantwortlichkeiten](#6-trennung-der-verantwortlichkeiten)
- [Features](#features)
- [Technologien](#technologien)
- [Architektur](#architektur)
- [OCR-Abstraktion](#ocr-abstraktion)
- [Unterstützte OCR-Engines](#unterstützte-ocr-engines)
  - [Tesseract](#tesseract)
  - [GLM-OCR](#glm-ocr)
- [Konfiguration](#konfiguration)
- [OCR Engine Factory](#ocr-engine-factory)
- [Testdaten](#testdaten)
- [Ground Truth](#ground-truth)
- [ImageTestCase](#imagetestcase)
- [Character Error Rate](#character-error-rate)
- [CER Calculator](#cer-calculator)
- [Benchmark Summary](#benchmark-summary)
- [Einzelnes OCR-Ergebnis](#einzelnes-ocr-ergebnis)
- [Ergebnisdateien](#ergebnisdateien)
- [SummaryResults.json](#summaryresultsjson)
- [Konsolenausgabe](#konsolenausgabe)
- [Installation](#installation)
  - [Voraussetzungen](#voraussetzungen)
- [Repository klonen](#repository-klonen)
- [Projekt wiederherstellen](#projekt-wiederherstellen)
- [API-Key in C#](#api-key-in-c)
- [TESSDATA\_PREFIX](#tessdata_prefix)
- [Projekt starten](#projekt-starten)
- [Benchmark starten](#benchmark-starten)
- [Neue OCR-Engine hinzufügen](#neue-ocr-engine-hinzufügen)
- [Logging](#logging)
- [Bekannte Einschränkungen](#bekannte-einschränkungen)
  - [Tesseract und komplexe Tabellen](#tesseract-und-komplexe-tabellen)
- [Reproduzierbarkeit](#reproduzierbarkeit)

---

# Ziele

Das Projekt verfolgt insbesondere folgende Ziele:

### 1. Vergleichbarkeit

Alle OCR-Engines sollen dieselben Testbilder und dieselben Ground-Truth-Daten verwenden.

### 2. Objektive Bewertung

Die Qualität einer OCR-Erkennung soll anhand messbarer Kennzahlen bewertet werden.

### 3. Erweiterbarkeit

Neue OCR-Engines sollen möglichst einfach hinzugefügt werden können.

### 4. Konfigurierbarkeit

Engine-spezifische Einstellungen sollen nicht mehrmals fest im Programmcode hinterlegt werden, sondern alle Einstellungen werden über die appsettings vorgenommen

### 5. Reproduzierbare Benchmarks

Die Ergebnisse jedes Benchmark-Laufs werden in einem eigenen Ergebnisverzeichnis gespeichert.

### 6. Trennung der Verantwortlichkeiten

OCR-Verarbeitung, Evaluation, Dateiverarbeitung, Konfiguration und Konsolenausgabe sollen voneinander getrennt bleiben.

---

# Features

Aktuell bietet das Projekt unter anderem:

* Vergleich zweier schon implementierter OCR-Engines
  * GLM-OCR
  * Tesseract
* zentrale Konfiguration über `appsettings.json`
* einheitliches `IOcrEngine`-Interface
* OCR-Engine-Factory
* automatische Berechnung der Character Error Rate
* Berechnung der CER in Prozent
* Messung der Verarbeitungszeit
* Erfassung von Token-Verbrauch
* Erfassung der verwendeten GPU
* Zusammenfassung der Ergebnisse pro Modell
* Speicherung einzelner Ergebnisse als JSON
* Speicherung einer Gesamtzusammenfassung als JSON
* Fortschrittsanzeige in der Konsole
* strukturierte Logs über Serilog
* automatisierte Unit-Tests mit xUnit

---

# Technologien

Das Projekt basiert auf folgenden Technologien:

| Technologie                        | Verwendung             |
| ---------------------------------- | ---------------------- |
| C#                                 | Programmiersprache     |
| .NET                               | Laufzeit und Framework |
| Spectre.Console                    | Konsolenoberfläche     |
| Serilog                            | Logging                |
| xUnit                              | Unit-Tests             |
| System.Text.Json                   | JSON-Verarbeitung      |
| Tesseract                          | klassische OCR         |
| GLM-OCR                            | generative OCR         |
| Microsoft.Extensions.Configuration | Konfiguration          |

---

# Architektur

Die Anwendung verwendet mehrere klar getrennte Komponenten.

Der grundsätzliche Ablauf ist:

![Programmablauf](docs\images\OCR_Tester_Programmablauf.svg)

---

# OCR-Abstraktion

Alle OCR-Engines implementieren das zentrale Interface `IOcrEngine`.

```csharp
public interface IOcrEngine
{
    string ModelName { get; }

    Task<OcrResult> ProcessImageAsync(
        ImageTestCase testCase);
}
```

Dadurch kennt der Benchmark Runner nicht die konkreten Implementierungen.

Er arbeitet lediglich mit:

```csharp
IOcrEngine
```

Das ist wichtig für die Erweiterbarkeit.

Der Runner muss beispielsweise nicht wissen, ob ein Modell:

* lokal läuft
* über HTTP angesprochen wird
* Tesseract verwendet
* ein LLM verwendet
* eine Cloud API verwendet

Solange die Engine `IOcrEngine` implementiert, kann sie in den Benchmark integriert werden.

---

# Unterstützte OCR-Engines

## Tesseract

[Tesseract](https://github.com/tesseract-ocr/tesseract) ist eine klassische Open-Source-OCR-Engine.

Die Konfiguration erfolgt beispielsweise über:

```json
{
    "Name": "Tesseract",
    "Type": "Tesseract",
    "TessDataPath": "tessdata",
    "Language": "deu+eng",
    "EngineMode": "TesseractOnly",
    "PageSegmentationMode": "SingleColumn", OsdOnly, AutoOsd, AutoOnly, Auto
    "CPU": "AMD Ryzen 7 7735U",
    "RAM": "16 GB"
}
```
* **TessDataPath:** Pfad in dem die Trainingsdaten enthalten sind
* **Language:** ermöglicht die Verarbeitung deutscher und englischer Zeichen bzw. Wörter
* **EngineMode:** Methodic für OCR Verfahren;
  * Default: Tesseract entscheidet selbst. Aktuell wird dabei die LSTM-OCR-Engine verwendet.
  * LstmOnly: Verwendet ausschließlich die neuere, auf neuronalen Netzen
  * TesseractAndLstm: Verwendet sowohl die klassische Tesseract-Engine als auch die neue LSTM-Engine.
  * TesseractOnly: Verwendet ausschließlich die klassische, ältere Tesseract-OCR-Engine.
* **PageSegmentationMode:** Hier gibt man die möglichen LayoutAnalysenModus an;
  * OsdOnly: Erkennt, wie das Bild gedreht ist und welche Schrift verwendet wird. Kein OCR.
  * AutoOsd: Erkennt automatisch das Seitenlayout und zusätzlich die Ausrichtung/Schrift.
  * AutoOnly: Analysiert das Seitenlayout, führt aber keine Texterkennung durch.
  * Auto: Versucht selbstständig herauszufinden, wie Text auf der Seite angeordnet ist.
  * SingleColumn: Geht davon aus, dass das Bild eine einzelne Textspalte enthält.
  * SingleBlockVertText: Geht von einem einzelnen Block aus, dessen Text vertikal angeordnet ist.
  * SingleBlock: Geht davon aus, dass das Bild einen einzigen zusammenhängenden Textblock enthält.
  * SingleLine: Das gesamte Bild enthält nur eine Textzeile.
  * SingleWord: Das gesamte Bild enthält nur ein einzelnes Wort.
  * CircleWord: Das Bild enthält ein Wort, das kreisförmig angeordnet ist.
  * SingleChar: Das Bild enthält nur ein einzelnes Zeichen.
  * SparseText: Sucht nach möglichst viel Text, auch wenn dieser ungeordnet und verteilt im Bild steht.
  * SparseTextOsd: Behandelt das Bild direkt als eine Textzeile und überspringt bestimmte Tesseract-interne Layout-Anpassungen.
  * RawLine: Kein OCR-Modus. Gibt nur an, wie viele Enum-Werte vorhanden sind.
---

## GLM-OCR

GLM-OCR wird über eine OpenAI-kompatible API angesprochen.

Beispielkonfiguration:

```json
{
  "Name": "GLM-OCR",
  "Type": "glm",
  "Endpoint": "http://localhost:8000/v1",
  "ApiKey": "",
  "Model": "zai-org/GLM-OCR",
  "Prompt": "Extract the text from this image."
}
```

* Mögliche Feste Prompts von GLM-OCR:
  * **"Text Recognition":** Reines Text OCR
  * **"Formula Recognition":** Extrahiert Formeln und gibt sie im Latex Format zurück
  * **"Table Recognition":** extrahiert Tabellenstrukturen mit Inhalt und gibt sie als Markdown oder HTML zurück
  * **Key Information Extraction:** 
    ```json
    {
        请按下列JSON格式输出图中信息: 
        {
            "Zu suchender Schlüssel": "..."
        }
    }
    ```

Der API-Key sollte **nicht direkt in `appsettings.json` gespeichert werden**.

Stattdessen wird eine Umgebungsvariable in dem Ordner der Konsolenanwendung gesetzt.

```bash
[Environment]::SetEnvironmentVariable(
    "GLM_API_KEY", # Den Namen der Variable hier eintragen
    "dein-api-key", # durch den API Key ersetzen
    "User" # User genau so stehen lassen
)
```

Zum überprüfen ob die Variable gesetzt ist:

```bash
[Environment]::GetEnvironmentVariable(
    "GLM_API_KEY", # Den Namen der Variable hier eintragen
    "User" # User genau so stehen lassen
)
```

Zum Löschen:

```powershell
[Environment]::SetEnvironmentVariable(
    "GLM_API_KEY",
    $null,
    "User"
)
```

Nach dem Setzen einer neuen User-Umgebungsvariable sollte die laufende Entwicklungsumgebung neu gestartet werden.

---

# Konfiguration

Die OCR-Engines werden über:

```text
Configuration/appsettings.json
```

konfiguriert.

Die Konfiguration wird über `Microsoft.Extensions.Configuration` geladen.

Die Anwendung kann dadurch beim Start feststellen, welche OCR-Engines ausgeführt werden sollen.

---

# OCR Engine Factory

Die `OcrEngineFactory` wandelt die Konfiguration in konkrete OCR-Implementierungen um.

Beispiel:

```csharp
public IOcrEngine Create(OcrEngineSettings settings)
{
    return settings.Type.ToLowerInvariant() switch
    {
        "glm" => new GlmOcrEngine(
            settings.Endpoint,
            settings.ApiKey,
            settings.Model,
            settings.Prompt
        ),

        "tesseract" => new TesseractOcrEngine(
            settings.TessDataPath,
            settings.Language,
            settings.EngineMode,
            settings.PageSegmentationMode
        ),

        _ => throw new InvalidOperationException(
            $"Unbekannter OCR-Engine-Typ: {settings.Type}")
    };
}
```

Der Vorteil ist, dass `ComparisonRunner` keine konkreten OCR-Klassen instanziieren muss.

---

# Testdaten

Die Testdaten befinden sich im `Testdata`-Verzeichnis.

Ein Testfall besteht grundsätzlich aus:

```text
Bild
+
Ground Truth
```

Beispiel:

```bash
Data/
│
├── Beispiel1/
│   ├── Bild_001.bmp
│   └── Ground_Truth.json # Mit den Daten des jeweiligen Bildes
|
└── Beispiel2/
    ├── Bild_002.bmp
    └── Ground_Truth.json

oder

Data/
│
├── Beispielbilder/
│   ├── Bild_001.bmp
|   └── Bild_002.bmp
│ 
└── Ground_Truth.json # Mit allen Daten
```

Es wird immer nach Bildern und den zugehörigen Ground_Truth's gesucht, unabhängig von der Ordnerstruktur

---

# Ground Truth

Die Ground Truth beschreibt den erwarteten Text eines Bildes.

Ein mögliches JSON-Format ist:

```json
{
  "Bild_001": {
        "ImageName": "Rechnung_001.png",
        "ExpectedText": "Rechnung Max Mustermann Gesamtbetrag: 123,45 €"
  }
}
```

Die Ground Truth ist die Referenz für die spätere Evaluation.

Je genauer die Ground Truth ist, desto aussagekräftiger ist das Benchmark-Ergebnis.

---

# ImageTestCase

Aus den Ground-Truth-Daten wird ein `ImageTestCase` erzeugt.

Das Modell enthält unter anderem:

```csharp
public class ImageTestCase
{
    public string ImageName =>
        GroundTruth.ImageName;

    public string ImagePath =>
        GroundTruth.ImagePath;

    public string ExpectedText =>
        GroundTruth.ExpectedText;
}
```

Dadurch kann der Benchmark Runner direkt mit einem Testfall arbeiten.

---

# Character Error Rate

Die **Character Error Rate (CER)** ist eine Kennzahl zur Bewertung von OCR-Systemen.

Sie basiert auf der Levenshtein-Distanz.

Dabei werden folgende Operationen betrachtet:

* Einfügen eines Zeichens
* Löschen eines Zeichens
* Ersetzen eines Zeichens

Die grundlegende Formel lautet:

```text
CER = Edit Distance / Anzahl Zeichen im Ground-Truth-Text
```

Für die Prozentdarstellung:

```text
CER % = Edit Distance / Anzahl Zeichen × 100
```

---

# CER Calculator

Die Berechnung erfolgt zentral über den `CerCalculator`.

Dadurch muss keine OCR-Engine selbst wissen, wie ihre Erkennungsqualität bewertet wird.

Das Ergebnis wird in `SingleBenchmark` gespeichert.

Beispielsweise:

```csharp
public class SingleBenchmark
{
    public int CharacterErrorRate { get; set; }

    public float CharacterErrorRateInPercent { get; set; }
}
```

Dadurch stehen sowohl die absolute Edit Distance als auch die prozentuale CER zur Verfügung.

---

# Benchmark Summary

Während des Benchmark-Laufs werden die Ergebnisse pro Modell zusammengefasst.

Ein `BenchmarkModelSummary` enthält beispielsweise:

```csharp
ModelName // Das Modell um das es geht
TotalImagesProcessed // Gesamtzahl der Bilder die Verarbeitet wurden
TotalProcessingTimeMs // Gesamtverarbeitungszeit
AverageProcessingTimeMs // Durchschnittsverarbeitungszeit
OverallCharacterErrorRate // Gesamt CER in Prozent
OverallCharacterErrors // Gesamtzahl der Errors
TotalInputTokens // Gesamtzahl der Eingabe tokens
TotalOutputTokens // Gesamtzahl der Ausgabe tokens
GraphicsProcessingUnit // Verwendete GPU
VRAM // Zur Verfügung stehender VRAM
CPU // Verwendete CPU
RAM // Zur Verfügung stehender RAM
```

---

# Einzelnes OCR-Ergebnis

Für jede Kombination aus Bild und OCR-Modell wird ein `OcrResult` erzeugt.

Beispiel:

```json
{
  "SingleBenchmark": {
    "CharacterErrorRate": ,
    "CharacterErrorRateInPercent": 
  },
  "CharacterErrorRate": ,
  "CharacterErrorRateInPercent": ,
  "ModelName": "",
  "RecognizedText": "",
  "ProcessingTimeMs": ,
  "InputTokens": ,
  "OutputTokens": ,
  "TotalTokens": 
}
```

---

# Ergebnisdateien

Für jeden Benchmark-Lauf wird ein eigenes Verzeichnis erstellt.

Beispiel:

```text
Results/
└── 20260907_112300/
    │
    ├── Bild_001_GLM-OCR_Result.json
    ├── Bild_001_Tesseract_Result.json
    ├── Bild_002_GLM-OCR_Result.json
    ├── Bild_002_Tesseract_Result.json
    │
    └── SummaryResults.json
```

Der Zeitstempel verhindert, dass ältere Benchmark-Ergebnisse überschrieben werden.

---

# SummaryResults.json

Am Ende eines Benchmark-Laufs wird eine zusammengefasste Datei erzeugt:

```text
SummaryResults.json
```

Diese enthält die Ergebnisse aller Modelle.

Beispiel:

```json
{
  "Models": [
    {
      "ModelName": ,
      "TotalImagesProcessed": ,
      "TotalProcessingTimeMs": ,
      "AverageProcessingTimeMs": ,
      "OverallCharacterErrorRate": ,
      "OverallCharacterErrors": ,
      "TotalInputTokens": ,
      "TotalOutputTokens": ,
      "GraphicsProcessingUnit": ,
      "VRAM": ,
      "CPU": ,
      "RAM": 
    }
  ]
}
```

Damit können Benchmark-Ergebnisse später auch automatisiert weiterverarbeitet oder ausgewertet werden.

---

# Konsolenausgabe

Für die Darstellung in der Konsole wird `Spectre.Console` verwendet.

Die Konsolenausgabe ist zentral im `ConsoleFormatter` gekapselt.

Beispielsweise werden Methoden für:

* Header
* Fehler
* Warnungen
* Informationen
* Fortschritt
* Benchmark-Start
* Benchmark-Ende
* Zusammenfassung

bereitgestellt.

Dadurch muss der eigentliche Benchmark-Code keine Formatierungsdetails enthalten.

---

# Installation

## Voraussetzungen

Für die Entwicklung werden benötigt:

* [.NET SDK 10.0.400](https://dotnet.microsoft.com/en-us/download/dotnet/10.0)

Je nach verwendeter OCR-Engine können weitere Voraussetzungen notwendig sein.

Für Tesseract werden insbesondere die entsprechenden `tessdata`-Dateien benötigt. Die Standard tessdata sind schon im Projekt vorhanden. Sollten jedoch die `fast` oder `best` genutzt werden wollen müssen diese eigenständig heruntergeladen werden. 

Für GLM-OCR muss ein erreichbarer GLM-OCR-Service vorhanden sein.

---

# Repository klonen

Das Repository kann beispielsweise mit Git geklont werden:

```bash
git clone <repository-url>
```

Danach:

```bash
cd OCR_Tester
```

---

# Projekt wiederherstellen

Abhängigkeiten können mit:

```bash
dotnet restore
```

wiederhergestellt werden.

Anschließend kann das Projekt gebaut werden:

```bash
dotnet build
```

---

# API-Key in C#

Der Key kann in C# über die Umgebungsvariable in `ComparisonRunner.cs` geladen werden:

```csharp
var apiKey = Environment.GetEnvironmentVariable(
    "GLM_API_KEY",
    EnvironmentVariableTarget.User);
```

Wenn kein Key gefunden wird, sollte die Anwendung einen verständlichen Fehler ausgeben.

---

# TESSDATA_PREFIX

Falls Tesseract die Sprachdaten nicht findet, kann unter Windows zusätzlich `TESSDATA_PREFIX` gesetzt werden.

Beispiel:

```powershell
[Environment]::SetEnvironmentVariable(
    "TESSDATA_PREFIX",
    "C:\Pfad\zu\OCR_Tester\tessdata",
    "User"
)
```

Anschließend kann der Wert überprüft werden:

```powershell
[Environment]::GetEnvironmentVariable(
    "TESSDATA_PREFIX",
    "User"
)
```

Danach sollte die Entwicklungsumgebung neu gestartet werden.

---

# Projekt starten

Das Projekt kann über eine Dotnet IDE oder Konsole gestartet werden.

Der Konsolenbefehl ist:

```bash
dotnet run
```

Anschließend erscheint das Hauptmenü.

![MainMenu](docs\images\MainMenu.png)

---

# Benchmark starten

Nach Auswahl von `Vergleich starten` soll der Pfad der Testdaten angegeben werden
werden:

![PathWindow](docs\images\PathWindow.png)

und danach startet der OCR Ablauf:

![TestAblauf](docs\images\TestAblauf.png)

Abschließend wird dann die Zusammenfassung und der Pfad der Ergebnisdateien angezeigt

![Result](docs\images\Result.png)

---

# Neue OCR-Engine hinzufügen

Eine wichtige Eigenschaft des Projekts ist die Erweiterbarkeit.

Eine neue OCR-Engine sollte `IOcrEngine` implementieren.

Beispielsweise:

```csharp
public class MyOcrEngine : IOcrEngine
{
    public string ModelName => "My OCR";

    public async Task<OcrResult> ProcessImageAsync(
        ImageTestCase testCase)
    {
        // OCR-Verarbeitung

        return new OcrResult
        {
            ModelName = ModelName,
            RecognizedText = "...",
            ProcessingTimeMs = 123
        };
    }
}
```

Anschließend wird die Engine in der Factory registriert.

Beispielsweise:

```csharp
"myocr" => new MyOcrEngine(
    settings.Endpoint,
    settings.ApiKey
),
```

Danach kann sie über `appsettings.json` aktiviert werden:

```json
{
  "Endpoint": "My OCR",
  "Prompt": "myocr"
}
```

Im `ComparisonRunner` selbst muss nur der Name der API_Key Umgebungsvariable geändert werden (siehe [API-Key in C#](#api-key-in-c))

---

# Logging

Für Logging wird Serilog verwendet. Die Logdaten werden nach Tagen beschrieben und 7 Tage bleiben erhalten, bis sie gelöscht werden.

---

# Bekannte Einschränkungen

## Tesseract und komplexe Tabellen

Tesseract erkennt Text, ist aber nicht primär auf komplexe Tabellenstrukturen spezialisiert.

Bei Tabellen können daher Fehler bei:

* Spaltenzuordnung
* Zeilenstruktur
* Zellgrenzen
* Reihenfolge der Textfragmente

auftreten.

Für Tabellen sollte deshalb nicht ausschließlich die CER betrachtet werden. Mir ist bewusst das dies hier der Fall ist, aber das Ziel dieses Programmes ist es zu verdeutlichen wo die Grenzen klassischer OCR Modelle liegen und Tesseract ist eines der bekanntesten Open-Source Modelle.

---

# Reproduzierbarkeit

Für aussagekräftige Vergleiche sollten möglichst identische Bedingungen verwendet werden.

Dazu gehören:

* gleicher Bilddatensatz
* gleiche Ground Truth
* gleiche Bildauflösung
* gleiche OCR-Konfiguration
* gleiche Hardware
* gleiche Modellversion
* gleiche Prompt-Konfiguration
* gleiche Spracheinstellungen

Besonders bei KI-basierten OCR-Modellen sollten Modellversion und Konfiguration dokumentiert werden.

