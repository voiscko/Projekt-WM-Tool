using System;
using System.Drawing;
using System.Windows.Forms;
using WMTippTool.Database;

namespace WMTippTool.Forms
{
    /// <summary>
    /// Dieses Fenster ist das SQL-Terminal. Es zeigt alle Datenbankbefehle in Echtzeit an.
    /// Es soll wie ein "Hacker-Terminal" aussehen, also schwarzer Hintergrund und grüne Schrift.
    /// </summary>
    public class LogForm : Form
    {
        // Eine Textbox, die über mehrere Zeilen geht. Hier werden die Logs reingeschrieben.
        private TextBox txtLogs = null!;

        // Der Konstruktor (Wird aufgerufen, wenn das Fenster erstellt wird)
        public LogForm()
        {
            InitializeComponent();
            
            // Wenn das Fenster geladen wird, holen wir uns alle bisherigen Logs
            // und schreiben sie in unsere Textbox.
            foreach (var log in QueryLogger.GetAllLogs())
            {
                AppendLog(log);
            }

            // Hier melden wir uns beim Event des QueryLoggers an.
            // Das bedeutet: Wenn im QueryLogger 'OnLogAdded' ausgelöst wird,
            // soll automatisch unsere Methode 'QueryLogger_OnLogAdded' ausgeführt werden.
            QueryLogger.OnLogAdded += QueryLogger_OnLogAdded;
        }

        private void InitializeComponent()
        {
            // Fenster-Eigenschaften festlegen
            this.Text = "🖥️ SQL-Terminal (Logs)";
            this.Size = new Size(800, 500); // Standardgröße des Fensters
            this.MinimumSize = new Size(400, 300); // So klein darf das Fenster minimal werden
            this.StartPosition = FormStartPosition.CenterScreen; // Fenster startet in der Mitte des Bildschirms
            this.BackColor = Color.Black; // Hintergrundfarbe schwarz für den Terminal-Look
            this.AutoScaleMode = AutoScaleMode.Dpi; // Sorgt dafür, dass es auf allen Monitoren gut skaliert

            // Die Textbox für die Logs konfigurieren
            txtLogs = new TextBox
            {
                Dock = DockStyle.Fill, // Füllt das komplette Fenster aus
                Multiline = true, // Erlaubt mehrere Zeilen
                ReadOnly = true, // Der Nutzer darf hier nichts reintippen, nur lesen!
                ScrollBars = ScrollBars.Vertical, // Fügt einen Scrollbalken hinzu, wenn der Text zu lang wird
                BackColor = Color.Black, // Schwarzer Hintergrund
                ForeColor = Color.Lime, // Grüne Schrift (wie im echten Terminal)
                Font = new Font("Consolas", 11, FontStyle.Regular), // Eine Programmierer-Schriftart
                BorderStyle = BorderStyle.None, // Kein hässlicher Rand
                Margin = new Padding(10)
            };

            // Füge die Textbox zum Fenster hinzu
            this.Controls.Add(txtLogs);
        }

        /// <summary>
        /// Diese Methode wird aufgerufen, sobald ein neuer Log-Eintrag existiert.
        /// </summary>
        private void QueryLogger_OnLogAdded(string message)
        {
            // Da das Event von einem anderen Thread (Hintergrundprozess) aufgerufen werden könnte,
            // müssen wir sicherstellen, dass wir die Textbox nur vom Haupt-Thread aus verändern.
            // 'InvokeRequired' prüft, ob wir im falschen Thread sind.
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => AppendLog(message)));
            }
            else
            {
                AppendLog(message);
            }
        }

        /// <summary>
        /// Schreibt den neuen Text unten an die Textbox dran und scrollt nach unten.
        /// </summary>
        private void AppendLog(string message)
        {
            txtLogs.AppendText(message + Environment.NewLine);
        }

        /// <summary>
        /// Wird aufgerufen, wenn das Fenster geschlossen wird.
        /// </summary>
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            // Wir müssen uns wieder vom Event abmelden!
            // Wenn wir das nicht tun, versucht der QueryLogger später, 
            // an ein geschlossenes Fenster Nachrichten zu schicken, was zu einem Absturz führt.
            QueryLogger.OnLogAdded -= QueryLogger_OnLogAdded;
            
            base.OnFormClosing(e);
        }
    }
}
