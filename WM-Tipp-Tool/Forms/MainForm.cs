using System;
using System.Drawing;
using System.Windows.Forms;
using WMTippTool.Database;

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
        private Button btnSpiele = null!;
        private Button btnTippen = null!;
        private Button btnRangliste = null!;
        private Button btnLogs = null!; // NEU: Button für das SQL-Terminal
        private Button btnDatabase = null!; // NEU: Button für die Datenbank-Ansicht
        
        // Ein Label (Textfeld) unten, um anzuzeigen, ob die Datenbank verbunden ist
        private Label lblDbStatus = null!;

        // Der Konstruktor: Das ist die Methode, die aufgerufen wird,
        // sobald das Fenster aus dem Nichts "erschaffen" (instanziiert) wird.
        public MainForm()
        {
            // Baut alle Buttons und Layouts auf
            InitializeComponent();
            
            // Prüft, ob MySQL erreichbar ist
            CheckDbStatus();
        }

        /// <summary>
        /// Hier wird das komplette Design des Fensters gebaut.
        /// </summary>
        private void InitializeComponent()
        {
            // Fenster-Titel und Größe einstellen
            this.Text = "⚽ WM-Tipp-Tool 2026";
            this.MinimumSize = new Size(500, 650);
            this.Size = new Size(620, 750); // Etwas größer für die 2 neuen Buttons
            this.StartPosition = FormStartPosition.CenterScreen; // Start in der Mitte des Monitors
            this.BackColor = Color.FromArgb(18, 18, 30); // Dunkelblauer Hintergrund
            
            // AutoScaleMode.Dpi sorgt dafür, dass unser Fenster nicht verpixelt, 
            // falls jemand einen Monitor mit Skalierung (z.B. 150%) benutzt.
            this.AutoScaleMode = AutoScaleMode.Dpi;

            // ===== Root TableLayoutPanel =====
            // Das TableLayoutPanel ist wie eine Excel-Tabelle. 
            // Es ordnet unsere Elemente (Buttons, Text) sauber untereinander an.
            var rootLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill, // Füllt das ganze Fenster aus
                ColumnCount = 1, // Wir brauchen nur eine einzige Spalte
                RowCount = 8, // Wir haben jetzt 8 Zeilen! (Header, 5 Buttons, DB-Status, Footer)
                BackColor = Color.FromArgb(18, 18, 30),
                Padding = new Padding(0)
            };

            // Hier geben wir den Zeilen ihre Höhe.
            // "Absolute" heißt feste Pixelhöhe. "Percent" heißt, der Restplatz wird prozentual aufgeteilt.
            rootLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 130f)); // Header
            rootLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 20f));   // Btn Spiele
            rootLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 20f));   // Btn Tippen
            rootLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 20f));   // Btn Rangliste
            rootLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 20f));   // Btn Datenbank
            rootLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 20f));   // Btn Logs
            rootLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 38f));  // DB-Status
            rootLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 30f));  // Footer
            rootLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f)); // Spalte kriegt 100% Breite

            // ----- Header -----
            // Der obere Bereich mit dem großen Text
            var pnlHeader = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(30, 30, 50)
            };
            var lblTitel = new Label
            {
                Text = "⚽ WM-Tipp-Tool 2026",
                Font = new Font("Segoe UI", 26, FontStyle.Bold),
                ForeColor = Color.FromArgb(255, 210, 0), // Gelbe Schrift
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter
            };
            pnlHeader.Controls.Add(lblTitel); // Label ins Panel stecken

            // ----- Menü-Buttons -----
            // Wir erstellen die Buttons und geben ihnen eine Farbe.
            btnSpiele = CreateMenuButton("🏟️  Spiele verwalten", Color.FromArgb(0, 120, 215));
            // Was passiert, wenn man klickt? Ein neues Fenster "SpielForm" öffnet sich!
            btnSpiele.Click += (s, e) => new SpielForm().ShowDialog();

            btnTippen = CreateMenuButton("✏️  Tipp abgeben", Color.FromArgb(16, 137, 62));
            btnTippen.Click += (s, e) => new TippForm().ShowDialog();

            btnRangliste = CreateMenuButton("🏆  Rangliste & Ergebnisse", Color.FromArgb(196, 43, 28));
            btnRangliste.Click += (s, e) => new RanglisteForm().ShowDialog();

            // Unsere 2 NEUEN Buttons für die Präsentation!
            btnDatabase = CreateMenuButton("🗄️  Datenbanken ansehen", Color.FromArgb(128, 0, 128)); // Lila
            btnDatabase.Click += (s, e) => new DatabaseViewForm().ShowDialog();

            btnLogs = CreateMenuButton("🖥️  SQL-Terminal (Logs)", Color.FromArgb(0, 0, 0)); // Schwarz (Hacker Style)
            btnLogs.Click += (s, e) => new LogForm().ShowDialog(); // LogForm öffnet sich separat

            // ----- DB-Status -----
            // Ein kleiner Text, der uns sagt, ob MySQL an ist.
            lblDbStatus = new Label
            {
                Text = "● Datenbankstatus wird geprüft...",
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.Gray,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter
            };

            // ----- Footer -----
            // Der kleine Text ganz unten im Fenster.
            var lblFooter = new Label
            {
                Text = "WM-Tipp-Tool © 2026 — Schulprojekt Präsentation",
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.FromArgb(80, 80, 100),
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter
            };

            // Zum Schluss packen wir alles in unsere "Excel-Tabelle" (das rootLayout).
            // Die Zahlen (z.B. 0, 0) bedeuten: Spalte 0, Zeile 0.
            // Spalte ist immer 0, weil wir nur eine haben. Die zweite Zahl ist die Zeile.
            rootLayout.Controls.Add(pnlHeader, 0, 0);
            
            // "WrapButton" ist unser kleiner Trick, damit die Buttons einen Abstand (Margin) zum Rand haben.
            rootLayout.Controls.Add(WrapButton(btnSpiele), 0, 1);
            rootLayout.Controls.Add(WrapButton(btnTippen), 0, 2);
            rootLayout.Controls.Add(WrapButton(btnRangliste), 0, 3);
            rootLayout.Controls.Add(WrapButton(btnDatabase), 0, 4); // Neu: Datenbanken
            rootLayout.Controls.Add(WrapButton(btnLogs), 0, 5);     // Neu: Logs
            
            rootLayout.Controls.Add(lblDbStatus, 0, 6); // Zeile 6
            rootLayout.Controls.Add(lblFooter, 0, 7);   // Zeile 7

            // Und ab mit dem fertigen Layout in unser Fenster!
            this.Controls.Add(rootLayout);
        }

        /// <summary>
        /// Eine kleine Hilfsmethode (Trick), um Buttons schöner aussehen zu lassen.
        /// Anstatt dass der Button das ganze Layout ausfüllt, packen wir ihn in ein "Panel" 
        /// (eine Art Kiste) und füllen diese Kiste mit unsichtbarem Abstand (Padding) am Rand.
        /// </summary>
        private static Panel WrapButton(Button btn)
        {
            var pnl = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(18, 18, 30),
                // Links und rechts 60 Pixel Abstand, oben/unten 10 Pixel.
                Padding = new Padding(60, 10, 60, 10) 
            };
            btn.Dock = DockStyle.Fill;
            pnl.Controls.Add(btn);
            return pnl;
        }

        /// <summary>
        /// Diese Methode ist quasi unsere "Button-Fabrik".
        /// Sie erstellt einen Button nach unseren Vorgaben (Text und Farbe)
        /// und spart uns so, dass wir den gleichen Code 5 mal schreiben müssen.
        /// </summary>
        private Button CreateMenuButton(string text, Color backColor)
        {
            var btn = new Button
            {
                Text = text,
                Font = new Font("Segoe UI", 13, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = backColor,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand // Die Maus wird zu einer Hand, wenn man drüber fährt
            };
            // Rand des Buttons unsichtbar machen
            btn.FlatAppearance.BorderSize = 0;

            // Coole Hover-Effekte (Wenn die Maus drüber geht, wird er heller)
            btn.MouseEnter += (s, e) => btn.BackColor = ControlPaint.Light(backColor, 0.15f);
            btn.MouseLeave += (s, e) => btn.BackColor = backColor; // Wieder normale Farbe

            return btn;
        }

        /// <summary>
        /// Prüft, ob unser Programm mit MySQL reden kann.
        /// </summary>
        private void CheckDbStatus()
        {
            // Ruft die Methode aus unserer Datenbank-Klasse auf
            bool connected = DBConnection.TestConnection();
            if (connected)
            {
                lblDbStatus.Text = "● Datenbankverbindung: Verbunden";
                lblDbStatus.ForeColor = Color.FromArgb(0, 200, 100); // Grün = Alles okay!
            }
            else
            {
                lblDbStatus.Text = "● Datenbankverbindung: Getrennt (XAMPP gestartet?)";
                lblDbStatus.ForeColor = Color.FromArgb(220, 60, 60); // Rot = Fehler!
            }
        }
    }
}
