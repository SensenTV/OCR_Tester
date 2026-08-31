param(
    [string]$Repository = "https://github.com/SensenTV/OCR_Tester"
)

$ErrorActionPreference = "Stop"

# ------------------------------------------------------------
# OCR_Tester - GitHub Issue Bulk Creator
# ------------------------------------------------------------

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host " OCR_Tester - GitHub Issue Import" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Prüfen, ob GitHub CLI installiert ist
if (-not (Get-Command gh -ErrorAction SilentlyContinue)) {
    Write-Host "FEHLER: GitHub CLI 'gh' wurde nicht gefunden." -ForegroundColor Red
    Write-Host ""
    Write-Host "Installiere GitHub CLI und führe danach 'gh auth login' aus."
    exit 1
}

# Prüfen, ob Benutzer angemeldet ist
try {
    gh auth status 2>&1 | Out-Null

    if ($LASTEXITCODE -ne 0) {
        throw "Nicht angemeldet"
    }
}
catch {
    Write-Host "FEHLER: Du bist nicht bei GitHub angemeldet." -ForegroundColor Red
    Write-Host ""
    Write-Host "Führe zuerst aus:"
    Write-Host "    gh auth login" -ForegroundColor Yellow
    Write-Host ""
    exit 1
}

# Repository automatisch erkennen
if ([string]::IsNullOrWhiteSpace($Repository)) {

    try {
        $Repository = (
            gh repo view --json nameWithOwner --jq ".nameWithOwner"
        ).Trim()
    }
    catch {
        Write-Host "FEHLER: Repository konnte nicht automatisch erkannt werden." -ForegroundColor Red
        Write-Host ""
        Write-Host "Alternativ kannst du das Repository beim Aufruf angeben:"
        Write-Host ""
        Write-Host ".\create-ocr-tester-issues.ps1 -Repository `"USERNAME/OCR_Tester`"" -ForegroundColor Yellow
        Write-Host ""
        exit 1
    }
}

Write-Host "Repository:" -NoNewline
Write-Host " $Repository" -ForegroundColor Green

# CSV im gleichen Ordner wie das Script
$CsvPath = Join-Path $PSScriptRoot "OCR_Tester_GitHub_Issues.csv"

if (-not (Test-Path $CsvPath)) {

    Write-Host ""
    Write-Host "FEHLER: CSV-Datei nicht gefunden!" -ForegroundColor Red
    Write-Host ""
    Write-Host "Erwarteter Pfad:"
    Write-Host $CsvPath -ForegroundColor Yellow
    Write-Host ""
    exit 1
}

# CSV laden
$Issues = Import-Csv -Path $CsvPath

Write-Host "CSV:" -NoNewline
Write-Host " $CsvPath" -ForegroundColor Green

Write-Host "Issues gefunden:" -NoNewline
Write-Host " $($Issues.Count)" -ForegroundColor Green

Write-Host ""

# ------------------------------------------------------------
# Labels
# ------------------------------------------------------------

$Labels = @(
    "setup",
    "architecture",
    "dependencies",
    "console-ui",
    "input",
    "decision",
    "ground-truth",
    "ocr",
    "tesseract",
    "glm-ocr",
    "api",
    "evaluation",
    "metrics",
    "performance",
    "cost",
    "hardware",
    "documentation",
    "output",
    "json",
    "testing",
    "quality",
    "logging",
    "legal",
    "benchmark",
    "priority:high",
    "priority:medium",
    "priority:low"
)

Write-Host "Prüfe/erstelle Labels..." -ForegroundColor Cyan

foreach ($Label in $Labels) {

    Write-Host "  -> $Label"

    gh label create `
        $Label `
        --repo $Repository `
        --force `
        --description "OCR_Tester"

    if ($LASTEXITCODE -ne 0) {
        Write-Host "     Warnung: Label konnte nicht erstellt werden." -ForegroundColor Yellow
    }
}

Write-Host ""
Write-Host "Labels fertig." -ForegroundColor Green
Write-Host ""

# ------------------------------------------------------------
# Bestätigung
# ------------------------------------------------------------

Write-Host "Es werden jetzt $($Issues.Count) Issues in"
Write-Host "$Repository" -ForegroundColor Cyan
Write-Host "erstellt."
Write-Host ""

$Confirmation = Read-Host "Fortfahren? (j/n)"

if ($Confirmation -notmatch "^[jJyY]$") {

    Write-Host ""
    Write-Host "Abgebrochen." -ForegroundColor Yellow
    exit 0
}

Write-Host ""

# ------------------------------------------------------------
# Issues erstellen
# ------------------------------------------------------------

$Counter = 0
$Successful = 0
$Failed = 0

foreach ($Issue in $Issues) {

    $Counter++

    Write-Host "[$Counter/$($Issues.Count)] " -NoNewline -ForegroundColor Cyan
    Write-Host $Issue.Title

    # Temporäre Datei für den Issue-Body
    $BodyFile = [System.IO.Path]::GetTempFileName()

    try {

        Set-Content `
            -Path $BodyFile `
            -Value $Issue.Body `
            -Encoding UTF8

        # Labels aus CSV
        $LabelList = @()

        if (-not [string]::IsNullOrWhiteSpace($Issue.Labels)) {

            $LabelList += (
                $Issue.Labels -split "," |
                ForEach-Object {
                    $_.Trim()
                }
            )
        }

        # Prioritätslabel hinzufügen
        switch ($Issue.Priority.ToLower()) {

            "high" {
                $LabelList += "priority:high"
            }

            "medium" {
                $LabelList += "priority:medium"
            }

            "low" {
                $LabelList += "priority:low"
            }
        }

        # Duplikate entfernen
        $LabelList = $LabelList | Select-Object -Unique

        # Argumente für gh vorbereiten
        $Arguments = @(
            "issue",
            "create",
            "--repo",
            $Repository,
            "--title",
            $Issue.Title,
            "--body-file",
            $BodyFile
        )

        foreach ($Label in $LabelList) {

            $Arguments += "--label"
            $Arguments += $Label
        }

        # Issue erstellen
        & gh @Arguments

        if ($LASTEXITCODE -eq 0) {

            Write-Host "       OK" -ForegroundColor Green
            $Successful++
        }
        else {

            Write-Host "       FEHLER" -ForegroundColor Red
            $Failed++
        }
    }
    catch {

        Write-Host "       FEHLER: $($_.Exception.Message)" -ForegroundColor Red
        $Failed++
    }
    finally {

        if (Test-Path $BodyFile) {

            Remove-Item `
                $BodyFile `
                -Force `
                -ErrorAction SilentlyContinue
        }
    }

    Write-Host ""
}

# ------------------------------------------------------------
# Ergebnis
# ------------------------------------------------------------

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host " Import abgeschlossen" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

Write-Host "Erfolgreich: " -NoNewline
Write-Host $Successful -ForegroundColor Green

Write-Host "Fehlgeschlagen: " -NoNewline
Write-Host $Failed -ForegroundColor Red

Write-Host "Gesamt: " -NoNewline
Write-Host $Issues.Count -ForegroundColor Cyan

Write-Host ""

if ($Failed -eq 0) {

    Write-Host "Alle Issues wurden erfolgreich erstellt." -ForegroundColor Green
}
else {

    Write-Host "Einige Issues konnten nicht erstellt werden." -ForegroundColor Yellow
}

Write-Host ""
Write-Host "Repository:"
Write-Host "https://github.com/SensenTV/OCR_Tester/issues" -ForegroundColor Cyan
Write-Host ""