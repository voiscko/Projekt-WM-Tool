using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using WMTippTool.Database;

namespace WMTippTool.Forms
{
    /// <summary>
    /// In diesem Fenster (Form) können Nutzer ihre Tipps für ein Spiel abgeben.
    /// Es zeigt auch eine Liste aller bisher abgegebenen Tipps an.
    /// </summary>
    public class TippForm : Form
    {
        // === Unsere Bausteine für dieses Fenster ===
        private ComboBox cboSpiele = null!; // Dropdown-Menü für die Spieleauswahl
        private TextBox txtBenutzername = null!; // Textfeld für den eigenen Namen
        private NumericUpDown nudTippTeam1 = null!; // Zahlenfeld für Tore von Team 1
        private NumericUpDown nudTippTeam2 = null!; // Zahlenfeld für Tore von Team 2
        private Button btnTippSpeichern = null!; // Button zum Speichern
        private DataGridView dgvTipps = null!; // Tabelle, in der alle Tipps stehen
        private Label lblSpielLabel = null!; // Kleiner Hinweis-Text unter dem Dropdown

        // Eine Liste, um uns im Hintergrund die echten IDs der Spiele zu merken.
        // Das Dropdown zeigt ja nur Text ("Deutschland vs. Spanien") an, 
        // wir brauchen für die Datenbank aber die ID (z.B. "5").
        private List<int> spielIds = new();

        public TippForm()
        {
            InitializeComponent(); // Baut das Fenster auf
            LadeOffeneSpiele(); // Füllt das Dropdown mit Spielen, die noch kein Ergebnis haben
            LadeTipps(); // Lädt alle bisherigen Tipps in die Tabelle
        }

