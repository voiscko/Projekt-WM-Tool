<div align="center">
  <img src="WM-Tipp-Tool/Cover.png" alt="WM-Tipp-Tool 2026 Cover" width="800" style="border-radius: 10px; box-shadow: 0 4px 8px rgba(0,0,0,0.1); margin-bottom: 20px;" />

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

## 📚 Dokumentation

Hier findest du alle Details zum Projekt, aufgeteilt in übersichtliche Bereiche (wie in Notion):

- 👥 **[Team & Aufgabenverteilung](Dokumentation/Team.md)** — Wer hat was gemacht?
- 🏗️ **[Architektur & Ordnerstruktur](Dokumentation/Architektur.md)** — Wie ist das Projekt aufgebaut und wie startet es?
- 🔑 **[Datenbank & Verbindung](Dokumentation/Datenbank.md)** — Alles über MySQL, Tabellen und den Protokollierer.
- 🖼️ **[Formulare & UI-Design](Dokumentation/Formulare.md)** — Details zu den Fenstern und dem Refactoring.
- 💯 **[Punktesystem](Dokumentation/Punktesystem.md)** — Wie werden die Punkte berechnet?

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

<div align="center">
  <i>Entwickelt mit ❤️ für das Schulprojekt & die Weltmeisterschaft 2026.</i><br>
  <b>Ein Projekt von voiscko (Telegram: <a href="https://t.me/voiscko">@voiscko</a>)</b>
</div>
