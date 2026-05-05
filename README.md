<div align="center">
  <!-- HINWEIS: Speichere das hochgeladene Bild als "cover.png" im gleichen Ordner wie diese Datei, damit es hier angezeigt wird! -->
  <img src="cover.png" alt="WM-Tipp-Tool 2026 Cover" width="800" style="border-radius: 10px; box-shadow: 0 4px 8px rgba(0,0,0,0.1); margin-bottom: 20px;" />

  # 🏆 WM-Tipp-Tool 2026

  **Das ultimative Desktop-Tool für dein WM-Tippspiel im Heimnetz!** ⚽

  [![C#](https://img.shields.io/badge/C%23-%23239120.svg?style=for-the-badge&logo=c-sharp&logoColor=white)](https://docs.microsoft.com/en-us/dotnet/csharp/)
  [![.NET 8](https://img.shields.io/badge/.NET_8-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/)
  [![MySQL](https://img.shields.io/badge/MySQL-4479A1?style=for-the-badge&logo=mysql&logoColor=white)](https://www.mysql.com/)
  [![Windows Forms](https://img.shields.io/badge/Windows_Forms-0078D6?style=for-the-badge&logo=windows&logoColor=white)](https://docs.microsoft.com/en-us/dotnet/desktop/winforms/)

  *Eine leistungsstarke, DPI-bewusste Windows Forms Anwendung mit MySQL-Datenbankanbindung. Entwickelt für reibungslose Tippspiele, beeindruckende Schulpräsentationen und echtes Fußballfieber.*
</div>

---

## ✨ Features

- **🎮 Intuitive Verwaltung:** Spiele in Sekundenschnelle anlegen, reale Ergebnisse eintragen und immer den Überblick behalten.
- **🎯 Smartes Tippsystem:** Jeder Mitspieler gibt seinen persönlichen Tipp ab (inkl. integriertem Duplikatschutz, um mehrfache Tipps zu verhindern).
- **🏆 Automatisierte Rangliste:** Punkteberechnung in Echtzeit. Die Tabelle aktualisiert sich magisch von selbst!
- **🖥️ DPI-Awareness:** Gestochen scharfes, responsives UI – das Layout passt sich automatisch an, selbst auf hochskalierten Windows-VMs (`PerMonitorV2`).
- **👨‍🏫 Präsentations-Modus:** Integriertes **Live SQL-Terminal** und eine **Rohdatenbank-Ansicht** – perfekt, um Lehrern die Funktionalität unter der Haube zu demonstrieren!
- **🌙 Modernes Design:** Elegantes Dark-Mode-Layout, das auf jedem Monitor gut aussieht.

---

## 🛠️ Tech-Stack & Bibliotheken

Hier ein Blick auf die Technologien, die dieses Projekt antreiben:

| Komponente | Technologie / Bibliothek | Beschreibung |
| :--- | :--- | :--- |
| **Sprache** | C# (.NET 8) | Moderne, leistungsstarke und typsichere Programmierung. |
| **UI-Framework** | Windows Forms | `net8.0-windows` für klassische Desktop-Entwicklung. |
| **Datenbank** | MySQL | Zuverlässiges relationelles Datenbanksystem. |
| **Treiber** | `MySql.Data (9.1.0)` | Offizieller NuGet-Treiber für die reibungslose C#-zu-MySQL-Verbindung. |
| **DPI-Support** | `PerMonitorV2` | Code & `app.manifest` für perfekte Skalierung ohne Verzerrungen. |

---

## 🚀 Installation & Start

Mit dieser Anleitung bringst du das Projekt in wenigen Minuten zum Laufen.

### Voraussetzungen
1. **[.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)** installiert.
2. **[MySQL Community Server](https://dev.mysql.com/downloads/mysql/)** läuft lokal auf deinem Rechner (Port 3306).
3. *(Empfohlen)* **[VS Code](https://code.visualstudio.com/)** mit der *C# Dev Kit* Extension.

### 1. Repository vorbereiten
Öffne ein Terminal (z.B. PowerShell) direkt im Projektordner (`WM-Tipp-Tool`):
```powershell
# Lädt alle benötigten Bibliotheken (wie MySql.Data) aus dem Netz
dotnet restore
```

### 2. Datenbank konfigurieren (`db.config`)
Sicherheit geht vor! Die Datenbank-Zugangsdaten werden nicht im Code gespeichert, sondern in einer lokalen Datei.

Erstelle die Konfigurationsdatei aus der mitgelieferten Vorlage:
```powershell
copy db.config.example db.config
```

Öffne die neue `db.config` Datei in einem Texteditor und trage dein MySQL-Passwort ein:
```ini
Host=localhost
Port=3306
Database=wm_tipp_db
User=root
Password=DEIN_PASSWORT_HIER
```
*(Pro-Tipp: Wenn du das Verknüpfen der Datei mal vergisst, fängt das Programm dies beim Start ab und erstellt automatisch eine Platzhalter-Datei für dich!)*

### 3. Anwendung starten
Jetzt kann es losgehen:
```powershell
dotnet run
```
**Die Magie passiert im Hintergrund:** Beim allerersten Start verbindet sich das Tool mit MySQL, erstellt automatisch die Datenbank `wm_tipp_db` und legt alle benötigten Tabellen (`spiele`, `tipps`) selbstständig an! 🎉

---

## 🕹️ So funktioniert das Tippspiel (Workflow)

Das Spielprinzip ist einfach, logisch und macht riesig Spaß:

1. 📅 **Spiele anlegen:** Der Admin trägt die anstehenden WM-Spiele ein (Team 1 vs. Team 2 + Datum & Uhrzeit).
2. ✍️ **Tipps abgeben:** Jeder Mitspieler gibt über das Dropdown-Menü seinen persönlichen Tipp für ein Spiel ab.
3. 📺 **Mitfiebern:** Das echte Spiel läuft im TV!
4. 🏁 **Ergebnis eintragen:** Nach dem Abpfiff trägst du das reale Ergebnis ein. Die Punkte der Spieler werden sofort berechnet.
5. 🥇 **Rangliste checken:** Wer ist der Tippkönig? Die Rangliste zeigt die Platzierungen und Gesamtpunkte live an.

### 💯 Punktelogik

Das Tool nutzt ein faires und transparentes Punktesystem:

| Situation | Beispiel | Punkte |
| :--- | :--- | :--- |
| **Exaktes Ergebnis** | Getippt: 2:1, Echtes Ergebnis: 2:1 | **3 Punkte** |
| **Richtige Tendenz** | Getippt: 2:1, Echtes Ergebnis: 3:0 | **1 Punkt** |
| **Falsch getippt** | Getippt: 2:1, Echtes Ergebnis: 0:1 | **0 Punkte** |

---

## 👨‍💻 Für Entwickler & Lehrer (Präsentation)

Dieses Projekt wurde mit Blick auf Transparenz und Lehre entwickelt. Der gesamte Code ist **einsteigerfreundlich und vollständig auf Deutsch kommentiert**, um Team-Kollegen die Einarbeitung zu erleichtern.

### Integrierte Präsentations-Tools
- 💻 **Live SQL-Terminal (`LogForm`):** Ein spezielles Fenster, das in Echtzeit alle `INSERT`, `UPDATE` und `SELECT` Befehle anzeigt. Perfekt, um im Unterricht zu erklären, was genau passiert, wenn man auf "Speichern" drückt!
- 🗄️ **Rohdaten-Ansicht (`DatabaseViewForm`):** Ein ungeschönter, direkter Blick auf die MySQL-Tabellen, um zu beweisen, dass die Datenstruktur ordnungsgemäß gefüllt wird.

### Datenbankschema im Detail

<details>
<summary><b>Klicke hier, um das relationale Tabellen-Design zu sehen</b></summary>

**Tabelle `spiele`**
- `id` (INT, PK, Auto_Increment)
- `team1`, `team2` (VARCHAR)
- `datum` (DATETIME)
- `ergebnis_team1`, `ergebnis_team2` (INT, NULLable für noch offene Spiele)

**Tabelle `tipps`**
- `id` (INT, PK, Auto_Increment)
- `spiel_id` (INT, FK) – *Verknüpft den Tipp mit einem Spiel*
- `benutzername` (VARCHAR)
- `tipp_team1`, `tipp_team2` (INT)
- `punkte` (INT, Default 0) – *Wird automatisch vom Programm berechnet und aktualisiert*

</details>

### Architektur & Layout-Stabilität
Um zu verhindern, dass das Layout auf virtuellen Maschinen (VMs) mit aktivierter Windows-Skalierung (z.B. 125% / 150%) zusammenbricht, nutzt die App **PerMonitorV2**. Das gesamte UI ist flexibel mit `TableLayoutPanel` und `Dock=Fill` aufgebaut. Es gibt keine starren `x/y`-Koordinaten, wodurch das Programm immer proportional perfekt skaliert!

---

## 🗺️ Roadmap & offene Punkte (Entwicklungsstand: Mai 2026)

**Nächste geplante Features:**
- [ ] Ergebnisse nachträglich korrigierbar machen (Edit-Funktion).
- [ ] Datum-Validierung (vergangene Daten beim Anlegen von Spielen blockieren).
- [ ] Visuelles Feedback in Dropdowns, wenn Spiele bereits ausgewertet wurden.
- [ ] CSV/Excel-Export der Rangliste hinzufügen.
- [ ] Echtes Benutzer-Login-System implementieren.

---

<div align="center">
  <i>Entwickelt mit ❤️ für das Schulprojekt & die Weltmeisterschaft 2026.</i><br>
  <b>Ein Projekt von voiscko (Telegram: <a href="https://t.me/voiscko">@voiscko</a>)</b>
</div>
