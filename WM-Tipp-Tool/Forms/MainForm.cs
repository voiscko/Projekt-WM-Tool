using System;
using System.Drawing;
using System.Windows.Forms;
using WMTippTool.Database;

/*
 * Projekt: WM-Tipp-Tool 2026
 * Entwickler: voiscko (Telegram: @voiscko)
 */

namespace WMTippTool.Forms
{
    /// <summary>
    /// Das ist das Hauptfenster (MainForm) unseres Programms.
    /// Es ist das allererste Fenster, das sich öffnet, wenn wir das Programm starten.
    /// Von hier aus können wir über Buttons in die anderen Bereiche navigieren.
    /// </summary>
    public class MainForm : Form
    {
        // Hier deklarieren wir unsere Buttons.
        // Das bedeutet: Wir sagen C#, dass es diese Buttons geben wird,
        // aber wir bauen sie erst später zusammen.
        private Button btnSpiele;
        private Button btnTippen;
        private Button btnRangliste;
        private Button btnProtokolle;     // Button für das SQL-Terminal
        private Button btnDatenbank; // Button für die Datenbank-Ansicht

        // Ein Label (Textfeld) unten, um anzuzeigen, ob die Datenbank verbunden ist
        private Label lblDatenbankStatus;

        // Der Konstruktor: Das ist die Methode, die aufgerufen wird,
        // sobald das Fenster aus dem Nichts "erschaffen" (instanziiert) wird.
        public MainForm()
        {
            // Baut alle Buttons und Layouts auf
            KomponentenInitialisieren();

            // Prüft, ob MySQL erreichbar ist
            DatenbankStatusPruefen();
        }

