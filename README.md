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

* [Über das Projekt](#über-das-projekt)
* [Ziele](#ziele)
* [Features](#features)
* [Technologien](#technologien)
* [Projektstruktur](#projektstruktur)
* [Architektur](#architektur)
* [Unterstützte OCR-Engines](#unterstützte-ocr-engines)
* [Konfiguration](#konfiguration)
* [API-Key konfigurieren](#api-key-konfigurieren)
* [Testdaten](#testdaten)
* [Ground Truth](#ground-truth)
* [Benchmark-Ablauf](#benchmark-ablauf)
* [Character Error Rate](#character-error-rate)
* [Ergebnisdateien](#ergebnisdateien)
* [Konsolenausgabe](#konsolenausgabe)
* [Installation](#installation)
* [Projekt starten](#projekt-starten)
* [Tests ausführen](#tests-ausführen)
* [Neue OCR-Engine hinzufügen](#neue-ocr-engine-hinzufügen)
* [Logging](#logging)
* [Fehlerbehandlung](#fehlerbehandlung)
* [Git und Testdaten](#git-und-testdaten)
* [Bekannte Einschränkungen](#bekannte-einschränkungen)
* [Weiterentwicklung](#weiterentwicklung)

---

# Über das Projekt

OCR steht für **Optical Character Recognition** und bezeichnet die automatische Erkennung von Text aus Bildern.

Das Ziel von `OCR_Tester` ist nicht lediglich zu prüfen, ob eine OCR-Engine Text erkennen kann. Stattdessen soll eine reproduzierbare Möglichkeit geschaffen werden, mehrere OCR-Systeme anhand derselben Bilder und derselben erwarteten Texte miteinander zu vergleichen.

Dafür wird jedem Testbild ein sogenannter **Ground-Truth-Text** zugeordnet.

Beispielsweise:

```text
Bild:
rechnung_001.png

Erwarteter Text:
Rechnung
Max Mustermann
Gesamtbetrag: 123,45 €
```

Eine OCR-Engine verarbeitet anschließend das Bild und liefert beispielsweise:

```text
Rechnung
Max Musterman
Gesamtbetrag: 123,45 €
```

Der erkannte Text wird anschließend mit dem erwarteten Text verglichen.

Dadurch kann beispielsweise festgestellt werden, dass ein Zeichen fehlt:

```text
Erwartet:  Max Mustermann
Erkannt:   Max Musterman
                    ^
```

Dieser Fehler fließt in die Character Error Rate ein.

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

Engine-spezifische Einstellungen sollen nicht fest im Programmcode hinterlegt werden.

### 5. Reproduzierbare Benchmarks

Die Ergebnisse jedes Benchmark-Laufs werden in einem eigenen Ergebnisverzeichnis gespeichert.

### 6. Trennung der Verantwortlichkeiten

OCR-Verarbeitung, Evaluation, Dateiverarbeitung, Konfiguration und Konsolenausgabe sollen voneinander getrennt bleiben.

---

# Features

Aktuell bietet das Projekt unter anderem:

* Vergleich mehrerer OCR-Engines
* zentrale Konfiguration über `appsettings.json`
* Unterstützung von Tesseract
* Unterstützung von GLM-OCR
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
| GLM-OCR                            | KI-basierte OCR        |
| Microsoft.Extensions.Configuration | Konfiguration          |

---

# Projektstruktur

Eine mögliche Struktur des Projekts sieht folgendermaßen aus:

```text
OCR_Tester/
│
├── Configuration/
│   └── appsettings.json
│
├── ConsoleUI/
│   ├── ConsoleFormatter.cs
│   ├── MainMenu.cs
│   └── ComparisonMenu.cs
│
├── Application/
│   └── ComparisonRunner.cs
│
├── Domain/
│   ├── Interfaces/
│   │   ├── IOcrEngine.cs
│   │   └── IEvaluationService.cs
│   │
│   └── Models/
│       ├── Image.cs
│       ├── GroundTruth.cs
│       ├── ImageTestCase.cs
│       ├── OcrResult.cs
│       ├── SingleBenchmark.cs
│       ├── BenchmarkSummary.cs
│       └── BenchmarkModelSummary.cs
│
├── Evaluation/
│   ├── CerCalculator.cs
│   └── BenchmarkSummaryCalculator.cs
│
├── Input/
│   └── DataLoader.cs
│
├── OCR/
│   ├── GLM/
│   │   └── GlmOcrEngine.cs
│   │
│   └── Tesseract/
│       └── TesseractOcrEngine.cs
│
├── Output/
│   └── JsonResultWriter.cs
│
├── Data/
│   └── ...
│
├── Results/
│   └── ...
│
├── Tests/
│   ├── CerCalculatorTests.cs
│   └── DataLoaderTests.cs
│
├── AssemblyInfo.cs
├── Program.cs
└── README.md
```

Die genaue Ordnerstruktur kann sich im Laufe der Entwicklung ändern.

---

# Architektur

Die Anwendung verwendet mehrere klar getrennte Komponenten.

Der grundsätzliche Ablauf ist:

```text
                    ┌─────────────────────┐
                    │     MainMenu        │
                    └──────────┬──────────┘
                               │
                               ▼
                    ┌─────────────────────┐
                    │  ComparisonMenu     │
                    └──────────┬──────────┘
                               │
                               ▼
                    ┌─────────────────────┐
                    │ ComparisonRunner    │
                    └──────────┬──────────┘
                               │
             ┌─────────────────┼─────────────────┐
             │                 │                 │
             ▼                 ▼                 ▼
      ┌─────────────┐  ┌──────────────┐  ┌─────────────┐
      │ DataLoader  │  │ EngineFactory│  │ Configuration│
      └─────────────┘  └──────┬───────┘  └─────────────┘
                               │
                    ┌──────────┴──────────┐
                    │                     │
                    ▼                     ▼
             ┌─────────────┐       ┌─────────────┐
             │ GLM-OCR      │       │ Tesseract   │
             └─────────────┘       └─────────────┘
                    │                     │
                    └──────────┬──────────┘
                               ▼
                    ┌─────────────────────┐
                    │    OcrResult        │
                    └──────────┬──────────┘
                               │
                               ▼
                    ┌─────────────────────┐
                    │   CerCalculator     │
                    └──────────┬──────────┘
                               │
                               ▼
                    ┌─────────────────────┐
                    │ SummaryCalculator   │
                    └──────────┬──────────┘
                               │
                               ▼
                    ┌─────────────────────┐
                    │ JsonResultWriter    │
                    └─────────────────────┘
```

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
  "EngineMode": "TesseractOnly"
}
```

Dabei können unter anderem Sprache und Page Segmentation Mode konfiguriert werden.

### Beispiel

```text
TessDataPath:
tessdata

Language:
deu+eng
```

`deu+eng` ermöglicht die Verarbeitung deutscher und englischer Zeichen bzw. Wörter.

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

Der API-Key sollte **nicht direkt in `appsettings.json` gespeichert werden**.

Stattdessen wird eine Umgebungsvariable verwendet.

---

# Konfiguration

Die OCR-Engines werden über:

```text
Configuration/appsettings.json
```

konfiguriert.

Beispiel:

```json
{
  "OcrEngines": [
    {
      "Name": "GLM-OCR",
      "Type": "glm",
      "Endpoint": "http://localhost:8000/v1",
      "ApiKey": "",
      "Model": "zai-org/GLM-OCR",
      "Prompt": "Extract the text from this image."
    },
    {
      "Name": "Tesseract",
      "Type": "Tesseract",
      "TessDataPath": "tessdata",
      "Language": "deu+eng",
      "EngineMode": "TesseractOnly",
      "PageSegmentationMode": 6
    }
  ]
}
```

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

Die Testdaten befinden sich im `Data`-Verzeichnis.

Ein Testfall besteht grundsätzlich aus:

```text
Bild
+
Ground Truth
```

Beispiel:

```text
Data/
│
├── Rechnung/
│   ├── Rechnung_001.png
│   └── groundtruth.json
│
├── Dokumente/
│   ├── Dokument_001.png
│   └── groundtruth.json
│
└── Tabellen/
    ├── Tabelle_001.png
    └── groundtruth.json
```

Die genaue Ordnerstruktur kann abhängig vom verwendeten Datensatz angepasst werden.

---

# Ground Truth

Die Ground Truth beschreibt den erwarteten Text eines Bildes.

Ein mögliches JSON-Format ist:

```json
{
  "Rechnung_001": {
    "Image": {
      "ImageName": "Rechnung_001.png",
      "ImagePath": "Data/Rechnung/Rechnung_001.png"
    },
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
    public GroundTruth GroundTruth { get; set; } = new();

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

# Benchmark-Ablauf

Ein Benchmark-Lauf läuft grundsätzlich folgendermaßen ab:

## 1. Konfiguration laden

Die Anwendung liest:

```text
Configuration/appsettings.json
```

ein.

---

## 2. OCR-Engines erstellen

Für jeden konfigurierten Eintrag wird über die Factory eine OCR-Engine erstellt.

Beispielsweise:

```text
GLM-OCR
Tesseract
```

---

## 3. Testdaten laden

Der `DataLoader` lädt alle Testfälle.

---

## 4. Benchmark starten

Jedes Bild wird mit jeder konfigurierten OCR-Engine verarbeitet.

Bei:

```text
100 Bilder
2 OCR-Engines
```

entstehen:

```text
100 × 2 = 200 OCR-Operationen
```

---

## 5. Verarbeitungszeit messen

Für jede OCR-Operation wird die benötigte Zeit erfasst.

Beispiel:

```text
GLM-OCR
Processing Time: 842 ms
```

---

## 6. OCR-Ergebnis erfassen

Das Ergebnis wird in einem `OcrResult` gespeichert.

Beispiel:

```text
Model:
GLM-OCR

RecognizedText:
Rechnung Max Musterman

ProcessingTime:
842 ms
```

---

## 7. CER berechnen

Der erkannte Text wird mit der Ground Truth verglichen.

Dabei wird die Character Error Rate berechnet.

---

## 8. Ergebnis speichern

Das Ergebnis des einzelnen Bildes wird als JSON gespeichert.

---

## 9. Summary aktualisieren

Der `BenchmarkSummaryCalculator` aktualisiert die zusammengefassten Werte des entsprechenden Modells.

---

## 10. Benchmark abschließen

Nach Verarbeitung aller Bilder wird eine Gesamtzusammenfassung erstellt.

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

## Beispiel

Ground Truth:

```text
Hallo Welt
```

OCR-Ergebnis:

```text
Hallo Wel
```

Es fehlt ein Zeichen:

```text
Hallo Wel[t]
```

Die Edit Distance beträgt:

```text
1
```

Bei einer Ground Truth mit 10 Zeichen ergibt sich:

```text
CER = 1 / 10
    = 0,1
    = 10 %
```

Je niedriger die CER ist, desto besser.

```text
0 %   = perfekt
5 %   = sehr gut
10 %  = einige Fehler
50 %  = viele Fehler
100 % = sehr hohe Fehlerquote
```

Diese Einteilung dient lediglich als grobe Orientierung. Die tatsächliche Qualität hängt stark vom jeweiligen Datensatz ab.

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

```text
ModelName
TotalImagesProcessed
TotalProcessingTimeMs
AverageProcessingTimeMs
OverallCharacterErrorRate
OverallCharacterErrors
TotalInputTokens
TotalOutputTokens
TotalCostInCents
GraphicsProcessingUnit
```

Beispiel:

```text
GLM-OCR

Bilder:
100

Gesamtzeit:
82.400 ms

Durchschnitt:
824 ms

CER:
4,82 %

Zeichenfehler:
183

Input Tokens:
12.500

Output Tokens:
9.800
```

---

# Einzelnes OCR-Ergebnis

Für jede Kombination aus Bild und OCR-Modell wird ein `OcrResult` erzeugt.

Beispiel:

```json
{
  "ModelName": "GLM-OCR",
  "RecognizedText": "Rechnung Max Musterman",
  "ProcessingTimeMs": 842,
  "InputTokens": 125,
  "OutputTokens": 38,
  "CharacterErrorRate": 1,
  "CharacterErrorRateInPercent": 4.55
}
```

Die genaue JSON-Struktur hängt vom aktuellen Modell ab.

---

# Ergebnisdateien

Für jeden Benchmark-Lauf wird ein eigenes Verzeichnis erstellt.

Beispiel:

```text
Results/
└── 20260907_112300/
    │
    ├── Rechnung_001_GLM-OCR_Result.json
    ├── Rechnung_001_Tesseract_Result.json
    ├── Rechnung_002_GLM-OCR_Result.json
    ├── Rechnung_002_Tesseract_Result.json
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
      "ModelName": "GLM-OCR",
      "TotalImagesProcessed": 100,
      "TotalProcessingTimeMs": 82400,
      "AverageProcessingTimeMs": 824,
      "OverallCharacterErrorRate": 4.82,
      "OverallCharacterErrors": 183,
      "TotalInputTokens": 12500,
      "TotalOutputTokens": 9800
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

# Fortschrittsanzeige

Während des Benchmarks zeigt die Anwendung den aktuellen Fortschritt an.

Beispielsweise:

```text
OCR Benchmark
████████████████████████████████████████ 75 %

GLM-OCR → Rechnung_075.png
```

Die maximale Anzahl der Operationen wird aus:

```text
Anzahl Testfälle × Anzahl OCR-Engines
```

berechnet.

Dadurch funktioniert die Fortschrittsanzeige unabhängig davon, wie viele Engines oder Bilder konfiguriert sind.

---

# Installation

## Voraussetzungen

Für die Entwicklung werden benötigt:

* .NET SDK
* Visual Studio oder JetBrains Rider
* Git

Je nach verwendeter OCR-Engine können weitere Voraussetzungen notwendig sein.

Für Tesseract werden insbesondere die entsprechenden `tessdata`-Dateien benötigt.

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

# API-Key konfigurieren

API-Keys sollten nicht in `appsettings.json` oder im Git-Repository gespeichert werden.

Für GLM-OCR wird daher eine Umgebungsvariable verwendet.

Unter Windows PowerShell:

```powershell
[Environment]::SetEnvironmentVariable(
    "GLM_API_KEY",
    "dein-api-key",
    "User"
)
```

Anschließend kann der Wert überprüft werden:

```powershell
[Environment]::GetEnvironmentVariable(
    "GLM_API_KEY",
    "User"
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

Nach dem Setzen einer neuen User-Umgebungsvariable sollte Visual Studio bzw. die laufende Entwicklungsumgebung neu gestartet werden.

---

# API-Key in C#

Der Key kann in C# über die Umgebungsvariable geladen werden:

```csharp
var apiKey = Environment.GetEnvironmentVariable(
    "GLM_API_KEY",
    EnvironmentVariableTarget.User);
```

Wenn kein Key gefunden wird, sollte die Anwendung einen verständlichen Fehler ausgeben.

Beispiel:

```text
Die Umgebungsvariable 'GLM_API_KEY' ist nicht gesetzt.
Setze sie bitte und starte die Anwendung erneut.
```

---

# Tesseract konfigurieren

Tesseract benötigt die entsprechenden Sprachdaten.

Beispielsweise:

```text
tessdata/
├── deu.traineddata
└── eng.traineddata
```

In der Konfiguration kann anschließend angegeben werden:

```json
{
  "TessDataPath": "tessdata",
  "Language": "deu+eng"
}
```

---

# Tesseract Page Segmentation Mode

Für unterschiedliche Dokumenttypen können unterschiedliche Page Segmentation Modes sinnvoll sein.

Beispielsweise kann für einfache Tabellen ein Test mit mehreren Modi sinnvoll sein:

```text
PSM 6
PSM 4
PSM 11
```

Die optimale Einstellung hängt vom Aufbau der Testbilder ab.

Tesseract ist grundsätzlich eine OCR-Engine und kein spezialisiertes Tabellenstruktur-Erkennungssystem. Bei komplexen Tabellen können daher zusätzliche Fehler bei Zeilen-, Spalten- oder Zellstrukturen auftreten.

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

Das Projekt kann über Visual Studio gestartet werden.

Alternativ über die .NET CLI:

```bash
dotnet run
```

Anschließend erscheint das Hauptmenü.

Beispielsweise:

```text
OCR Benchmark - Hauptmenü

Was möchtest du tun?

> Vergleich starten
  Beenden
```

---

# Benchmark starten

Nach Auswahl von:

```text
Vergleich starten
```

werden:

1. Konfiguration geladen
2. OCR-Engines erstellt
3. Testdaten geladen
4. Benchmark gestartet
5. OCR-Ergebnisse berechnet
6. CER berechnet
7. Einzelergebnisse gespeichert
8. Summary erstellt
9. Summary gespeichert
10. Ergebnisse in der Konsole angezeigt

---

# Tests ausführen

Das Projekt verwendet xUnit für Unit-Tests.

Alle Tests können über:

```bash
dotnet test
```

ausgeführt werden.

Alternativ können Tests direkt in Visual Studio über den Test Explorer gestartet werden.

---

# CER-Tests

Der `CerCalculator` wird unter anderem mit folgenden Fällen getestet:

### Identische Texte

```text
Expected:
Hello World

OCR:
Hello World

Ergebnis:
CER = 0
```

### Ein Fehler

```text
Expected:
Hello World

OCR:
Hello Wxrld

Ergebnis:
1 Zeichenfehler
```

### Fehlendes Zeichen

```text
Expected:
Hello World

OCR:
Hello Wold
```

### Zusätzliches Zeichen

```text
Expected:
Hello World

OCR:
Hello Woorld
```

### Leere Strings

Auch folgende Fälle werden getestet:

```text
""
""
```

sowie:

```text
""
"Text"
```

und:

```text
"Text"
""
```

Dadurch wird sichergestellt, dass die CER-Berechnung auch für Grenzfälle korrekt funktioniert.

---

# DataLoader-Tests

Auch der `DataLoader` wird mit Unit-Tests überprüft.

Getestet werden unter anderem:

* gültige JSON-Datei
* ungültige JSON-Datei
* fehlende Bildreferenz
* leerer Ground-Truth-Text

Für Tests werden temporäre Verzeichnisse und JSON-Dateien erzeugt.

Die Testdaten werden nach Abschluss des Tests wieder entfernt.

---

# InternalsVisibleTo

Wenn interne Methoden direkt getestet werden sollen, kann die Test-Assembly über `InternalsVisibleTo` Zugriff erhalten.

Dafür befindet sich beispielsweise im Hauptprojekt:

```text
AssemblyInfo.cs
```

mit:

```csharp
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("OCR_Tester.Tests")]
```

Dadurch können Tests auf `internal`-Methoden zugreifen, ohne diese Methoden öffentlich machen zu müssen.

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
  "Name": "My OCR",
  "Type": "myocr"
}
```

Der `ComparisonRunner` selbst muss dafür nicht verändert werden.

---

# Warum das Interface wichtig ist

Ohne `IOcrEngine` müsste der Runner konkrete Klassen kennen:

```csharp
var glm = new GlmOcrEngine(...);
var tesseract = new TesseractOcrEngine(...);
```

Das würde den Runner mit jeder neuen OCR-Engine komplexer machen.

Mit dem Interface arbeitet der Runner stattdessen allgemein:

```csharp
foreach (var engine in ocrEngines)
{
    var result =
        await engine.ProcessImageAsync(testCase);
}
```

Damit ist die eigentliche Implementierung der OCR-Engine für den Runner irrelevant.

---

# Konfigurationsabhängigkeit

Das Projekt verfolgt das Prinzip:

```text
Code
  ↓
generische Logik

Konfiguration
  ↓
konkrete Einstellungen
```

Engine-spezifische Werte wie:

* Endpoint
* Model
* Prompt
* Sprache
* Tesseract-Modus
* Page Segmentation Mode

sollten möglichst nicht direkt im Benchmark Runner stehen.

---

# Logging

Für Logging wird Serilog verwendet.

Beispielsweise:

```csharp
Log.Information(
    "Sending image to GLM-OCR: {ImagePath}",
    testCase.ImagePath);
```

Fehler werden inklusive Exception geloggt:

```csharp
catch (Exception ex)
{
    Log.Error(
        ex,
        "Error occurred while processing image: {ImagePath}",
        testCase.ImagePath);

    throw;
}
```

Dadurch bleiben Fehlermeldungen inklusive Stack Trace erhalten.

---

# Exception Handling

Auf Ebene der OCR-Engine können Fehler protokolliert und anschließend weitergereicht werden.

Beispielsweise:

```csharp
catch (Exception ex)
{
    Log.Error(
        ex,
        "Fehler beim Senden des Bildes an GLM-OCR.");

    throw new InvalidOperationException(
        "Fehler beim Senden des Bildes an GLM-OCR.",
        ex);
}
```

Durch:

```csharp
throw;
```

wird eine Exception unverändert weitergereicht.

Durch:

```csharp
throw ex;
```

kann dagegen der ursprüngliche Stack Trace verloren gehen.

Daher sollte grundsätzlich `throw;` verwendet werden, wenn keine neue Exception benötigt wird.

---

# Fehlerbehandlung im Benchmark

Die Anwendung kann grundsätzlich zwischen zwei Strategien unterscheiden.

## Fehler beendet Benchmark

Ein schwerwiegender Fehler wird weitergereicht:

```text
OCR Engine
    ↓
Exception
    ↓
ComparisonRunner
    ↓
Application beendet
```

Dies ist sinnvoll, wenn ein Benchmark ohne vollständige Ergebnisse keinen Sinn ergibt.

## Fehler überspringt einzelnen Test

Alternativ kann ein Fehler protokolliert und nur das aktuelle Bild übersprungen werden.

Beispielsweise:

```text
Bild 25:
GLM-OCR → Fehler

Bild 26:
GLM-OCR → erfolgreich
```

Diese Strategie eignet sich besonders für große Testdatensätze.

---

# Git

Das Repository sollte keine sensiblen Informationen enthalten.

Insbesondere sollten folgende Inhalte nicht committed werden:

```text
API Keys
Secrets
persönliche Zugangsdaten
große Testdatensätze
generierte Benchmark-Ergebnisse
```

---

# .gitignore

Wenn der komplette Testdatenordner nicht versioniert werden soll:

```gitignore
Data/
```

Damit wird der gesamte Inhalt von `Data` ignoriert.

Auch generierte Ergebnisse können ignoriert werden:

```gitignore
Results/
```

Eine mögliche Konfiguration:

```gitignore
Data/
Results/
bin/
obj/
```

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

Für Tabellen sollte deshalb nicht ausschließlich die CER betrachtet werden.

---

## CER bewertet nur Text

Die CER misst Unterschiede zwischen zwei Texten.

Sie bewertet nicht direkt:

* Tabellenstruktur
* Layout
* Positionen
* Bounding Boxes
* Spalten
* Zeilen
* Schriftarten
* Dokumentstruktur

Ein OCR-Modell kann daher eine relativ gute CER besitzen, obwohl die erkannte Dokumentstruktur für einen bestimmten Anwendungsfall ungeeignet ist.

---

# Benchmark-Ergebnisse interpretieren

Bei einem Vergleich von OCR-Modellen sollten mehrere Kennzahlen betrachtet werden.

Beispielsweise:

| Kennzahl               | Bedeutung                        |
| ---------------------- | -------------------------------- |
| CER                    | Textqualität                     |
| Durchschnittliche Zeit | Geschwindigkeit                  |
| Gesamtzeit             | Laufzeit des gesamten Benchmarks |
| Input Tokens           | Eingabeaufwand eines KI-Modells  |
| Output Tokens          | erzeugter Textaufwand            |
| Kosten                 | wirtschaftlicher Vergleich       |
| GPU                    | verwendete Hardware              |

Ein Modell mit der niedrigsten CER ist daher nicht automatisch das beste Modell.

Beispiel:

```text
Modell A
CER: 2 %
Zeit: 4 Sekunden

Modell B
CER: 4 %
Zeit: 0,5 Sekunden
```

Welches Modell besser ist, hängt vom konkreten Einsatzzweck ab.

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

---

# Entwicklungsprinzipien

Das Projekt orientiert sich an einigen grundlegenden Prinzipien.

## Single Responsibility

Jede Komponente sollte möglichst eine klar definierte Aufgabe besitzen.

Beispiele:

```text
DataLoader
→ Daten laden

CerCalculator
→ OCR-Qualität bewerten

BenchmarkSummaryCalculator
→ Ergebnisse aggregieren

JsonResultWriter
→ Ergebnisse speichern

ConsoleFormatter
→ Konsolenausgabe formatieren

ComparisonRunner
→ Benchmark-Ablauf steuern
```

---

## Dependency Inversion

Der Runner arbeitet mit Abstraktionen:

```csharp
IOcrEngine
```

statt direkt mit:

```csharp
GlmOcrEngine
TesseractOcrEngine
```

Das erleichtert spätere Erweiterungen.

---

# Weiterentwicklung

Mögliche zukünftige Erweiterungen sind:

## Weitere OCR-Modelle

Beispielsweise:

* PaddleOCR
* EasyOCR
* weitere Vision-LLMs
* Cloud-OCR-Dienste
* lokale Vision-Modelle

---

## Weitere Metriken

Neben CER könnten implementiert werden:

* Word Error Rate (WER)
* Precision
* Recall
* F1-Score
* Normalized Edit Distance
* Layout Similarity
* Table Structure Accuracy

---

## Parallelisierung

Aktuell können OCR-Operationen sequenziell ausgeführt werden.

Für große Datensätze könnte eine kontrollierte Parallelisierung die Laufzeit deutlich reduzieren.

Dabei muss allerdings auf die jeweilige OCR-Engine geachtet werden, da nicht jede Engine beliebig viele parallele Requests unterstützt.

---

## HTML- oder Web-Reports

Die JSON-Ergebnisse könnten später für einen übersichtlichen Report verwendet werden.

Beispielsweise:

```text
Benchmark Report

┌────────────┬─────────┬──────────┬────────────┐
│ Modell     │ CER     │ Ø Zeit   │ Bilder     │
├────────────┼─────────┼──────────┼────────────┤
│ GLM-OCR    │ 2,41 %  │ 820 ms   │ 100        │
│ Tesseract  │ 7,83 %  │ 210 ms   │ 100        │
└────────────┴─────────┴──────────┴────────────┘
```

---

## Grafische Auswertung

Langfristig könnten die Ergebnisse beispielsweise als Diagramme dargestellt werden:

```text
CER

GLM-OCR     █████
Tesseract   ███████████████
```

oder:

```text
Verarbeitungszeit

GLM-OCR     █████████████
Tesseract   ███
```

Dadurch können Qualitäts- und Performance-Unterschiede schneller erkannt werden.

---

# Zusammenfassung

`OCR_Tester` stellt eine modulare Benchmark-Plattform für OCR-Systeme dar.

Der zentrale Gedanke ist die Trennung von:

```text
Testdaten
    ↓
OCR Engine
    ↓
OCR Result
    ↓
Evaluation
    ↓
Summary
    ↓
JSON / Console
```

Durch das `IOcrEngine`-Interface können unterschiedliche OCR-Systeme über eine gemeinsame Schnittstelle angesprochen werden.

Die Konfiguration über `appsettings.json` ermöglicht es, OCR-Engines und deren Einstellungen zentral zu verwalten.

Die `CerCalculator`-Komponente sorgt für eine einheitliche Bewertung der OCR-Ergebnisse.

Der `BenchmarkSummaryCalculator` aggregiert die Ergebnisse und ermöglicht dadurch einen direkten Vergleich der Modelle.

Die einzelnen Ergebnisse und die Gesamtzusammenfassung werden als JSON gespeichert, sodass die Daten später unabhängig von der Konsolenanwendung weiterverarbeitet werden können.

Damit bildet das Projekt eine gute Grundlage für einen reproduzierbaren und erweiterbaren Vergleich verschiedener OCR-Technologien.

---

# Lizenz

Falls das Projekt öffentlich veröffentlicht wird, sollte hier die verwendete Lizenz angegeben werden.

Beispielsweise:

```text
MIT License
```

oder die für das Projekt gewünschte Lizenz.

---

# Autor

**OCR_Tester**

Ein C#-Projekt zum Benchmarking und Vergleich verschiedener OCR-Engines.


Open-AI API Key setzten:

in dem Ordner der Konsolenanwendung 

```bash
[Environment]::SetEnvironmentVariable(
    "GLM_API_KEY",
    "dein-api-key",
    "User"
)
```