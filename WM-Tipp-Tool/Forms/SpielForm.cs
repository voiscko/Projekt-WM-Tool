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
        private DataGridView dgvSpiele = null!;
        // Zwei Eingabefelder für die Teamnamen
        private TextBox txtTeam1 = null!;
        private TextBox txtTeam2 = null!;
        // Ein Feld für das Datum und die Uhrzeit
        private DateTimePicker dtpDatum = null!;
        // Buttons für unsere Aktionen
        private Button btnHinzufuegen = null!;
        private Button btnLoeschen = null!;
        private Button btnAktualisieren = null!;

        // Der Konstruktor: Wird ausgeführt, wenn das Fenster geöffnet wird.
        public SpielForm()
        {
            InitializeComponent(); // Baut das Fenster auf (Buttons, Farben etc.)
            LadeSpiele(); // Holt sofort die Spiele aus der Datenbank und zeigt sie an
        }

        /// <summary>
        /// Hier wird das Aussehen des Fensters "zusammengebaut".
        /// </summary>
        private void InitializeComponent()
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
            var rootLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 4,
                BackColor = Color.FromArgb(22, 22, 35),
                Padding = new Padding(14, 10, 14, 10)
            };
            // Wir haben 4 Zeilen: Titel | Eingabefelder | Die Tabelle mit den Spielen | Buttons unten
            rootLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 52f));   // Titel
            rootLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 165f));  // Eingabe-Panel
            rootLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));   // Die Tabelle
            rootLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 55f));   // Buttons
            rootLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));

            // ----- 1. Titel oben -----
            var lblTitel = new Label
            {
                Text = "🏟️ Spiele verwalten",
                Font = new Font("Segoe UI", 20, FontStyle.Bold),
                ForeColor = Color.FromArgb(255, 210, 0),
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft
            };

            // ----- 2. Eingabebereich -----
            // Ein Kasten (Panel) für die Eingaben (Team 1, Team 2, Datum)
            var pnlInput = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(35, 35, 55),
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(14, 10, 14, 10)
            };
            
            // Ein kleines Layout im Eingabekasten, um alles ordentlich anzuordnen
            var inputLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 4, // 4 Spalten (z.B. Label "Team 1" | Textbox | Label "Team 2" | Textbox)
                RowCount = 3,
                BackColor = Color.FromArgb(35, 35, 55)
            };
            inputLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 75f));   
            inputLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50f));     
            inputLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 75f));   
            inputLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50f));     
            inputLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 33.3f));         
            inputLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 33.3f));         
            inputLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 33.4f));         

            // Eingabefelder erstellen
            var lblTeam1 = StyleLabel("Team 1:");
            txtTeam1 = StyleTextBox();
            var lblTeam2 = StyleLabel("Team 2:");
            txtTeam2 = StyleTextBox();

            inputLayout.Controls.Add(lblTeam1, 0, 0);
            inputLayout.Controls.Add(txtTeam1, 1, 0);
            inputLayout.Controls.Add(lblTeam2, 2, 0);
            inputLayout.Controls.Add(txtTeam2, 3, 0);

            // Datumseingabe
            var lblDatum = StyleLabel("Datum:");
            dtpDatum = new DateTimePicker
            {
                Dock = DockStyle.Fill,
                Format = DateTimePickerFormat.Custom,
                CustomFormat = "dd.MM.yyyy HH:mm", // Format: Tag.Monat.Jahr Stunde:Minute
                ShowUpDown = false,
                CalendarForeColor = Color.White,
                CalendarTitleBackColor = Color.FromArgb(0, 120, 215),
                BackColor = Color.FromArgb(50, 50, 70),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 11),
                Margin = new Padding(0, 4, 0, 4)
            };
            var dtpPanel = new Panel { Dock = DockStyle.Fill };
            dtpPanel.Controls.Add(dtpDatum);
            dtpDatum.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom;
            dtpDatum.Location = new Point(0, 4);
            dtpDatum.Width = dtpPanel.Width;
            dtpPanel.SizeChanged += (s, e) => dtpDatum.Width = dtpPanel.Width;

            inputLayout.Controls.Add(lblDatum, 0, 1);
            inputLayout.Controls.Add(dtpPanel, 1, 1);
            
            var lblDatumHint = new Label
            {
                Text = "Format: TT.MM.JJJJ HH:MM",
                Font = new Font("Segoe UI", 9, FontStyle.Italic),
                ForeColor = Color.FromArgb(120, 120, 150),
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(8, 0, 0, 0)
            };
            inputLayout.SetColumnSpan(dtpPanel, 1);
            inputLayout.Controls.Add(lblDatumHint, 2, 1);
            inputLayout.SetColumnSpan(lblDatumHint, 2);

            // "Hinzufügen" Button
            btnHinzufuegen = new Button
            {
                Text = "➕ Spiel hinzufügen",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                BackColor = Color.FromArgb(0, 120, 215),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Bottom,
                Margin = new Padding(0, 4, 0, 4)
            };
            btnHinzufuegen.FlatAppearance.BorderSize = 0;
            // Wenn der Button geklickt wird, rufe "BtnHinzufuegen_Click" auf!
            btnHinzufuegen.Click += BtnHinzufuegen_Click;

            inputLayout.Controls.Add(btnHinzufuegen, 0, 2);
            inputLayout.SetColumnSpan(btnHinzufuegen, 2);

            pnlInput.Controls.Add(inputLayout);

            // ----- 3. Tabelle (DataGridView) -----
            dgvSpiele = new DataGridView
            {
                Dock = DockStyle.Fill,
                BackgroundColor = Color.FromArgb(28, 28, 45),
                BorderStyle = BorderStyle.None,
                RowHeadersVisible = false,
                AllowUserToAddRows = false, // Nicht direkt in der Tabelle schreiben
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                Font = new Font("Segoe UI", 10),
                GridColor = Color.FromArgb(60, 60, 80),
                Margin = new Padding(0, 8, 0, 8)
            };
            StyleDataGridView(dgvSpiele);

            // ----- 4. Buttons unten (Löschen, Aktualisieren, Zurück) -----
            var btnPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight,
                BackColor = Color.FromArgb(22, 22, 35),
                WrapContents = false,
                Padding = new Padding(0, 6, 0, 0)
            };

            btnLoeschen = CreateActionButton("🗑️ Ausgewähltes Spiel löschen", Color.FromArgb(180, 40, 40));
            btnLoeschen.Click += BtnLoeschen_Click;

            btnAktualisieren = CreateActionButton("🔄 Aktualisieren", Color.FromArgb(60, 60, 90));
            btnAktualisieren.Click += (s, e) => LadeSpiele();

            var btnZurueck = CreateActionButton("← Zurück zum Menü", Color.FromArgb(50, 50, 75));
            btnZurueck.Click += (s, e) => this.Close(); // Schließt dieses Fenster

            btnPanel.Controls.AddRange(new Control[] { btnLoeschen, btnAktualisieren, btnZurueck });

            // Alles in unser RootLayout stecken
            rootLayout.Controls.Add(lblTitel, 0, 0);
            rootLayout.Controls.Add(pnlInput, 0, 1);
            rootLayout.Controls.Add(dgvSpiele, 0, 2);
            rootLayout.Controls.Add(btnPanel, 0, 3);

            this.Controls.Add(rootLayout);
        }

        /// <summary>
        /// Wird ausgeführt, wenn man auf "Spiel hinzufügen" drückt.
        /// Nimmt die Eingaben und speichert sie in der Datenbank.
        /// </summary>
        private void BtnHinzufuegen_Click(object? sender, EventArgs e)
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
                using var conn = DBConnection.GetConnection();
                conn.Open();
                
                // Der SQL-Befehl (INSERT). Die @-Werte sind Platzhalter, um uns vor Hacks zu schützen!
                string sql = "INSERT INTO spiele (team1, team2, datum) VALUES (@t1, @t2, @datum)";
                
                // Wir schicken den SQL-Befehl ins Log, damit man das im Terminal sehen kann!
                // Wir ersetzen die Platzhalter manuell für's Log, damit es dort gut aussieht:
                string logSql = $"INSERT INTO spiele (team1, team2, datum) VALUES ('{team1}', '{team2}', '{dtpDatum.Value:yyyy-MM-dd HH:mm:ss}')";
                QueryLogger.Log(logSql);

                // Den echten Befehl an die Datenbank schicken
                using var cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@t1", team1);
                cmd.Parameters.AddWithValue("@t2", team2);
                cmd.Parameters.AddWithValue("@datum", dtpDatum.Value);
                cmd.ExecuteNonQuery(); // "Los, mach es!"

                // Erfolg anzeigen
                MessageBox.Show($"Spiel '{team1} vs. {team2}' wurde erfolgreich hinzugefügt!",
                    "Erfolg", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Felder wieder leeren und Tabelle neu laden
                txtTeam1.Clear();
                txtTeam2.Clear();
                LadeSpiele();
            }
            catch (Exception ex)
            {
                // Wenn was schief geht (z.B. XAMPP aus), stürzt das Programm nicht ab, sondern zeigt den Fehler an.
                MessageBox.Show($"Fehler beim Hinzufügen des Spiels:\n{ex.Message}",
                    "Datenbankfehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Wird ausgeführt, wenn man ein Spiel anklickt und auf "Löschen" drückt.
        /// </summary>
        private void BtnLoeschen_Click(object? sender, EventArgs e)
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
            string spielName = $"{dgvSpiele.SelectedRows[0].Cells["Team 1"].Value} vs. {dgvSpiele.SelectedRows[0].Cells["Team 2"].Value}";

            // Fragen, ob der Nutzer sich wirklich sicher ist
            var result = MessageBox.Show(
                $"Soll das Spiel '{spielName}' wirklich gelöscht werden?\nAlle zugehörigen Tipps werden ebenfalls gelöscht!",
                "Löschen bestätigen", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result != DialogResult.Yes) return; // Wenn er "Nein" drückt, abbrechen.

            try
            {
                using var conn = DBConnection.GetConnection();
                conn.Open();
                
                // Logge den Befehl für unser Terminal
                QueryLogger.Log($"DELETE FROM spiele WHERE id = {spielId}");

                using var cmd = new MySqlCommand("DELETE FROM spiele WHERE id = @id", conn);
                cmd.Parameters.AddWithValue("@id", spielId);
                cmd.ExecuteNonQuery(); // Löscht das Spiel
                
                MessageBox.Show("Spiel wurde erfolgreich gelöscht.", "Gelöscht",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                
                LadeSpiele(); // Tabelle aktualisieren
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fehler beim Löschen:\n{ex.Message}",
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
                using var conn = DBConnection.GetConnection();
                conn.Open();
                
                // Der SELECT-Befehl: Wir holen uns id, team1, team2 und das datum aus der Tabelle
                string sql = @"SELECT id, team1 AS 'Team 1', team2 AS 'Team 2', 
                               DATE_FORMAT(datum, '%d.%m.%Y %H:%i') AS 'Datum',
                               IFNULL(CONCAT(ergebnis_team1, ':', ergebnis_team2), '—') AS 'Ergebnis'
                               FROM spiele ORDER BY datum ASC";
                
                QueryLogger.Log(sql); // Logs!
                
                // Ein Adapter "übersetzt" das Ergebnis der Datenbank in eine Form, die C# versteht
                using var adapter = new MySqlDataAdapter(sql, conn);
                var table = new System.Data.DataTable();
                adapter.Fill(table); // Füllt unsere C# Tabelle mit den Daten
                
                dgvSpiele.DataSource = table; // Zeigt die Daten in unserem Fenster an

                // Die ID brauchen wir intern (fürs Löschen), aber wir wollen sie dem Nutzer nicht zeigen (sieht hässlich aus)
                if (dgvSpiele.Columns.Contains("id"))
                    dgvSpiele.Columns["id"].Visible = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fehler beim Laden der Spiele:\n{ex.Message}",
                    "Datenbankfehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // =========================================================
        // Ab hier sind nur noch kleine Hilfsmethoden für das Design
        // =========================================================
        private static Label StyleLabel(string text) => new Label
        {
            Text = text,
            Font = new Font("Segoe UI", 10, FontStyle.Bold),
            ForeColor = Color.FromArgb(180, 180, 210),
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleLeft
        };

        private static TextBox StyleTextBox() => new TextBox
        {
            Dock = DockStyle.Fill,
            Font = new Font("Segoe UI", 11),
            BackColor = Color.FromArgb(50, 50, 70),
            ForeColor = Color.White,
            BorderStyle = BorderStyle.FixedSingle,
            Margin = new Padding(0, 4, 8, 4)
        };

        private static Button CreateActionButton(string text, Color backColor)
        {
            var btn = new Button
            {
                Text = text,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                BackColor = backColor,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                Height = 40,
                AutoSize = false,
                Width = 260,
                Margin = new Padding(0, 0, 10, 0)
            };
            btn.FlatAppearance.BorderSize = 0;
            return btn;
        }

        private static void StyleDataGridView(DataGridView dgv)
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
