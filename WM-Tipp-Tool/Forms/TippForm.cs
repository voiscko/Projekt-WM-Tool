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
        private ComboBox cboSpiele;       // Dropdown-Menü für die Spieleauswahl
        private TextBox txtBenutzername;  // Textfeld für den eigenen Namen
        private NumericUpDown nudTippTeam1; // Zahlenfeld für Tore von Team 1
        private NumericUpDown nudTippTeam2; // Zahlenfeld für Tore von Team 2
        private Button btnTippSpeichern;  // Button zum Speichern
        private DataGridView dgvTipps;    // Tabelle, in der alle Tipps stehen
        private Label lblSpielHinweis;      // Kleiner Hinweis-Text unter dem Dropdown

        // Eine Liste, um uns im Hintergrund die echten IDs der Spiele zu merken.
        // Das Dropdown zeigt ja nur Text ("Deutschland vs. Spanien") an,
        // wir brauchen für die Datenbank aber die ID (z.B. "5").
        private List<int> spielIDs = new List<int>();

        public TippForm()
        {
            KomponentenInitialisieren(); // Baut das Fenster auf
            LadeOffeneSpiele();    // Füllt das Dropdown mit Spielen, die noch kein Ergebnis haben
            LadeTipps();           // Lädt alle bisherigen Tipps in die Tabelle
        }

        /// <summary>
        /// Hier wird das Design des Fensters gebaut.
        /// </summary>
        private void KomponentenInitialisieren()
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
            TableLayoutPanel hauptLayout = new TableLayoutPanel();
            hauptLayout.Dock = DockStyle.Fill;
            hauptLayout.ColumnCount = 1;
            hauptLayout.RowCount = 5;
            hauptLayout.BackColor = Color.FromArgb(22, 22, 35);
            hauptLayout.Padding = new Padding(14, 10, 14, 10);

            // 5 Zeilen: Titel | Eingabekasten | "Alle Tipps" Text | Tabelle | Buttons unten
            hauptLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 52f));
            hauptLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 215f));
            hauptLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 36f));
            hauptLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));
            hauptLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 55f));
            hauptLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));

            // ----- Titel -----
            Label lblTitel = new Label();
            lblTitel.Text = "✏️ Tipp abgeben";
            lblTitel.Font = new Font("Segoe UI", 20, FontStyle.Bold);
            lblTitel.ForeColor = Color.FromArgb(255, 210, 0);
            lblTitel.Dock = DockStyle.Fill;
            lblTitel.TextAlign = ContentAlignment.MiddleLeft;

            // ----- Eingabekasten -----
            Panel pnlEingabe = new Panel();
            pnlEingabe.Dock = DockStyle.Fill;
            pnlEingabe.BackColor = Color.FromArgb(35, 35, 55);
            pnlEingabe.BorderStyle = BorderStyle.FixedSingle;
            pnlEingabe.Padding = new Padding(14, 8, 14, 8);

            TableLayoutPanel eingabeLayout = new TableLayoutPanel();
            eingabeLayout.Dock = DockStyle.Fill;
            eingabeLayout.ColumnCount = 2; // Links das Label (Beschreibung), Rechts das Eingabefeld
            eingabeLayout.RowCount = 4;
            eingabeLayout.BackColor = Color.FromArgb(35, 35, 55);
            eingabeLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 190f));
            eingabeLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
            eingabeLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 25f));
            eingabeLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 26f));
            eingabeLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 25f));
            eingabeLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 50f));

            // Zeile 0: Spiel-Auswahl (Dropdown)
            Label lblSpiel = LabelStylen("Spiel auswählen:");
            cboSpiele = new ComboBox();
            cboSpiele.Dock = DockStyle.Fill;
            cboSpiele.Font = new Font("Segoe UI", 11);
            cboSpiele.BackColor = Color.FromArgb(50, 50, 70);
            cboSpiele.ForeColor = Color.White;
            cboSpiele.DropDownStyle = ComboBoxStyle.DropDownList;
            cboSpiele.FlatStyle = FlatStyle.Flat;
            cboSpiele.Margin = new Padding(0, 4, 0, 2);
            // Wenn man ein anderes Spiel auswählt, passiert etwas!
            cboSpiele.SelectedIndexChanged += CboSpiele_AuswahlGeaendert;
            eingabeLayout.Controls.Add(lblSpiel, 0, 0);
            eingabeLayout.Controls.Add(cboSpiele, 1, 0);

            // Zeile 1: Status-Label ("Kein Spiel ausgewählt")
            lblSpielHinweis = new Label();
            lblSpielHinweis.Text = "Kein Spiel ausgewählt";
            lblSpielHinweis.Font = new Font("Segoe UI", 10, FontStyle.Italic);
            lblSpielHinweis.ForeColor = Color.Gray;
            lblSpielHinweis.Dock = DockStyle.Fill;
            lblSpielHinweis.TextAlign = ContentAlignment.MiddleLeft;
            lblSpielHinweis.Padding = new Padding(190, 0, 0, 0);
            eingabeLayout.Controls.Add(lblSpielHinweis, 0, 1);
            eingabeLayout.SetColumnSpan(lblSpielHinweis, 2);

            // Zeile 2: Benutzername Eingabe
            Label lblBenutzer = LabelStylen("Ihr Name:");
            txtBenutzername = new TextBox();
            txtBenutzername.Dock = DockStyle.Fill;
            txtBenutzername.Font = new Font("Segoe UI", 11);
            txtBenutzername.BackColor = Color.FromArgb(50, 50, 70);
            txtBenutzername.ForeColor = Color.White;
            txtBenutzername.BorderStyle = BorderStyle.FixedSingle;
            txtBenutzername.Margin = new Padding(0, 4, 0, 4);
            eingabeLayout.Controls.Add(lblBenutzer, 0, 2);
            eingabeLayout.Controls.Add(txtBenutzername, 1, 2);

            // Zeile 3: Tore tippen und Button
            Label lblTipp = LabelStylen("Ihr Tipp:");

            // Ein kleiner extra Container (FlowLayoutPanel) nur für die Tore-Zahlen
            FlowLayoutPanel tippFlussLayout = new FlowLayoutPanel();
            tippFlussLayout.Dock = DockStyle.Fill;
            tippFlussLayout.FlowDirection = FlowDirection.LeftToRight;
            tippFlussLayout.BackColor = Color.FromArgb(35, 35, 55);
            tippFlussLayout.WrapContents = false;
            tippFlussLayout.Padding = new Padding(0, 4, 0, 4);

            Label lblTeam1 = new Label();
            lblTeam1.Text = "Team 1:";
            lblTeam1.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            lblTeam1.ForeColor = Color.FromArgb(180, 180, 210);
            lblTeam1.AutoSize = true;
            lblTeam1.Margin = new Padding(0, 6, 6, 0);

            nudTippTeam1 = ZahlenFeldErstellen(); // Zahlenfeld von 0 bis 30

            Label lblDoppelpunkt = new Label();
            lblDoppelpunkt.Text = ":";
            lblDoppelpunkt.Font = new Font("Segoe UI", 16, FontStyle.Bold);
            lblDoppelpunkt.ForeColor = Color.White;
            lblDoppelpunkt.AutoSize = true;
            lblDoppelpunkt.Margin = new Padding(6, 2, 6, 0);

            Label lblTeam2 = new Label();
            lblTeam2.Text = "Team 2:";
            lblTeam2.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            lblTeam2.ForeColor = Color.FromArgb(180, 180, 210);
            lblTeam2.AutoSize = true;
            lblTeam2.Margin = new Padding(0, 6, 6, 0);

            nudTippTeam2 = ZahlenFeldErstellen();

            btnTippSpeichern = new Button();
            btnTippSpeichern.Text = "✅ Tipp speichern";
            btnTippSpeichern.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            btnTippSpeichern.BackColor = Color.FromArgb(16, 137, 62); // Grün
            btnTippSpeichern.ForeColor = Color.White;
            btnTippSpeichern.FlatStyle = FlatStyle.Flat;
            btnTippSpeichern.Cursor = Cursors.Hand;
            btnTippSpeichern.Height = 38;
            btnTippSpeichern.Width = 220;
            btnTippSpeichern.Margin = new Padding(20, 2, 0, 0);
            btnTippSpeichern.FlatAppearance.BorderSize = 0;
            btnTippSpeichern.Click += BtnTippSpeichern_Klick;

            tippFlussLayout.Controls.AddRange(new Control[]
            {
                lblTeam1, nudTippTeam1, lblDoppelpunkt, lblTeam2, nudTippTeam2, btnTippSpeichern
            });

            eingabeLayout.Controls.Add(lblTipp, 0, 3);
            eingabeLayout.Controls.Add(tippFlussLayout, 1, 3);

            pnlEingabe.Controls.Add(eingabeLayout);

            // ----- Label über der Tabelle -----
            Label lblAllleTipps = new Label();
            lblAllleTipps.Text = "📋 Alle abgegebenen Tipps:";
            lblAllleTipps.Font = new Font("Segoe UI", 13, FontStyle.Bold);
            lblAllleTipps.ForeColor = Color.FromArgb(180, 180, 220);
            lblAllleTipps.Dock = DockStyle.Fill;
            lblAllleTipps.TextAlign = ContentAlignment.BottomLeft;
            lblAllleTipps.Padding = new Padding(0, 0, 0, 2);

            // ----- Tabelle (DataGridView) -----
            dgvTipps = new DataGridView();
            dgvTipps.Dock = DockStyle.Fill;
            dgvTipps.BackgroundColor = Color.FromArgb(28, 28, 45);
            dgvTipps.BorderStyle = BorderStyle.None;
            dgvTipps.RowHeadersVisible = false;
            dgvTipps.AllowUserToAddRows = false;
            dgvTipps.AllowUserToDeleteRows = false;
            dgvTipps.ReadOnly = true;
            dgvTipps.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvTipps.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvTipps.Font = new Font("Segoe UI", 10);
            dgvTipps.GridColor = Color.FromArgb(60, 60, 80);
            dgvTipps.Margin = new Padding(0, 4, 0, 4);
            TabelleStylen(dgvTipps);

            // ----- Buttons ganz unten -----
            FlowLayoutPanel btnPanel = new FlowLayoutPanel();
            btnPanel.Dock = DockStyle.Fill;
            btnPanel.FlowDirection = FlowDirection.LeftToRight;
            btnPanel.BackColor = Color.FromArgb(22, 22, 35);
            btnPanel.WrapContents = false;
            btnPanel.Padding = new Padding(0, 6, 0, 0);

            Button btnAktualisieren = new Button();
            btnAktualisieren.Text = "🔄 Tipps aktualisieren";
            btnAktualisieren.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            btnAktualisieren.BackColor = Color.FromArgb(60, 60, 90);
            btnAktualisieren.ForeColor = Color.White;
            btnAktualisieren.FlatStyle = FlatStyle.Flat;
            btnAktualisieren.Cursor = Cursors.Hand;
            btnAktualisieren.Height = 40;
            btnAktualisieren.Width = 240;
            btnAktualisieren.Margin = new Padding(0, 0, 10, 0);
            btnAktualisieren.FlatAppearance.BorderSize = 0;
            btnAktualisieren.Click += BtnAktualisieren_Klick;

            Button btnZurueck = new Button();
            btnZurueck.Text = "← Zurück zum Menü";
            btnZurueck.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            btnZurueck.BackColor = Color.FromArgb(50, 50, 75);
            btnZurueck.ForeColor = Color.White;
            btnZurueck.FlatStyle = FlatStyle.Flat;
            btnZurueck.Cursor = Cursors.Hand;
            btnZurueck.Height = 40;
            btnZurueck.Width = 220;
            btnZurueck.Margin = new Padding(0, 0, 0, 0);
            btnZurueck.FlatAppearance.BorderSize = 0;
            btnZurueck.Click += BtnZurueck_Klick;

            btnPanel.Controls.AddRange(new Control[] { btnAktualisieren, btnZurueck });

            // Alles zusammensetzen
            hauptLayout.Controls.Add(lblTitel, 0, 0);
            hauptLayout.Controls.Add(pnlEingabe, 0, 1);
            hauptLayout.Controls.Add(lblAllleTipps, 0, 2);
            hauptLayout.Controls.Add(dgvTipps, 0, 3);
            hauptLayout.Controls.Add(btnPanel, 0, 4);

            this.Controls.Add(hauptLayout);
        }

        private void BtnAktualisieren_Klick(object sender, EventArgs e)
        {
            LadeTipps();
        }

        private void BtnZurueck_Klick(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Wenn man im Dropdown ein Spiel auswählt, ändert sich der Text darunter.
        /// </summary>
        private void CboSpiele_AuswahlGeaendert(object sender, EventArgs e)
        {
            if (cboSpiele.SelectedIndex >= 0)
            {
                lblSpielHinweis.Text = "✓ Spiel ausgewählt";
            }
        }

        /// <summary>
        /// Wird ausgeführt, wenn man auf "Tipp speichern" drückt.
        /// Hier wird der Tipp in die MySQL Datenbank eingetragen.
        /// </summary>
        private void BtnTippSpeichern_Klick(object sender, EventArgs e)
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
            int spielId = spielIDs[cboSpiele.SelectedIndex];
            int t1 = (int)nudTippTeam1.Value;
            int t2 = (int)nudTippTeam2.Value;

            try
            {
                using (MySqlConnection conn = DatenbankVerbindung.VerbindungAbrufen())
                {
                    conn.Open();

                    // 1. Zuerst prüfen wir, ob dieser Name für dieses Spiel schon getippt hat.
                    // Wir zählen einfach (COUNT), wie viele Tipps es von ihm für das Spiel gibt.
                    string checkSql = "SELECT COUNT(*) FROM tipps WHERE spiel_id = @sid AND benutzername = @name";

                    // Wir loggen den Befehl (mit Werten eingefügt, damit man es im Terminal lesen kann)
                    SQLProtokollierer.Protokollieren("SELECT COUNT(*) FROM tipps WHERE spiel_id = " + spielId + " AND benutzername = '" + name + "'");

                    long count = 0;
                    using (MySqlCommand checkCmd = new MySqlCommand(checkSql, conn))
                    {
                        checkCmd.Parameters.AddWithValue("@sid", spielId);
                        checkCmd.Parameters.AddWithValue("@name", name);
                        count = (long)checkCmd.ExecuteScalar(); // Führt den Befehl aus und gibt die Zahl zurück
                    }

                    // Wenn er schon getippt hat (count > 0), brechen wir ab!
                    if (count > 0)
                    {
                        MessageBox.Show(
                            "'" + name + "' hat für dieses Spiel bereits einen Tipp abgegeben!\nJeder Benutzer kann pro Spiel nur einmal tippen.",
                            "Tipp bereits vorhanden", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    // 2. Wenn wir hier sind, darf er tippen. Wir fügen den Tipp ein (INSERT).
                    string sql = "INSERT INTO tipps (spiel_id, benutzername, tipp_team1, tipp_team2) VALUES (@sid, @name, @t1, @t2)";

                    SQLProtokollierer.Protokollieren("INSERT INTO tipps (spiel_id, benutzername, tipp_team1, tipp_team2) VALUES ("
                        + spielId + ", '" + name + "', " + t1 + ", " + t2 + ")");

                    using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@sid", spielId);
                        cmd.Parameters.AddWithValue("@name", name);
                        cmd.Parameters.AddWithValue("@t1", t1);
                        cmd.Parameters.AddWithValue("@t2", t2);
                        cmd.ExecuteNonQuery(); // Tipp wird in MySQL gespeichert!
                    }

                    MessageBox.Show(
                        "Tipp von '" + name + "' (" + t1 + ":" + t2 + ") wurde erfolgreich gespeichert!",
                        "Tipp gespeichert", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Felder wieder aufräumen
                    txtBenutzername.Clear();
                    nudTippTeam1.Value = 0;
                    nudTippTeam2.Value = 0;

                    // Tabelle aktualisieren, damit der neue Tipp direkt sichtbar wird
                    LadeTipps();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fehler beim Speichern des Tipps:\n" + ex.Message,
                    "Datenbankfehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Sucht alle Spiele aus der Datenbank, bei denen "ergebnis_team1" noch leer (NULL) ist.
        /// Das sind Spiele, die noch nicht vorbei sind.
        /// </summary>
        private void LadeOffeneSpiele()
        {
            spielIDs.Clear();
            cboSpiele.Items.Clear();

            try
            {
                using (MySqlConnection conn = DatenbankVerbindung.VerbindungAbrufen())
                {
                    conn.Open();

                    string sql = "SELECT id, team1, team2, datum FROM spiele WHERE ergebnis_team1 IS NULL ORDER BY datum ASC";
                    SQLProtokollierer.Protokollieren(sql); // Logging

                    using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                    {
                        using (MySqlDataReader reader = cmd.ExecuteReader()) // Reader liest Zeile für Zeile aus MySQL
                        {
                            while (reader.Read()) // Solange es noch Zeilen gibt...
                            {
                                // ID geheim in unserer Liste speichern
                                spielIDs.Add(reader.GetInt32("id"));

                                // Schönen Text für das Dropdown zusammenbauen
                                string datum = reader.GetDateTime("datum").ToString("dd.MM.yyyy HH:mm");
                                cboSpiele.Items.Add(reader.GetString("team1") + " vs. " + reader.GetString("team2") + " (" + datum + ")");
                            }
                        }
                    }

                    // Wenn es gar keine Spiele gibt (z.B. frisch installiert)
                    if (cboSpiele.Items.Count == 0)
                    {
                        cboSpiele.Items.Add("Keine offenen Spiele vorhanden");
                        cboSpiele.Enabled = false;         // Dropdown sperren
                        btnTippSpeichern.Enabled = false;  // Button sperren
                        lblSpielHinweis.Text = "Aktuell keine Spiele zum Tippen verfügbar.";
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fehler beim Laden der Spiele:\n" + ex.Message,
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
                using (MySqlConnection conn = DatenbankVerbindung.VerbindungAbrufen())
                {
                    conn.Open();

                    // JOIN: Verbindet die Tabelle "tipps" (t) mit "spiele" (s) anhand der spiel_id.
                    string sql = @"SELECT t.id, CONCAT(s.team1, ' vs. ', s.team2) AS 'Spiel',
                               t.benutzername AS 'Name',
                               CONCAT(t.tipp_team1, ':', t.tipp_team2) AS 'Tipp',
                               t.punkte AS 'Punkte'
                               FROM tipps t
                               JOIN spiele s ON t.spiel_id = s.id
                               ORDER BY s.datum ASC, t.benutzername ASC";

                    SQLProtokollierer.Protokollieren(sql); // Logging

                    using (MySqlDataAdapter adapter = new MySqlDataAdapter(sql, conn))
                    {
                        System.Data.DataTable table = new System.Data.DataTable();
                        adapter.Fill(table);
                        dgvTipps.DataSource = table;
                    }

                    // ID verstecken
                    if (dgvTipps.Columns.Contains("id"))
                    {
                        dgvTipps.Columns["id"].Visible = false;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fehler beim Laden der Tipps:\n" + ex.Message,
                    "Datenbankfehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // =========================================================
        // Hilfsmethoden fürs Design (Farben, Schriftart etc.)
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

        private static NumericUpDown ZahlenFeldErstellen()
        {
            NumericUpDown nud = new NumericUpDown();
            nud.Minimum = 0;
            nud.Maximum = 30; // Man kann maximal 30 Tore tippen :)
            nud.Value = 0;
            nud.Font = new Font("Segoe UI", 13, FontStyle.Bold);
            nud.BackColor = Color.FromArgb(50, 50, 70);
            nud.ForeColor = Color.White;
            nud.Width = 70;
            nud.Height = 36;
            nud.Margin = new Padding(0, 2, 8, 0);
            return nud;
        }

        private static void TabelleStylen(DataGridView dgv)
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