        /// <summary>
        /// Hier wird das komplette Design des Fensters gebaut.
        /// </summary>
        private void KomponentenInitialisieren()
        {
            // Fenster-Titel und Größe einstellen
            this.Text = "⚽ WM-Tipp-Tool 2026";
            this.MinimumSize = new Size(500, 650);
            this.Size = new Size(620, 750);
            this.StartPosition = FormStartPosition.CenterScreen; // Start in der Mitte des Monitors
            this.BackColor = Color.FromArgb(18, 18, 30);          // Dunkelblauer Hintergrund

            // AutoScaleMode.Dpi sorgt dafür, dass unser Fenster nicht verpixelt,
            // falls jemand einen Monitor mit Skalierung (z.B. 150%) benutzt.
            this.AutoScaleMode = AutoScaleMode.Dpi;

            // ===== Root TableLayoutPanel =====
            // Das TableLayoutPanel ist wie eine Excel-Tabelle.
            // Es ordnet unsere Elemente (Buttons, Text) sauber untereinander an.
            TableLayoutPanel hauptLayout = new TableLayoutPanel();
            hauptLayout.Dock = DockStyle.Fill; // Füllt das ganze Fenster aus
            hauptLayout.ColumnCount = 1;       // Wir brauchen nur eine einzige Spalte
            hauptLayout.RowCount = 8;          // Wir haben 8 Zeilen! (Header, 5 Buttons, DB-Status, Footer)
            hauptLayout.BackColor = Color.FromArgb(18, 18, 30);
            hauptLayout.Padding = new Padding(0);

            // Hier geben wir den Zeilen ihre Höhe.
            // "Absolute" heißt feste Pixelhöhe. "Percent" heißt, der Restplatz wird prozentual aufgeteilt.
            hauptLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 130f)); // Header
            hauptLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 20f));   // Btn Spiele
            hauptLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 20f));   // Btn Tippen
            hauptLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 20f));   // Btn Rangliste
            hauptLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 20f));   // Btn Datenbank
            hauptLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 20f));   // Btn Logs
            hauptLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 38f));  // DB-Status
            hauptLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 30f));  // Footer
            hauptLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f)); // Spalte kriegt 100% Breite

            // ----- Header -----
            // Der obere Bereich mit dem großen Text
            Panel pnlKopfbereich = new Panel();
            pnlKopfbereich.Dock = DockStyle.Fill;
            pnlKopfbereich.BackColor = Color.FromArgb(30, 30, 50);

            Label lblTitel = new Label();
            lblTitel.Text = "⚽ WM-Tipp-Tool 2026";
            lblTitel.Font = new Font("Segoe UI", 26, FontStyle.Bold);
            lblTitel.ForeColor = Color.FromArgb(255, 210, 0); // Gelbe Schrift
            lblTitel.Dock = DockStyle.Fill;
            lblTitel.TextAlign = ContentAlignment.MiddleCenter;

            pnlKopfbereich.Controls.Add(lblTitel); // Label ins Panel stecken

            // ----- Menü-Buttons -----
            // Wir erstellen die Buttons und geben ihnen eine Farbe.
            btnSpiele = MenueButtonErstellen("🏟️  Spiele verwalten", Color.FromArgb(0, 120, 215));
            // Was passiert, wenn man klickt? Ein neues Fenster "SpielForm" öffnet sich!
            btnSpiele.Click += BtnSpiele_Klick;

            btnTippen = MenueButtonErstellen("✏️  Tipp abgeben", Color.FromArgb(16, 137, 62));
            btnTippen.Click += BtnTippen_Klick;

            btnRangliste = MenueButtonErstellen("🏆  Rangliste & Ergebnisse", Color.FromArgb(196, 43, 28));
            btnRangliste.Click += BtnRangliste_Klick;

            // Unsere 2 Buttons für die Präsentation!
            btnDatenbank = MenueButtonErstellen("🗄️  Datenbanken ansehen", Color.FromArgb(128, 0, 128)); // Lila
            btnDatenbank.Click += BtnDatenbank_Klick;

            btnProtokolle = MenueButtonErstellen("🖥️  SQL-Terminal (Logs)", Color.FromArgb(0, 0, 0)); // Schwarz (Hacker Style)
            btnProtokolle.Click += BtnProtokolle_Klick;

            // ----- DB-Status -----
            // Ein kleiner Text, der uns sagt, ob MySQL an ist.
            lblDatenbankStatus = new Label();
            lblDatenbankStatus.Text = "● Datenbankstatus wird geprüft...";
            lblDatenbankStatus.Font = new Font("Segoe UI", 10);
            lblDatenbankStatus.ForeColor = Color.Gray;
            lblDatenbankStatus.Dock = DockStyle.Fill;
            lblDatenbankStatus.TextAlign = ContentAlignment.MiddleCenter;

            // ----- Footer -----
            // Der kleine Text ganz unten im Fenster.
            Label lblFooter = new Label();
            lblFooter.Text = "WM-Tipp-Tool © 2026 — by voiscko (Telegram: @voiscko)";
            lblFooter.Font = new Font("Segoe UI", 9);
            lblFooter.ForeColor = Color.FromArgb(80, 80, 100);
            lblFooter.Dock = DockStyle.Fill;
            lblFooter.TextAlign = ContentAlignment.MiddleCenter;

            // Zum Schluss packen wir alles in unsere "Excel-Tabelle" (das hauptLayout).
            // Die Zahlen (z.B. 0, 0) bedeuten: Spalte 0, Zeile 0.
            // Spalte ist immer 0, weil wir nur eine haben. Die zweite Zahl ist die Zeile.
            hauptLayout.Controls.Add(pnlKopfbereich, 0, 0);

            // "ButtonVerpacken" ist unser kleiner Trick, damit die Buttons einen Abstand (Margin) zum Rand haben.
            hauptLayout.Controls.Add(ButtonVerpacken(btnSpiele), 0, 1);
            hauptLayout.Controls.Add(ButtonVerpacken(btnTippen), 0, 2);
            hauptLayout.Controls.Add(ButtonVerpacken(btnRangliste), 0, 3);
            hauptLayout.Controls.Add(ButtonVerpacken(btnDatenbank), 0, 4); // Datenbanken
            hauptLayout.Controls.Add(ButtonVerpacken(btnProtokolle), 0, 5);     // Logs

            hauptLayout.Controls.Add(lblDatenbankStatus, 0, 6); // Zeile 6
            hauptLayout.Controls.Add(lblFooter, 0, 7);   // Zeile 7

            // Und ab mit dem fertigen Layout in unser Fenster!
            this.Controls.Add(hauptLayout);
        }

        // ===== Button-Click-Methoden =====
        // Jede Methode öffnet ein neues Fenster (Dialog).

        private void BtnSpiele_Klick(object sender, EventArgs e)
        {
            SpielForm fenster = new SpielForm();
            fenster.ShowDialog();
        }

        private void BtnTippen_Klick(object sender, EventArgs e)
        {
            TippForm fenster = new TippForm();
            fenster.ShowDialog();
        }

        private void BtnRangliste_Klick(object sender, EventArgs e)
        {
            RanglisteForm fenster = new RanglisteForm();
            fenster.ShowDialog();
        }

        private void BtnDatenbank_Klick(object sender, EventArgs e)
        {
            DatenbankAnsichtForm fenster = new DatenbankAnsichtForm();
            fenster.ShowDialog();
        }

        private void BtnProtokolle_Klick(object sender, EventArgs e)
        {
            ProtokollForm fenster = new ProtokollForm();
            fenster.ShowDialog();
        }

        /// <summary>
        /// Eine kleine Hilfsmethode (Trick), um Buttons schöner aussehen zu lassen.
        /// Anstatt dass der Button das ganze Layout ausfüllt, packen wir ihn in ein "Panel"
        /// (eine Art Kiste) und füllen diese Kiste mit unsichtbarem Abstand (Padding) am Rand.
        /// </summary>
        private static Panel ButtonVerpacken(Button btn)
        {
            Panel pnl = new Panel();
            pnl.Dock = DockStyle.Fill;
            pnl.BackColor = Color.FromArgb(18, 18, 30);
            // Links und rechts 60 Pixel Abstand, oben/unten 10 Pixel.
            pnl.Padding = new Padding(60, 10, 60, 10);

            btn.Dock = DockStyle.Fill;
            pnl.Controls.Add(btn);
            return pnl;
        }

        /// <summary>
        /// Diese Methode ist quasi unsere "Button-Fabrik".
        /// Sie erstellt einen Button nach unseren Vorgaben (Text und Farbe)
        /// und spart uns so, dass wir den gleichen Code 5 mal schreiben müssen.
        /// </summary>
        private Button MenueButtonErstellen(string text, Color backColor)
        {
            Button btn = new Button();
            btn.Text = text;
            btn.Font = new Font("Segoe UI", 13, FontStyle.Bold);
            btn.ForeColor = Color.White;
            btn.BackColor = backColor;
            btn.FlatStyle = FlatStyle.Flat;
            btn.Cursor = Cursors.Hand; // Die Maus wird zu einer Hand, wenn man drüber fährt

            // Rand des Buttons unsichtbar machen
            btn.FlatAppearance.BorderSize = 0;

            // Coole Hover-Effekte (Wenn die Maus drüber geht, wird er heller).
            // Wir speichern die Originalfarbe, damit wir sie beim Verlassen wiederherstellen können.
            Color originalColor = backColor;
            btn.MouseEnter += delegate (object sender, EventArgs e)
            {
                btn.BackColor = ControlPaint.Light(originalColor, 0.15f);
            };
            btn.MouseLeave += delegate (object sender, EventArgs e)
            {
                btn.BackColor = originalColor; // Wieder normale Farbe
            };

            return btn;
        }

        /// <summary>
        /// Prüft, ob unser Programm mit MySQL reden kann.
        /// </summary>
        private void DatenbankStatusPruefen()
        {
            // Ruft die Methode aus unserer Datenbank-Klasse auf
            bool connected = DatenbankVerbindung.VerbindungTesten();
            if (connected)
            {
                lblDatenbankStatus.Text = "● Datenbankverbindung: Verbunden";
                lblDatenbankStatus.ForeColor = Color.FromArgb(0, 200, 100); // Grün = Alles okay!
            }
            else
            {
                lblDatenbankStatus.Text = "● Datenbankverbindung: Getrennt (XAMPP gestartet?)";
                lblDatenbankStatus.ForeColor = Color.FromArgb(220, 60, 60); // Rot = Fehler!
            }
        }
    }
}
