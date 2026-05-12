using System;
using System.Drawing;
using System.Windows.Forms;
using WMTippTool.Datenbank;

namespace WMTippTool.Formulare
{
    /// <summary>
    /// Dieses Fenster ist das SQL-Terminal. Es zeigt alle Datenbankbefehle in Echtzeit an.
    /// Es soll wie ein "Hacker-Terminal" aussehen, also schwarzer Hintergrund und grüne Schrift.
    /// </summary>
    public class ProtokollForm : Form
    {
        // Eine Textbox, die über mehrere Zeilen geht. Hier werden die Logs reingeschrieben.
        private TextBox txtProtokolle;

        // Der Konstruktor (Wird aufgerufen, wenn das Fenster erstellt wird)
        public ProtokollForm()
        {
            KomponentenInitialisieren();

            // Wenn das Fenster geladen wird, holen wir uns alle bisherigen Logs
            // und schreiben sie in unsere Textbox.
            foreach (string log in SQLProtokollierer.AlleProtokolleAbrufen())
            {
                ProtokollHinzufuegen(log);
            }

            // Hier melden wir uns beim Event des SQLProtokollierers an.
            // Das bedeutet: Wenn im SQLProtokollierer 'BeiNeuemProtokollEintrag' ausgelöst wird,
            // soll automatisch unsere Methode 'SQLProtokollierer_BeiNeuemProtokollEintrag' ausgeführt werden.
            SQLProtokollierer.BeiNeuemProtokollEintrag += SQLProtokollierer_BeiNeuemProtokollEintrag;
        }

        private void KomponentenInitialisieren()
        {
            // Fenster-Eigenschaften festlegen
            this.Text = "🖥️ SQL-Terminal (Logs)";
            this.Size = new Size(800, 500); // Standardgröße des Fensters
            this.MinimumSize = new Size(400, 300); // So klein darf das Fenster minimal werden
            this.StartPosition = FormStartPosition.CenterScreen; // Fenster startet in der Mitte des Bildschirms
            this.BackColor = Color.Black; // Hintergrundfarbe schwarz für den Terminal-Look
            this.AutoScaleMode = AutoScaleMode.Dpi; // Sorgt dafür, dass es auf allen Monitoren gut skaliert

            // Die Textbox für die Logs konfigurieren
            txtProtokolle = new TextBox();
            txtProtokolle.Dock = DockStyle.Fill;        // Füllt das komplette Fenster aus
            txtProtokolle.Multiline = true;              // Erlaubt mehrere Zeilen
            txtProtokolle.ReadOnly = true;               // Der Nutzer darf hier nichts reintippen, nur lesen!
            txtProtokolle.ScrollBars = ScrollBars.Vertical; // Fügt einen Scrollbalken hinzu, wenn der Text zu lang wird
            txtProtokolle.BackColor = Color.Black;       // Schwarzer Hintergrund
            txtProtokolle.ForeColor = Color.Lime;        // Grüne Schrift (wie im echten Terminal)
            txtProtokolle.Font = new Font("Consolas", 11, FontStyle.Regular); // Eine Programmierer-Schriftart
            txtProtokolle.BorderStyle = BorderStyle.None; // Kein hässlicher Rand
            txtProtokolle.Margin = new Padding(10);

            // Füge die Textbox zum Fenster hinzu
            this.Controls.Add(txtProtokolle);
        }

        /// <summary>
        /// Diese Methode wird aufgerufen, sobald ein neuer Protokollieren-Eintrag existiert.
        /// </summary>
        private void SQLProtokollierer_BeiNeuemProtokollEintrag(string message)
        {
            // Da das Event von einem anderen Thread (Hintergrundprozess) aufgerufen werden könnte,
            // müssen wir sicherstellen, dass wir die Textbox nur vom Haupt-Thread aus verändern.
            // 'InvokeRequired' prüft, ob wir im falschen Thread sind.
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(delegate () { ProtokollHinzufuegen(message); }));
            }
            else
            {
                ProtokollHinzufuegen(message);
            }
        }

        /// <summary>
        /// Schreibt den neuen Text unten an die Textbox dran und scrollt nach unten.
        /// </summary>
        private void ProtokollHinzufuegen(string message)
        {
            txtProtokolle.AppendText(message + Environment.NewLine);
        }

        /// <summary>
        /// Wird aufgerufen, wenn das Fenster geschlossen wird.
        /// </summary>
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            // Wir müssen uns wieder vom Event abmelden!
            // Wenn wir das nicht tun, versucht der SQLProtokollierer später,
            // an ein geschlossenes Fenster Nachrichten zu schicken, was zu einem Absturz führt.
            SQLProtokollierer.BeiNeuemProtokollEintrag -= SQLProtokollierer_BeiNeuemProtokollEintrag;

            base.OnFormClosing(e);
        }
    }
}