        /// <summary>
        /// Hier wird das Design des Fensters gebaut.
        /// </summary>
        private void InitializeComponent()
        {
            // Fenstereinstellungen
            this.Text = "✏️ Tipp abgeben";
            this.MinimumSize = new Size(800, 620);
            this.Size = new Size(1100, 820);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.Sizable;
            this.BackColor = Color.FromArgb(22, 22, 35);
            this.AutoScaleMode = AutoScaleMode.Dpi;

            // ===== Haupt-Layout =====
            var rootLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 5,
                BackColor = Color.FromArgb(22, 22, 35),
                Padding = new Padding(14, 10, 14, 10)
            };
            // 5 Zeilen: Titel | Eingabekasten | "Alle Tipps" Text | Tabelle | Buttons unten
            rootLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 52f));
            rootLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 215f));
            rootLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 36f));
            rootLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));
            rootLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 55f));
            rootLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));

            // ----- Titel -----
            var lblTitel = new Label
            {
                Text = "✏️ Tipp abgeben",
                Font = new Font("Segoe UI", 20, FontStyle.Bold),
                ForeColor = Color.FromArgb(255, 210, 0),
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft
            };

            // ----- Eingabekasten -----
            var pnlInput = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(35, 35, 55),
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(14, 8, 14, 8)
            };

            var inputLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2, // Links das Label (Beschreibung), Rechts das Eingabefeld
                RowCount = 4,
                BackColor = Color.FromArgb(35, 35, 55)
            };
            inputLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 190f)); 
            inputLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));  
            inputLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 25f));  
            inputLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 26f)); 
            inputLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 25f));  
            inputLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 50f));  

            // Zeile 0: Spiel-Auswahl (Dropdown)
            var lblSpiel = StyleLabel("Spiel auswählen:");
            cboSpiele = new ComboBox
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 11),
                BackColor = Color.FromArgb(50, 50, 70),
                ForeColor = Color.White,
                DropDownStyle = ComboBoxStyle.DropDownList,
                FlatStyle = FlatStyle.Flat,
                Margin = new Padding(0, 4, 0, 2)
            };
            // Wenn man ein anderes Spiel auswählt, passiert etwas!
            cboSpiele.SelectedIndexChanged += CboSpiele_SelectedIndexChanged;
            inputLayout.Controls.Add(lblSpiel, 0, 0);
            inputLayout.Controls.Add(cboSpiele, 1, 0);

            // Zeile 1: Status-Label ("Kein Spiel ausgewählt")
            lblSpielLabel = new Label
            {
                Text = "Kein Spiel ausgewählt",
                Font = new Font("Segoe UI", 10, FontStyle.Italic),
                ForeColor = Color.Gray,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(190, 0, 0, 0)
            };
            inputLayout.Controls.Add(lblSpielLabel, 0, 1);
            inputLayout.SetColumnSpan(lblSpielLabel, 2);

            // Zeile 2: Benutzername Eingabe
            var lblBenutzer = StyleLabel("Ihr Name:");
            txtBenutzername = new TextBox
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 11),
                BackColor = Color.FromArgb(50, 50, 70),
                ForeColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Margin = new Padding(0, 4, 0, 4)
            };
            inputLayout.Controls.Add(lblBenutzer, 0, 2);
            inputLayout.Controls.Add(txtBenutzername, 1, 2);

            // Zeile 3: Tore tippen und Button
            var lblTipp = StyleLabel("Ihr Tipp:");

            // Ein kleiner extra Container (FlowLayoutPanel) nur für die Tore-Zahlen
            var tippFlow = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight,
                BackColor = Color.FromArgb(35, 35, 55),
                WrapContents = false,
                Padding = new Padding(0, 4, 0, 4)
            };

            var lblTeam1 = new Label
            {
                Text = "Team 1:",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(180, 180, 210),
                AutoSize = true,
                Margin = new Padding(0, 6, 6, 0)
            };

            nudTippTeam1 = CreateNumericUpDown(); // Zahlenfeld von 0 bis 30

            var lblDoppelpunkt = new Label
            {
                Text = ":",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = true,
                Margin = new Padding(6, 2, 6, 0)
            };

            var lblTeam2 = new Label
            {
                Text = "Team 2:",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(180, 180, 210),
                AutoSize = true,
                Margin = new Padding(0, 6, 6, 0)
            };

            nudTippTeam2 = CreateNumericUpDown();

            btnTippSpeichern = new Button
            {
                Text = "✅ Tipp speichern",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                BackColor = Color.FromArgb(16, 137, 62), // Grün
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                Height = 38,
                Width = 220,
                Margin = new Padding(20, 2, 0, 0)
            };
            btnTippSpeichern.FlatAppearance.BorderSize = 0;
            btnTippSpeichern.Click += BtnTippSpeichern_Click;

            tippFlow.Controls.AddRange(new Control[]
            {
                lblTeam1, nudTippTeam1, lblDoppelpunkt, lblTeam2, nudTippTeam2, btnTippSpeichern
            });

            inputLayout.Controls.Add(lblTipp, 0, 3);
            inputLayout.Controls.Add(tippFlow, 1, 3);

            pnlInput.Controls.Add(inputLayout);

            // ----- Label über der Tabelle -----
            var lblAllleTipps = new Label
            {
                Text = "📋 Alle abgegebenen Tipps:",
                Font = new Font("Segoe UI", 13, FontStyle.Bold),
                ForeColor = Color.FromArgb(180, 180, 220),
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.BottomLeft,
                Padding = new Padding(0, 0, 0, 2)
            };

            // ----- Tabelle (DataGridView) -----
            dgvTipps = new DataGridView
            {
                Dock = DockStyle.Fill,
                BackgroundColor = Color.FromArgb(28, 28, 45),
                BorderStyle = BorderStyle.None,
                RowHeadersVisible = false,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                Font = new Font("Segoe UI", 10),
                GridColor = Color.FromArgb(60, 60, 80),
                Margin = new Padding(0, 4, 0, 4)
            };
            StyleDataGridView(dgvTipps);

            // ----- Buttons ganz unten -----
            var btnPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight,
                BackColor = Color.FromArgb(22, 22, 35),
                WrapContents = false,
                Padding = new Padding(0, 6, 0, 0)
            };

            var btnAktualisieren = new Button
            {
                Text = "🔄 Tipps aktualisieren",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                BackColor = Color.FromArgb(60, 60, 90),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                Height = 40,
                Width = 240,
                Margin = new Padding(0, 0, 10, 0)
            };
            btnAktualisieren.FlatAppearance.BorderSize = 0;
            btnAktualisieren.Click += (s, e) => LadeTipps();

            var btnZurueck = new Button
            {
                Text = "← Zurück zum Menü",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                BackColor = Color.FromArgb(50, 50, 75),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                Height = 40,
                Width = 220,
                Margin = new Padding(0, 0, 0, 0)
            };
            btnZurueck.FlatAppearance.BorderSize = 0;
            btnZurueck.Click += (s, e) => this.Close();

            btnPanel.Controls.AddRange(new Control[] { btnAktualisieren, btnZurueck });

            // Alles zusammensetzen
            rootLayout.Controls.Add(lblTitel, 0, 0);
            rootLayout.Controls.Add(pnlInput, 0, 1);
            rootLayout.Controls.Add(lblAllleTipps, 0, 2);
            rootLayout.Controls.Add(dgvTipps, 0, 3);
            rootLayout.Controls.Add(btnPanel, 0, 4);

            this.Controls.Add(rootLayout);
        }

        /// <summary>
        /// Wenn man im Dropdown ein Spiel auswählt, ändert sich der Text darunter.
        /// </summary>
        private void CboSpiele_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (cboSpiele.SelectedIndex >= 0)
                lblSpielLabel.Text = "✓ Spiel ausgewählt";
        }

        /// <summary>
        /// Wird ausgeführt, wenn man auf "Tipp speichern" drückt.
        /// Hier wird der Tipp in die MySQL Datenbank eingetragen.
        /// </summary>
        private void BtnTippSpeichern_Click(object? sender, EventArgs e)
        {
            // Hat der Nutzer überhaupt ein Spiel im Dropdown ausgewählt?
            if (cboSpiele.SelectedIndex < 0)
            {
                MessageBox.Show("Bitte wählen Sie ein Spiel aus!", "Kein Spiel ausgewählt",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string name = txtBenutzername.Text.Trim();
            if (string.IsNullOrEmpty(name))
            {
                MessageBox.Show("Bitte geben Sie Ihren Namen ein!", "Name fehlt",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Wir holen uns die echte Spiel-ID aus unserer geheimen Liste im Hintergrund
            int spielId = spielIds[cboSpiele.SelectedIndex];
            int t1 = (int)nudTippTeam1.Value;
            int t2 = (int)nudTippTeam2.Value;

            try
            {
                using var conn = DBConnection.GetConnection();
                conn.Open();

                // 1. Zuerst prüfen wir, ob dieser Name für dieses Spiel schon getippt hat.
                // Wir zählen einfach (COUNT), wie viele Tipps es von ihm für das Spiel gibt.
                string checkSql = "SELECT COUNT(*) FROM tipps WHERE spiel_id = @sid AND benutzername = @name";
                
                // Wir loggen den Befehl (mit Werten eingefügt, damit man es im Terminal lesen kann)
                QueryLogger.Log($"SELECT COUNT(*) FROM tipps WHERE spiel_id = {spielId} AND benutzername = '{name}'");

                using var checkCmd = new MySqlCommand(checkSql, conn);
                checkCmd.Parameters.AddWithValue("@sid", spielId);
                checkCmd.Parameters.AddWithValue("@name", name);
                long count = (long)checkCmd.ExecuteScalar()!; // Führt den Befehl aus und gibt die Zahl zurück

                // Wenn er schon getippt hat (count > 0), brechen wir ab!
                if (count > 0)
                {
                    MessageBox.Show(
                        $"'{name}' hat für dieses Spiel bereits einen Tipp abgegeben!\nJeder Benutzer kann pro Spiel nur einmal tippen.",
                        "Tipp bereits vorhanden", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // 2. Wenn wir hier sind, darf er tippen. Wir fügen den Tipp ein (INSERT).
                string sql = "INSERT INTO tipps (spiel_id, benutzername, tipp_team1, tipp_team2) VALUES (@sid, @name, @t1, @t2)";
                
                QueryLogger.Log($"INSERT INTO tipps (spiel_id, benutzername, tipp_team1, tipp_team2) VALUES ({spielId}, '{name}', {t1}, {t2})");

                using var cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@sid", spielId);
                cmd.Parameters.AddWithValue("@name", name);
                cmd.Parameters.AddWithValue("@t1", t1);
                cmd.Parameters.AddWithValue("@t2", t2);
                cmd.ExecuteNonQuery(); // Tipp wird in MySQL gespeichert!

                MessageBox.Show(
                    $"Tipp von '{name}' ({t1}:{t2}) wurde erfolgreich gespeichert!",
                    "Tipp gespeichert", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Felder wieder aufräumen
                txtBenutzername.Clear();
                nudTippTeam1.Value = 0;
                nudTippTeam2.Value = 0;
                
                // Tabelle aktualisieren, damit der neue Tipp direkt sichtbar wird
                LadeTipps();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fehler beim Speichern des Tipps:\n{ex.Message}",
                    "Datenbankfehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Sucht alle Spiele aus der Datenbank, bei denen "ergebnis_team1" noch leer (NULL) ist.
        /// Das sind Spiele, die noch nicht vorbei sind.
        /// </summary>
        private void LadeOffeneSpiele()
        {
            spielIds.Clear();
            cboSpiele.Items.Clear();

            try
            {
                using var conn = DBConnection.GetConnection();
                conn.Open();
                
                string sql = "SELECT id, team1, team2, datum FROM spiele WHERE ergebnis_team1 IS NULL ORDER BY datum ASC";
                QueryLogger.Log(sql); // Logging

                using var cmd = new MySqlCommand(sql, conn);
                using var reader = cmd.ExecuteReader(); // Reader liest Zeile für Zeile aus MySQL

                while (reader.Read()) // Solange es noch Zeilen gibt...
                {
                    // ID geheim in unserer Liste speichern
                    spielIds.Add(reader.GetInt32("id")); 
                    
                    // Schönen Text für das Dropdown zusammenbauen
                    string datum = reader.GetDateTime("datum").ToString("dd.MM.yyyy HH:mm");
                    cboSpiele.Items.Add($"{reader.GetString("team1")} vs. {reader.GetString("team2")} ({datum})");
                }

                // Wenn es gar keine Spiele gibt (z.B. frisch installiert)
                if (cboSpiele.Items.Count == 0)
                {
                    cboSpiele.Items.Add("Keine offenen Spiele vorhanden");
                    cboSpiele.Enabled = false; // Dropdown sperren
                    btnTippSpeichern.Enabled = false; // Button sperren
                    lblSpielLabel.Text = "Aktuell keine Spiele zum Tippen verfügbar.";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fehler beim Laden der Spiele:\n{ex.Message}",
                    "Datenbankfehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Holt alle Tipps aus der Datenbank und zeigt sie in der großen Tabelle an.
        /// Um den Namen des Spiels anzeigen zu können, müssen wir die Tabelle 'tipps'
        /// mit der Tabelle 'spiele' verknüpfen (JOIN).
        /// </summary>
        private void LadeTipps()
        {
            try
            {
                using var conn = DBConnection.GetConnection();
                conn.Open();
                
                // JOIN: Verbindet die Tabelle "tipps" (t) mit "spiele" (s) anhand der spiel_id.
                string sql = @"SELECT t.id, CONCAT(s.team1, ' vs. ', s.team2) AS 'Spiel',
                               t.benutzername AS 'Name', 
                               CONCAT(t.tipp_team1, ':', t.tipp_team2) AS 'Tipp',
                               t.punkte AS 'Punkte'
                               FROM tipps t
                               JOIN spiele s ON t.spiel_id = s.id
                               ORDER BY s.datum ASC, t.benutzername ASC";
                
                QueryLogger.Log(sql); // Logging

                using var adapter = new MySqlDataAdapter(sql, conn);
                var table = new System.Data.DataTable();
                adapter.Fill(table);
                dgvTipps.DataSource = table;

                // ID verstecken
                if (dgvTipps.Columns.Contains("id"))
                    dgvTipps.Columns["id"].Visible = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fehler beim Laden der Tipps:\n{ex.Message}",
                    "Datenbankfehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // =========================================================
        // Hilfsmethoden fürs Design (Farben, Schriftart etc.)
        // =========================================================
        private static Label StyleLabel(string text) => new Label
        {
            Text = text,
            Font = new Font("Segoe UI", 10, FontStyle.Bold),
            ForeColor = Color.FromArgb(180, 180, 210),
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleLeft
        };

        private static NumericUpDown CreateNumericUpDown() => new NumericUpDown
        {
            Minimum = 0,
            Maximum = 30, // Man kann maximal 30 Tore tippen :)
            Value = 0,
            Font = new Font("Segoe UI", 13, FontStyle.Bold),
            BackColor = Color.FromArgb(50, 50, 70),
            ForeColor = Color.White,
            Width = 70,
            Height = 36,
            Margin = new Padding(0, 2, 8, 0)
        };

        private static void StyleDataGridView(DataGridView dgv)
        {
            dgv.DefaultCellStyle.BackColor = Color.FromArgb(28, 28, 45);
            dgv.DefaultCellStyle.ForeColor = Color.White;
            dgv.DefaultCellStyle.SelectionBackColor = Color.FromArgb(16, 120, 62);
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
