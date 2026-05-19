# 🔑 Datenbank & Verbindung

## 🔌 Die Datenbankverbindung — `DatenbankVerbindung.cs`

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

## 📋 Der SQL-Protokollierer — `SQLProtokollierer.cs`

Diese Klasse ist unser **digitales Notizbuch**. Jedes Mal, wenn irgendwo im Programm ein SQL-Befehl an MySQL geschickt wird, rufen wir hier `Protokollieren(...)` auf.

```csharp
// Beispiel aus SpielForm.cs:
SQLProtokollierer.Protokollieren("INSERT INTO spiele ...");
```

Die Klasse speichert alle Einträge in einer Liste. Wenn das `ProtokollForm` (das Terminal-Fenster) geöffnet wird, zeigt es alle gesammelten Einträge an.

**Das besondere Feature:** Ein **Event** (`BeiNeuemProtokollEintrag`). Das ist wie ein Alarm. Das `ProtokollForm` meldet sich bei diesem Alarm an. Sobald ein neuer Befehl protokolliert wird, klingelt der Alarm und der neue Eintrag erscheint **sofort in Echtzeit** im Terminal-Fenster, ohne dass man auf Aktualisieren drücken muss.

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
