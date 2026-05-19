# 🖼️ Formulare (Fenster)

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
| [`TippForm.cs`](../WM-Tipp-Tool/Formulare/TippForm.cs) (Logik) | 515 Zeilen | **160 Zeilen** | Aylin |
| [`SpielForm.cs`](../WM-Tipp-Tool/Formulare/SpielForm.cs) (Logik) | 450 Zeilen | **131 Zeilen** | Lian |
| [`TippFormDesign.cs`](../WM-Tipp-Tool/Formulare/TippFormDesign.cs) | - | ~180 Zeilen (neu) | Mark |
| [`SpielFormDesign.cs`](../WM-Tipp-Tool/Formulare/SpielFormDesign.cs) | - | ~160 Zeilen (neu) | Mark |
| [`DesignHelper.cs`](../WM-Tipp-Tool/Formulare/DesignHelper.cs) | - | Zentrale Design-Logik | Mark |

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

Das `ProtokollForm` meldet sich beim **Event** des `SQLProtokollierer`s an:

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
