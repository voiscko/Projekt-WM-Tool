using System;
using System.Drawing;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using WMTippTool.Database;

namespace WMTippTool.Forms
{
    /// <summary>
    /// In diesem Fenster (Form) verwalten wir die Spiele.
    /// Man kann neue Spiele anlegen, löschen und die Liste aller Spiele sehen.
    /// </summary>
    public class SpielForm : Form
    {
        // === Unsere Bausteine (Controls) für dieses Fenster ===
        // Eine Tabelle (DataGridView), in der alle Spiele angezeigt werden.
        private DataGridView dgvSpiele;
        // Zwei Eingabefelder für die Teamnamen
        private TextBox txtTeam1;
        private TextBox txtTeam2;
        // Ein Feld für das Datum und die Uhrzeit
        private DateTimePicker dtpDatum;
        // Buttons für unsere Aktionen
        private Button btnHinzufuegen;
        private Button btnLoeschen;
        private Button btnAktualisieren;

        // Der Konstruktor: Wird ausgeführt, wenn das Fenster geöffnet wird.
        public SpielForm()
        {
            KomponentenInitialisieren(); // Baut das Fenster auf (Buttons, Farben etc.)
            LadeSpiele();          // Holt sofort die Spiele aus der Datenbank und zeigt sie an
        }

