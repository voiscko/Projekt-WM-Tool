<div align="center">
  <img src="cover.png" alt="WM-Tipp-Tool 2026 Cover" width="800" style="border-radius: 10px; box-shadow: 0 4px 8px rgba(0,0,0,0.1); margin-bottom: 20px;" />

  # 🏆 WM-Tipp-Tool 2026

  **Das ultimative Desktop-Tool für dein WM-Tippspiel im Heimnetz!** ⚽

  [![C#](https://img.shields.io/badge/C%23-%23239120.svg?style=for-the-badge&logo=c-sharp&logoColor=white)](https://docs.microsoft.com/en-us/dotnet/csharp/)
  [![.NET 8](https://img.shields.io/badge/.NET_8-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/)
  [![MySQL](https://img.shields.io/badge/MySQL-4479A1?style=for-the-badge&logo=mysql&logoColor=white)](https://www.mysql.com/)
  [![Windows Forms](https://img.shields.io/badge/Windows_Forms-0078D6?style=for-the-badge&logo=windows&logoColor=white)](https://docs.microsoft.com/en-us/dotnet/desktop/winforms/)

  *Eine leistungsstarke Windows Forms Anwendung mit MySQL-Datenbankanbindung. Entwickelt für reibungslose Tippspiele und Schulpräsentationen.*
</div>

---

## ✨ Features

- **🎮 Spiele verwalten:** Spiele anlegen, Ergebnisse eintragen, alles im Blick behalten.
- **🎯 Tipps abgeben:** Jeder Nutzer gibt seinen eigenen Tipp ab – mit Duplikatschutz.
- **🏆 Automatische Rangliste:** Punkte werden sofort berechnet, die Tabelle aktualisiert sich von selbst.
- **🖥️ DPI-Aware:** Scharfes UI, auch auf 4K-Monitoren und Windows-VMs mit Skalierung.
- **👨‍🏫 Präsentations-Modus:** Live SQL-Terminal + Rohdatenbank-Ansicht für Lehrer.
- **🌙 Dark Mode:** Elegantes dunkles Design auf allen Fenstern.

---

## 🗺️ Ordnerstruktur — Was liegt wo?

Hier siehst du, wie das Projekt aufgebaut ist und was sich in welchem Ordner befindet:

```
Projekt WM Tool/               ← Das Haupt-Repository auf GitHub
│
├── README.md                  ← Diese Datei (Projektbeschreibung & Erklärung)
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

---

## 🔑 Die Datenbankverbindung — `Datenbank/DatenbankVerbindung.cs`

Dies ist unsere wichtigste Hilfsklasse. Sie ist `static` — das bedeutet, sie existiert **nur einmal** im gesamten Programm und ist von überall erreichbar.

### Wie liest sie die `db.config`?

Die `db.config`-Datei ist eine einfache Textdatei:
```ini
Host=localhost
Port=3306
Database=wm_tipp_db
User=root
Password=
```

Die Klasse liest diese Datei Zeile für Zeile und sucht nach dem `=`-Zeichen. Alles links davon ist der **Schlüssel** (`Host`), alles rechts ist der **Wert** (`localhost`).

### Wie wird die Verbindung aufgebaut?

MySQL braucht einen sogenannten **ConnectionString** – das ist eine einzelne Textzeile mit allen Anmeldedaten zusammengefasst:

```
Server=localhost;Port=3306;Database=wm_tipp_db;Uid=root;Pwd=;CharSet=utf8mb4;
```

Die Methode `VerbindungAbrufen()` baut diesen String zusammen und gibt ein `MySqlConnection`-Objekt zurück. Dieses Objekt ist quasi das "Telefonkabel" zur Datenbank.

### Die wichtigsten Methoden im Überblick

| Methode | Was sie tut |
| :--- | :--- |
| `DatenbankInitialisieren()` | Wird beim Programmstart einmalig aufgerufen. Erstellt die Datenbank und Tabellen, falls sie noch nicht existieren. |
| `VerbindungAbrufen()` | Gibt ein neues, geöffnetes Verbindungs-Objekt zurück. Wird in jedem Formular genutzt. |
| `VerbindungTesten()` | Probiert kurz, ob MySQL überhaupt erreichbar ist. Gibt `true` oder `false` zurück. |
| `KonfigurationLaden()` | Liest die `db.config`-Datei. Wird automatisch beim ersten Aufruf erledigt. |

---

## 📋 Der SQL-Protokollierer — `Datenbank/SQLProtokollierer.cs`

Diese Klasse ist unser **digitales Notizbuch**. Jedes Mal, wenn irgendwo im Programm ein SQL-Befehl an MySQL geschickt wird, rufen wir hier `Protokollieren(...)` auf.

```csharp
// Beispiel aus SpielForm.cs:
SQLProtokollierer.Protokollieren("INSERT INTO spiele ...");
```

Die Klasse speichert alle Einträge in einer Liste. Wenn das `ProtokollForm` (das Terminal-Fenster) geöffnet wird, zeigt es alle gesammelten Einträge an.

**Das besondere Feature:** Ein **Event** (`BeiNeuemProtokollEintrag`). Das ist wie ein Alarm. Das `ProtokollForm` meldet sich bei diesem Alarm an. Sobald ein neuer Befehl protokolliert wird, klingelt der Alarm und der neue Eintrag erscheint **sofort in Echtzeit** im Terminal-Fenster, ohne dass man auf Aktualisieren drücken muss.

---

## 🖼️ Die Formulare (Fenster) — `Formulare/`

Jede `.cs`-Datei im `Formulare/`-Ordner ist **ein Fenster** des Programms. Sie alle erben von der Klasse `Form` (von Windows bereitgestellt), die die Grundfunktionalität eines Fensters mitbringt.

Jedes Formular hat immer diese zwei Methoden:

1. **Konstruktor** (`public MainForm()`) – Wird aufgerufen, wenn das Fenster "erschaffen" wird. Hier werden `KomponentenInitialisieren()` und Daten laden aufgerufen.
2. **`KomponentenInitialisieren()`** – Baut das visuelle Design auf (Buttons, Farben, Layout).

### 📐 Warum gibt es extra "Design"-Dateien? (Refactoring)

Damit der Code für alle im Team einfacher zu verstehen ist, haben wir die **Logik** (Was passiert, wenn man klickt?) vom **Design** (Wo ist der Button, welche Farbe hat er?) getrennt. 

Das nennt man **"Refactoring"** (Code aufräumen).
Vorher waren hunderte Zeilen Design-Code mit dem Logik-Code vermischt. Jetzt haben wir für einige Formulare `partial` Klassen erstellt. `partial` bedeutet, dass eine C#-Klasse auf mehrere Dateien aufgeteilt werden kann:

- **Die Logik-Datei (z.B. `TippForm.cs`):** Enthält nur noch Datenbankabfragen, Klick-Events und das Prüfen von Eingaben.
- **Die Design-Datei (z.B. `TippFormDesign.cs`):** Enthält nur die `KomponentenInitialisieren()` Methode mit Schriftarten, Farben und Layout (TableLayoutPanel etc.).
- **Der `DesignHelper.cs`:** Eine zentrale Hilfsklasse. Hier definieren wir unsere Farben und standardisieren, wie Buttons, Tabellen und Dropdowns im ganzen Programm aussehen sollen. Das spart unglaublich viel Code!

**Der Erfolg dieses Refactorings (Wer hat was gemacht):**
Durch diese Auslagerung wurden die Hauptdateien extrem verkleinert und sind nun perfekt übersichtlich für das Lernen und die Präsentation!

| Datei | Vorher | Nachher | Wer ist zuständig? |
| :--- | :--- | :--- | :--- |
| `TippForm.cs` (Logik) | 515 Zeilen | **160 Zeilen** | Aylin |
| `SpielForm.cs` (Logik) | 450 Zeilen | **131 Zeilen** | Lian |
| `TippFormDesign.cs` | - | ~180 Zeilen (neu) | Mark |
| `SpielFormDesign.cs` | - | ~160 Zeilen (neu) | Mark |
| `DesignHelper.cs` | - | Zentrale Design-Logik | Mark |

Jetzt können Aylin und Lian sich zu 100% auf die C#-Logik und MySQL konzentrieren, während Mark die Design-Dateien und den `DesignHelper` verwaltet.

---

### Wie öffnet man ein Fenster?

```csharp
// Öffnet TippForm als Dialog (Hauptfenster wird blockiert, bis TippForm geschlossen wird)
TippForm fenster = new TippForm();
fenster.ShowDialog();
```

---

### 1. `MainForm.cs` — Das Hauptmenü

Das ist das **allererste Fenster**, das der Nutzer sieht. Es hat 5 Buttons, die jeweils ein anderes Fenster öffnen. Außerdem zeigt es unten an, ob MySQL gerade verbunden ist (grüner oder roter Punkt).

**Ablauf:**
1. `KomponentenInitialisieren()` baut alle 5 Buttons und das Layout auf
2. `DatenbankStatusPruefen()` ruft `DatenbankVerbindung.VerbindungTesten()` auf und zeigt das Ergebnis an

---

### 2. `SpielForm.cs` — Spiele verwalten

Hier kann der Admin (Lehrer/Organisator) neue Spiele anlegen und bestehende löschen.

**So funktioniert das Hinzufügen:**
```
Nutzer gibt Team1, Team2, Datum ein
  → Klick auf "Spiel hinzufügen"
  → BtnHinzufuegen_Klick() wird aufgerufen
  → Eingaben werden geprüft (leer? gleicher Name?)
  → SQL: INSERT INTO spiele (team1, team2, datum) VALUES (...)
  → Tabelle wird neu geladen: LadeSpiele()
```

**So funktioniert das Löschen:**
```
Nutzer klickt eine Zeile in der Tabelle an
  → Klick auf "Ausgewähltes Spiel löschen"
  → ID des ausgewählten Spiels aus der Tabelle auslesen
  → Bestätigungsdialog (JA/NEIN?)
  → SQL: DELETE FROM spiele WHERE id = ...
  → Tabelle wird neu geladen
```

> **Wichtig:** Wenn ein Spiel gelöscht wird, werden durch `ON DELETE CASCADE` in der Datenbank **automatisch auch alle Tipps** zu diesem Spiel gelöscht!

---

### 3. `TippForm.cs` — Tipps abgeben

Hier geben die Mitspieler ihre Tipps ein. Das Dropdown-Menü zeigt nur Spiele an, die noch **kein Ergebnis** haben (also noch nicht gespielt wurden).

**Hintergrundliste mit IDs:**
Das Dropdown zeigt dem Nutzer lesbaren Text (`"Deutschland vs. Spanien (14.06.2026 18:00)"`). Im Hintergrund gibt es eine geheime `List<int> spielIDs`, die die echten Datenbank-IDs in der gleichen Reihenfolge speichert. Wenn der Nutzer den 3. Eintrag auswählt, nehmen wir `spielIDs[2]` als die echte ID für den Datenbankbefehl.

**Duplikatschutz:**
Bevor ein Tipp gespeichert wird, fragt das Programm die Datenbank:
```sql
SELECT COUNT(*) FROM tipps WHERE spiel_id = 5 AND benutzername = 'Max'
```
Wenn die Zahl größer als 0 ist, hat der Nutzer schon getippt → Fehlermeldung.

---

### 4. `RanglisteForm.cs` — Ergebnisse & Rangliste

Dies ist das **komplexeste Fenster**. Es hat zwei Aufgaben:
1. Echte Ergebnisse eintragen und Punkte berechnen
2. Die Gesamtrangliste anzeigen

**Wie werden Punkte berechnet?**

```csharp
private static int BerechnePunkte(int tipp1, int tipp2, int ergebnis1, int ergebnis2)
{
    // Exakt richtig? → 3 Punkte
    if (tipp1 == ergebnis1 && tipp2 == ergebnis2) return 3;

    // Richtige Tendenz? (Wer gewonnen hat, stimmt)
    // Math.Sign gibt +1 (Sieg), -1 (Niederlage) oder 0 (Unentschieden) zurück
    if (Math.Sign(tipp1 - tipp2) == Math.Sign(ergebnis1 - ergebnis2)) return 1;

    return 0; // Komplett falsch
}
```

**Der Ablauf beim Eintragen eines Ergebnisses:**
```
Admin wählt Spiel im Dropdown, gibt Ergebnis ein
  → Klick auf "Ergebnis speichern & Punkte berechnen"
  → UPDATE spiele SET ergebnis_team1 = ..., ergebnis_team2 = ... WHERE id = ...
  → Alle Tipps für dieses Spiel laden (SELECT ... FROM tipps WHERE spiel_id = ...)
  → Für JEDEN Tipp: BerechnePunkte() aufrufen
  → UPDATE tipps SET punkte = ... WHERE id = ...
  → Rangliste und Tipps-Tabelle neu laden
```

**Die Rangliste-Abfrage (komplexes SQL):**
```sql
SELECT
  ROW_NUMBER() OVER (ORDER BY SUM(punkte) DESC) AS 'Platz',
  benutzername AS 'Name',
  SUM(punkte) AS 'Gesamtpunkte',
  COUNT(*) AS 'Tipps gesamt'
FROM tipps
GROUP BY benutzername
ORDER BY Gesamtpunkte DESC
```
- `GROUP BY benutzername` → fasst alle Tipps eines Nutzers zusammen
- `SUM(punkte)` → addiert alle seine Punkte
- `ROW_NUMBER()` → vergibt automatisch die Platzierung (1, 2, 3, ...)

---

### 5. `DatenbankAnsichtForm.cs` — Rohdaten ansehen

Ein einfaches Fenster für Präsentationen. Es zeigt den genauen Inhalt der Datenbank-Tabellen so an, wie sie in MySQL gespeichert sind. Der SQL-Befehl dahinter ist simpel:

```sql
SELECT * FROM spiele   -- oder tipps
```

---

### 6. `ProtokollForm.cs` — SQL-Terminal

Ein Terminal-Fenster im Hacker-Style (schwarzer Hintergrund, grüne Schrift). Es zeigt in Echtzeit jeden SQL-Befehl an, der im Programm ausgeführt wird.

**Wie funktioniert die Echtzeit-Aktualisierung?**

Das `ProtokollForm` meldet sich beim **Event** des `SQLProtokollierers` an:
```csharp
// Im Konstruktor:
SQLProtokollierer.BeiNeuemProtokollEintrag += SQLProtokollierer_BeiNeuemProtokollEintrag;
```

Wenn jetzt irgendwo im Programm `SQLProtokollierer.Protokollieren(...)` aufgerufen wird, feuert das Event und die Methode `SQLProtokollierer_BeiNeuemProtokollEintrag` wird automatisch aufgerufen → neuer Text erscheint sofort im Terminal.

Wenn das Fenster **geschlossen** wird, muss es sich wieder abmelden (sonst Absturz):
```csharp
// OnFormClosing:
SQLProtokollierer.BeiNeuemProtokollEintrag -= SQLProtokollierer_BeiNeuemProtokollEintrag;
```

---

## 🗄️ Die Datenbank — Tabellenstruktur

Das Programm nutzt zwei Tabellen in MySQL, die beim ersten Start **automatisch erstellt** werden:

### Tabelle `spiele`
Speichert alle WM-Spiele.

| Spalte | Typ | Beschreibung |
| :--- | :--- | :--- |
| `id` | INT, PK, Auto | Automatische, einmalige ID |
| `team1` | VARCHAR(100) | Name von Team 1 |
| `team2` | VARCHAR(100) | Name von Team 2 |
| `datum` | DATETIME | Datum und Uhrzeit des Spiels |
| `ergebnis_team1` | INT, NULL | Echtes Ergebnis (leer = Spiel noch offen) |
| `ergebnis_team2` | INT, NULL | Echtes Ergebnis (leer = Spiel noch offen) |

### Tabelle `tipps`
Speichert alle Tipps der Nutzer.

| Spalte | Typ | Beschreibung |
| :--- | :--- | :--- |
| `id` | INT, PK, Auto | Automatische, einmalige ID |
| `spiel_id` | INT, FK | Verknüpfung zur `spiele`-Tabelle |
| `benutzername` | VARCHAR(100) | Name des Tippers |
| `tipp_team1` | INT | Getippte Tore für Team 1 |
| `tipp_team2` | INT | Getippte Tore für Team 2 |
| `punkte` | INT | Errechnete Punkte (Standard: 0) |

> **Fremdschlüssel (FK):** Die Spalte `spiel_id` in `tipps` verweist auf die `id` in `spiele`. Mit `ON DELETE CASCADE` werden beim Löschen eines Spiels automatisch alle dazugehörigen Tipps mitgelöscht.

---

## 💯 Punktesystem

| Situation | Beispiel | Punkte |
| :--- | :--- | :--- |
| **Exaktes Ergebnis** | Getippt: 2:1, Echt: 2:1 | **3 Punkte** |
| **Richtige Tendenz** | Getippt: 3:0, Echt: 1:0 | **1 Punkt** |
| **Falsch getippt** | Getippt: 2:1, Echt: 0:1 | **0 Punkte** |

---

## 🚀 Installation & Start

### Voraussetzungen
1. **[.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)** installiert
2. **MySQL / XAMPP** läuft lokal auf Port 3306
3. Visual Studio (empfohlen) oder VS Code mit C# Dev Kit

### Schritt 1 — Repository klonen & Pakete laden
```powershell
dotnet restore
```

### Schritt 2 — Datenbank konfigurieren
```powershell
copy db.config.example db.config
```
Die `db.config` öffnen und das MySQL-Passwort eintragen:
```ini
Host=localhost
Port=3306
Database=wm_tipp_db
User=root
Password=DEIN_PASSWORT
```

### Schritt 3 — Programm starten
```powershell
dotnet run
```
Beim ersten Start erstellt das Programm die Datenbank und alle Tabellen **automatisch**!

---

## 🛠️ Tech-Stack

| Komponente | Technologie | Beschreibung |
| :--- | :--- | :--- |
| **Sprache** | C# (.NET 8) | Moderne, typsichere Programmierung |
| **UI-Framework** | Windows Forms | Desktop-Fenster auf Windows |
| **Datenbank** | MySQL | Relationales Datenbanksystem |
| **Treiber** | `MySql.Data 9.1.0` | Offizielles NuGet-Paket für C# ↔ MySQL |
| **DPI-Support** | PerMonitorV2 | Scharfes UI auf allen Monitoren |

---

## 👥 Team & Aufgabenverteilung

- **Aylin**: Developer — [TippForm.cs Logik](WM-Tipp-Tool/Formulare/TippForm.cs)
- **Lian**: Developer — [SpielForm.cs Logik](WM-Tipp-Tool/Formulare/SpielForm.cs)
- **Mark**: UI-Design Refactoring - [Program.cs](WM-Tipp-Tool/Program.cs) + [Datenbank/](WM-Tipp-Tool/Datenbank/))

---

<div align="center">
  <i>Entwickelt mit ❤️ für das Schulprojekt & die Weltmeisterschaft 2026.</i><br>
  <b>Ein Projekt von voiscko (Telegram: <a href="https://t.me/voiscko">@voiscko</a>)</b>
</div>
