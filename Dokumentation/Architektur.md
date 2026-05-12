# 🏗️ Architektur & Ordnerstruktur

## 🗺️ Ordnerstruktur — Was liegt wo?

Hier siehst du, wie das Projekt aufgebaut ist und was sich in welchem Ordner befindet:

```
Projekt WM Tool/               ← Das Haupt-Repository auf GitHub
│
├── README.md                  ← Hauptseite (Projektbeschreibung & Erklärung)
├── LICENSE                    ← Lizenz des Projekts
│
└── WM-Tipp-Tool/              ← Der eigentliche C#-Quellcode
    │
    ├── Program.cs             ← 🚀 DER STARTPUNKT – hier beginnt das Programm
    ├── WM-Tipp-Tool.csproj    ← Projektdatei (Einstellungen für den Build)
    ├── WM-Tipp-Tool.sln       ← Lösungsdatei für Visual Studio (nicht anfassen!)
    ├── app.manifest           ← DPI-Einstellungen für Windows
    ├── db.config              ← 🔑 Deine MySQL-Zugangsdaten (NICHT in Git!)
    ├── db.config.example      ← Vorlage für die db.config
    │
    ├── Datenbank/             ← 📦 Alles rund um MySQL-Verbindung & Logging
    │   ├── DatenbankVerbindung.cs   ← Stellt die Verbindung zu MySQL her
    │   └── SQLProtokollierer.cs     ← Speichert alle SQL-Befehle für das Terminal
    │
    ├── Formulare/             ← 🖼️ Alle Fenster (Forms) des Programms
    │   ├── MainForm.cs              ← Das Hauptmenü (erstes Fenster)
    │   ├── SpielForm.cs             ← Spiele anlegen & löschen (Logik)
    │   ├── SpielFormDesign.cs       ← Visuelles Layout für das Spiel-Fenster
    │   ├── TippForm.cs              ← Tipps abgeben (Logik)
    │   ├── TippFormDesign.cs        ← Visuelles Layout für das Tipp-Fenster
    │   ├── RanglisteForm.cs         ← Ergebnisse eintragen & Rangliste sehen
    │   ├── DatenbankAnsichtForm.cs  ← Rohdaten in der Datenbank anzeigen
    │   ├── ProtokollForm.cs         ← Live SQL-Terminal (Hacker-Style)
    │   └── DesignHelper.cs          ← Zentrale Hilfsklasse für Farben & Styles
    │
    └── SQL/                   ← 📄 SQL-Skripte zum manuellen Einrichten der DB
        ├── 01_create_database.sql
        ├── 02_create_spiele.sql
        └── 03_create_tipps.sql
```

---

## 🔌 Wie startet das Programm? — `Program.cs`

Die Datei `Program.cs` ist der **allererste Code**, der ausgeführt wird. Sie ist der Startknopf unseres Programms:

```csharp
// 1. DPI-Modus setzen (damit alles scharf ist)
Application.SetHighDpiMode(HighDpiMode.PerMonitorV2);
Application.EnableVisualStyles();

// 2. Datenbank einrichten (Tabellen erstellen, falls nötig)
DatenbankVerbindung.DatenbankInitialisieren();

// 3. Das Hauptfenster öffnen
Application.Run(new MainForm());
```

**Der Ablauf beim Start:**
1. Windows-Einstellungen setzen (Schärfe auf jedem Monitor)
2. Die Klasse `DatenbankVerbindung` aufrufen → sie liest die `db.config`, verbindet sich mit MySQL und erstellt die Tabellen, falls sie noch nicht existieren
3. Das Hauptmenü (`MainForm`) öffnen