        /// <summary>
        /// Hier wird das Aussehen des Fensters "zusammengebaut".
        /// </summary>
        private void KomponentenInitialisieren()
        {
            // Fenstereigenschaften
            this.Text = "🏟️ Spiele verwalten";
            this.MinimumSize = new Size(800, 580);
            this.Size = new Size(1100, 780);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(22, 22, 35); // Dunkler Hintergrund
            this.ForeColor = Color.White;
            this.AutoScaleMode = AutoScaleMode.Dpi;

            // ===== Haupt-Layout =====
            // Wieder eine "Excel-Tabelle" (TableLayoutPanel), um die Bereiche zu unterteilen.
            TableLayoutPanel hauptLayout = new TableLayoutPanel();
            hauptLayout.Dock = DockStyle.Fill;
            hauptLayout.ColumnCount = 1;
            hauptLayout.RowCount = 4;
            hauptLayout.BackColor = Color.FromArgb(22, 22, 35);
            hauptLayout.Padding = new Padding(14, 10, 14, 10);

            // Wir haben 4 Zeilen: Titel | Eingabefelder | Die Tabelle mit den Spielen | Buttons unten
            hauptLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 52f));   // Titel
            hauptLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 165f));  // Eingabe-Panel
            hauptLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));   // Die Tabelle
            hauptLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 55f));   // Buttons
            hauptLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));

            // ----- 1. Titel oben -----
            Label lblTitel = new Label();
            lblTitel.Text = "🏟️ Spiele verwalten";
            lblTitel.Font = new Font("Segoe UI", 20, FontStyle.Bold);
            lblTitel.ForeColor = Color.FromArgb(255, 210, 0);
            lblTitel.Dock = DockStyle.Fill;
            lblTitel.TextAlign = ContentAlignment.MiddleLeft;

            // ----- 2. Eingabebereich -----
            // Ein Kasten (Panel) für die Eingaben (Team 1, Team 2, Datum)
            Panel pnlEingabe = new Panel();
            pnlEingabe.Dock = DockStyle.Fill;
            pnlEingabe.BackColor = Color.FromArgb(35, 35, 55);
            pnlEingabe.BorderStyle = BorderStyle.FixedSingle;
            pnlEingabe.Padding = new Padding(14, 10, 14, 10);

            // Ein kleines Layout im Eingabekasten, um alles ordentlich anzuordnen
            TableLayoutPanel eingabeLayout = new TableLayoutPanel();
            eingabeLayout.Dock = DockStyle.Fill;
            eingabeLayout.ColumnCount = 4; // 4 Spalten (z.B. Label "Team 1" | Textbox | Label "Team 2" | Textbox)
            eingabeLayout.RowCount = 3;
            eingabeLayout.BackColor = Color.FromArgb(35, 35, 55);

            eingabeLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 75f));
            eingabeLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50f));
            eingabeLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 75f));
            eingabeLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50f));
            eingabeLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 33.3f));
            eingabeLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 33.3f));
            eingabeLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 33.4f));

            // Eingabefelder erstellen
            Label lblTeam1 = LabelStylen("Team 1:");
            txtTeam1 = TextFeldStylen();
            Label lblTeam2 = LabelStylen("Team 2:");
            txtTeam2 = TextFeldStylen();

            eingabeLayout.Controls.Add(lblTeam1, 0, 0);
            eingabeLayout.Controls.Add(txtTeam1, 1, 0);
            eingabeLayout.Controls.Add(lblTeam2, 2, 0);
            eingabeLayout.Controls.Add(txtTeam2, 3, 0);

            // Datumseingabe
            Label lblDatum = LabelStylen("Datum:");
            dtpDatum = new DateTimePicker();
            dtpDatum.Dock = DockStyle.Fill;
            dtpDatum.Format = DateTimePickerFormat.Custom;
            dtpDatum.CustomFormat = "dd.MM.yyyy HH:mm"; // Format: Tag.Monat.Jahr Stunde:Minute
            dtpDatum.ShowUpDown = false;
            dtpDatum.CalendarForeColor = Color.White;
            dtpDatum.CalendarTitleBackColor = Color.FromArgb(0, 120, 215);
            dtpDatum.BackColor = Color.FromArgb(50, 50, 70);
            dtpDatum.ForeColor = Color.White;
            dtpDatum.Font = new Font("Segoe UI", 11);
            dtpDatum.Margin = new Padding(0, 4, 0, 4);

            Panel dtpPanel = new Panel();
            dtpPanel.Dock = DockStyle.Fill;
            dtpPanel.Controls.Add(dtpDatum);
            dtpDatum.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom;
            dtpDatum.Location = new Point(0, 4);
            dtpDatum.Width = dtpPanel.Width;
            dtpPanel.SizeChanged += DtpPanel_GroesseGeaendert;

            eingabeLayout.Controls.Add(lblDatum, 0, 1);
            eingabeLayout.Controls.Add(dtpPanel, 1, 1);

            Label lblDatumHint = new Label();
            lblDatumHint.Text = "Format: TT.MM.JJJJ HH:MM";
            lblDatumHint.Font = new Font("Segoe UI", 9, FontStyle.Italic);
            lblDatumHint.ForeColor = Color.FromArgb(120, 120, 150);
            lblDatumHint.Dock = DockStyle.Fill;
            lblDatumHint.TextAlign = ContentAlignment.MiddleLeft;
            lblDatumHint.Padding = new Padding(8, 0, 0, 0);

            eingabeLayout.SetColumnSpan(dtpPanel, 1);
            eingabeLayout.Controls.Add(lblDatumHint, 2, 1);
            eingabeLayout.SetColumnSpan(lblDatumHint, 2);

            // "Hinzufügen" Button
            btnHinzufuegen = new Button();
            btnHinzufuegen.Text = "➕ Spiel hinzufügen";
            btnHinzufuegen.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            btnHinzufuegen.BackColor = Color.FromArgb(0, 120, 215);
            btnHinzufuegen.ForeColor = Color.White;
            btnHinzufuegen.FlatStyle = FlatStyle.Flat;
            btnHinzufuegen.Cursor = Cursors.Hand;
            btnHinzufuegen.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Bottom;
            btnHinzufuegen.Margin = new Padding(0, 4, 0, 4);
            btnHinzufuegen.FlatAppearance.BorderSize = 0;
            // Wenn der Button geklickt wird, rufe "BtnHinzufuegen_Klick" auf!
            btnHinzufuegen.Click += BtnHinzufuegen_Klick;

            eingabeLayout.Controls.Add(btnHinzufuegen, 0, 2);
            eingabeLayout.SetColumnSpan(btnHinzufuegen, 2);

            pnlEingabe.Controls.Add(eingabeLayout);

            // ----- 3. Tabelle (DataGridView) -----
            dgvSpiele = new DataGridView();
            dgvSpiele.Dock = DockStyle.Fill;
            dgvSpiele.BackgroundColor = Color.FromArgb(28, 28, 45);
            dgvSpiele.BorderStyle = BorderStyle.None;
            dgvSpiele.RowHeadersVisible = false;
            dgvSpiele.AllowUserToAddRows = false;    // Nicht direkt in der Tabelle schreiben
            dgvSpiele.AllowUserToDeleteRows = false;
            dgvSpiele.ReadOnly = true;
            dgvSpiele.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvSpiele.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvSpiele.Font = new Font("Segoe UI", 10);
            dgvSpiele.GridColor = Color.FromArgb(60, 60, 80);
            dgvSpiele.Margin = new Padding(0, 8, 0, 8);
            TabelleStylen(dgvSpiele);

            // ----- 4. Buttons unten (Löschen, Aktualisieren, Zurück) -----
            FlowLayoutPanel btnPanel = new FlowLayoutPanel();
            btnPanel.Dock = DockStyle.Fill;
            btnPanel.FlowDirection = FlowDirection.LeftToRight;
            btnPanel.BackColor = Color.FromArgb(22, 22, 35);
            btnPanel.WrapContents = false;
            btnPanel.Padding = new Padding(0, 6, 0, 0);

            btnLoeschen = AktionsButtonErstellen("🗑️ Ausgewähltes Spiel löschen", Color.FromArgb(180, 40, 40));
            btnLoeschen.Click += BtnLoeschen_Klick;

            btnAktualisieren = AktionsButtonErstellen("🔄 Aktualisieren", Color.FromArgb(60, 60, 90));
            btnAktualisieren.Click += BtnAktualisieren_Klick;

            Button btnZurueck = AktionsButtonErstellen("← Zurück zum Menü", Color.FromArgb(50, 50, 75));
            btnZurueck.Click += BtnZurueck_Klick; // Schließt dieses Fenster

            btnPanel.Controls.AddRange(new Control[] { btnLoeschen, btnAktualisieren, btnZurueck });

            // Alles in unser RootLayout stecken
            hauptLayout.Controls.Add(lblTitel, 0, 0);
            hauptLayout.Controls.Add(pnlEingabe, 0, 1);
            hauptLayout.Controls.Add(dgvSpiele, 0, 2);
            hauptLayout.Controls.Add(btnPanel, 0, 3);

            this.Controls.Add(hauptLayout);
        }

        // Wenn die Panel-Größe sich ändert, passen wir auch die Breite des DateTimePickers an.
        private void DtpPanel_GroesseGeaendert(object sender, EventArgs e)
        {
            Panel pnl = (Panel)sender;
            dtpDatum.Width = pnl.Width;
        }

        private void BtnAktualisieren_Klick(object sender, EventArgs e)
        {
            LadeSpiele();
        }

        private void BtnZurueck_Klick(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Wird ausgeführt, wenn man auf "Spiel hinzufügen" drückt.
        /// Nimmt die Eingaben und speichert sie in der Datenbank.
        /// </summary>
        private void BtnHinzufuegen_Klick(object sender, EventArgs e)
        {
            // "Trim" entfernt unsichtbare Leerzeichen am Anfang und Ende
            string team1 = txtTeam1.Text.Trim();
            string team2 = txtTeam2.Text.Trim();

            // Sicherheitscheck: Wurde überhaupt was eingegeben?
            if (string.IsNullOrEmpty(team1) || string.IsNullOrEmpty(team2))
            {
                MessageBox.Show("Bitte beide Teamnamen eingeben!", "Pflichtfelder fehlen",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return; // Bricht ab, nichts passiert
            }

            // Ein Team kann nicht gegen sich selbst spielen :)
            if (team1 == team2)
            {
                MessageBox.Show("Team 1 und Team 2 dürfen nicht identisch sein!", "Ungültige Eingabe",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // Verbindung zur Datenbank aufbauen
                using (MySqlConnection conn = DatenbankVerbindung.VerbindungAbrufen())
                {
                    conn.Open();

                    // Der SQL-Befehl (INSERT). Die @-Werte sind Platzhalter, um uns vor Hacks zu schützen!
                    string sql = "INSERT INTO spiele (team1, team2, datum) VALUES (@t1, @t2, @datum)";

                    // Wir schicken den SQL-Befehl ins Protokollieren, damit man das im Terminal sehen kann!
                    string logSql = "INSERT INTO spiele (team1, team2, datum) VALUES ('"
                        + team1 + "', '" + team2 + "', '"
                        + dtpDatum.Value.ToString("yyyy-MM-dd HH:mm:ss") + "')";
                    SQLProtokollierer.Protokollieren(logSql);

                    // Den echten Befehl an die Datenbank schicken
                    using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@t1", team1);
                        cmd.Parameters.AddWithValue("@t2", team2);
                        cmd.Parameters.AddWithValue("@datum", dtpDatum.Value);
                        cmd.ExecuteNonQuery(); // "Los, mach es!"
                    }

                    // Erfolg anzeigen
                    MessageBox.Show("Spiel '" + team1 + " vs. " + team2 + "' wurde erfolgreich hinzugefügt!",
                        "Erfolg", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Felder wieder leeren und Tabelle neu laden
                    txtTeam1.Clear();
                    txtTeam2.Clear();
                    LadeSpiele();
                }
            }
            catch (Exception ex)
            {
                // Wenn was schief geht (z.B. XAMPP aus), stürzt das Programm nicht ab, sondern zeigt den Fehler an.
                MessageBox.Show("Fehler beim Hinzufügen des Spiels:\n" + ex.Message,
                    "Datenbankfehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Wird ausgeführt, wenn man ein Spiel anklickt und auf "Löschen" drückt.
        /// </summary>
        private void BtnLoeschen_Klick(object sender, EventArgs e)
        {
            // Prüfen, ob überhaupt ein Spiel angeklickt/ausgewählt wurde
            if (dgvSpiele.SelectedRows.Count == 0)
            {
                MessageBox.Show("Bitte wählen Sie ein Spiel aus der Liste aus.",
                    "Keine Auswahl", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Wir holen uns die ID des angeklickten Spiels
            int spielId = Convert.ToInt32(dgvSpiele.SelectedRows[0].Cells["id"].Value);
            string team1Name = dgvSpiele.SelectedRows[0].Cells["Team 1"].Value.ToString();
            string team2Name = dgvSpiele.SelectedRows[0].Cells["Team 2"].Value.ToString();
            string spielName = team1Name + " vs. " + team2Name;

            // Fragen, ob der Nutzer sich wirklich sicher ist
            DialogResult result = MessageBox.Show(
                "Soll das Spiel '" + spielName + "' wirklich gelöscht werden?\nAlle zugehörigen Tipps werden ebenfalls gelöscht!",
                "Löschen bestätigen", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result != DialogResult.Yes)
            {
                return; // Wenn er "Nein" drückt, abbrechen.
            }

            try
            {
                using (MySqlConnection conn = DatenbankVerbindung.VerbindungAbrufen())
                {
                    conn.Open();

                    // Logge den Befehl für unser Terminal
                    SQLProtokollierer.Protokollieren("DELETE FROM spiele WHERE id = " + spielId);

                    using (MySqlCommand cmd = new MySqlCommand("DELETE FROM spiele WHERE id = @id", conn))
                    {
                        cmd.Parameters.AddWithValue("@id", spielId);
                        cmd.ExecuteNonQuery(); // Löscht das Spiel
                    }

                    MessageBox.Show("Spiel wurde erfolgreich gelöscht.", "Gelöscht",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);

                    LadeSpiele(); // Tabelle aktualisieren
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fehler beim Löschen:\n" + ex.Message,
                    "Datenbankfehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Holt alle Spiele aus der Datenbank und zeigt sie in der Tabelle (DataGridView) an.
        /// </summary>
        private void LadeSpiele()
        {
            try
            {
                using (MySqlConnection conn = DatenbankVerbindung.VerbindungAbrufen())
                {
                    conn.Open();

                    // Der SELECT-Befehl: Wir holen uns id, team1, team2 und das datum aus der Tabelle
                    string sql = @"SELECT id, team1 AS 'Team 1', team2 AS 'Team 2',
                               DATE_FORMAT(datum, '%d.%m.%Y %H:%i') AS 'Datum',
                               IFNULL(CONCAT(ergebnis_team1, ':', ergebnis_team2), '—') AS 'Ergebnis'
                               FROM spiele ORDER BY datum ASC";

                    SQLProtokollierer.Protokollieren(sql); // Logs!

                    // Ein Adapter "übersetzt" das Ergebnis der Datenbank in eine Form, die C# versteht
                    using (MySqlDataAdapter adapter = new MySqlDataAdapter(sql, conn))
                    {
                        System.Data.DataTable table = new System.Data.DataTable();
                        adapter.Fill(table); // Füllt unsere C# Tabelle mit den Daten

                        dgvSpiele.DataSource = table; // Zeigt die Daten in unserem Fenster an
                    }

                    // Die ID brauchen wir intern (fürs Löschen), aber wir wollen sie dem Nutzer nicht zeigen
                    if (dgvSpiele.Columns.Contains("id"))
                    {
                        dgvSpiele.Columns["id"].Visible = false;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fehler beim Laden der Spiele:\n" + ex.Message,
                    "Datenbankfehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // =========================================================
        // Ab hier sind nur noch kleine Hilfsmethoden für das Design
        // =========================================================
        private static Label LabelStylen(string text)
        {
            Label lbl = new Label();
            lbl.Text = text;
            lbl.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            lbl.ForeColor = Color.FromArgb(180, 180, 210);
            lbl.Dock = DockStyle.Fill;
            lbl.TextAlign = ContentAlignment.MiddleLeft;
            return lbl;
        }

        private static TextBox TextFeldStylen()
        {
            TextBox txt = new TextBox();
            txt.Dock = DockStyle.Fill;
            txt.Font = new Font("Segoe UI", 11);
            txt.BackColor = Color.FromArgb(50, 50, 70);
            txt.ForeColor = Color.White;
            txt.BorderStyle = BorderStyle.FixedSingle;
            txt.Margin = new Padding(0, 4, 8, 4);
            return txt;
        }

        private static Button AktionsButtonErstellen(string text, Color backColor)
        {
            Button btn = new Button();
            btn.Text = text;
            btn.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            btn.BackColor = backColor;
            btn.ForeColor = Color.White;
            btn.FlatStyle = FlatStyle.Flat;
            btn.Cursor = Cursors.Hand;
            btn.Height = 40;
            btn.AutoSize = false;
            btn.Width = 260;
            btn.Margin = new Padding(0, 0, 10, 0);
            btn.FlatAppearance.BorderSize = 0;
            return btn;
        }

        private static void TabelleStylen(DataGridView dgv)
        {
            dgv.DefaultCellStyle.BackColor = Color.FromArgb(28, 28, 45);
            dgv.DefaultCellStyle.ForeColor = Color.White;
            dgv.DefaultCellStyle.SelectionBackColor = Color.FromArgb(0, 100, 180);
            dgv.DefaultCellStyle.SelectionForeColor = Color.White;
            dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(40, 40, 65);
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(255, 210, 0);
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            dgv.EnableHeadersVisualStyles = false;
            dgv.ColumnHeadersHeight = 42;
            dgv.RowTemplate.Height = 36;
        }
    }
}
