using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using WMTippTool.Database;

namespace WMTippTool.Forms
{
    /// <summary>
    /// In diesem Fenster geht es um Ergebnisse und die Rangliste!
    /// Hier trägt der Admin (z.B. der Lehrer) das echte Ergebnis eines Spiels ein.
    /// Danach berechnet das Programm automatisch für alle Schüler die Punkte
    /// und zeigt an, wer auf welchem Platz in der Rangliste steht.
    /// </summary>
    public class RanglisteForm : Form
    {
        // === Unsere Bausteine für dieses Fenster ===
        private ComboBox cboSpiele = null!; // Dropdown, um ein Spiel auszuwählen
        private NumericUpDown nudErgebnisTeam1 = null!; // Eingabe: Tore Team 1
        private NumericUpDown nudErgebnisTeam2 = null!; // Eingabe: Tore Team 2
        private Button btnErgebnisEintragen = null!; // Button zum Speichern und Berechnen
        
        private DataGridView dgvRangliste = null!; // Tabelle links: Die große Gesamtrangliste
        private DataGridView dgvSpieleTipps = null!; // Tabelle rechts: Wer hat was für dieses Spiel getippt?
        
        private Label lblSpielInfo = null!; // Kleines Info-Textfeld
        
        // Liste für die geheimen Spiel-IDs im Hintergrund
        private List<int> spielIds = new();

        public RanglisteForm()
        {
            InitializeComponent();
            LadeSpieleFuerErgebnis(); // Holt alle Spiele für das Dropdown-Menü
            LadeRangliste(); // Berechnet und lädt die aktuelle Rangliste
        }

        /// <summary>
        /// Baut das Fenster und ordnet alles an.
        /// </summary>
        private void InitializeComponent()
        {
            // Fenstereigenschaften
            this.Text = "🏆 Rangliste & Ergebnisse";
            this.MinimumSize = new Size(900, 680);
            this.Size = new Size(1280, 900); // Großes Fenster, da wir zwei Tabellen haben
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(22, 22, 35);
            this.AutoScaleMode = AutoScaleMode.Dpi;

            // ===== Haupt-Layout =====
            var rootLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 4,
                BackColor = Color.FromArgb(22, 22, 35),
                Padding = new Padding(14, 10, 14, 10)
            };
            // 4 Zeilen: Titel | Ergebnis-Eingabe oben | Tabellen in der Mitte | Buttons unten
            rootLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 52f));
            rootLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 170f));
            rootLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));
            rootLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 55f));
            rootLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));

            // ----- 1. Titel -----
            var lblTitel = new Label
            {
                Text = "🏆 Rangliste & Ergebnisse",
                Font = new Font("Segoe UI", 20, FontStyle.Bold),
                ForeColor = Color.FromArgb(255, 210, 0),
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft
            };

            // ===== 2. Ergebnis-Panel oben =====
            var pnlErgebnis = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(35, 35, 55),
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(14, 8, 14, 8)
            };

            var ergebnisLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 3,
                BackColor = Color.FromArgb(35, 35, 55)
            };
            ergebnisLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 32f));
            ergebnisLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 50f));
            ergebnisLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 50f));
            ergebnisLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));

            var lblErgebnisTitel = new Label
            {
                Text = "📝 Ergebnis eintragen",
                Font = new Font("Segoe UI", 13, FontStyle.Bold),
                ForeColor = Color.FromArgb(255, 210, 0),
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft
            };

            var spielZeileLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 3,
                RowCount = 1,
                BackColor = Color.FromArgb(35, 35, 55)
            };
            spielZeileLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 65f));
            spielZeileLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
            spielZeileLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 200f));
            spielZeileLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));

            var lblSpiel = StyleLabel("Spiel:");
            cboSpiele = new ComboBox
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 11),
                BackColor = Color.FromArgb(50, 50, 70),
                ForeColor = Color.White,
                DropDownStyle = ComboBoxStyle.DropDownList,
                FlatStyle = FlatStyle.Flat,
                Margin = new Padding(0, 4, 8, 4)
            };
            // Wenn der Nutzer ein Spiel im Dropdown ändert, zeige die Tipps für DIESES Spiel rechts an.
            cboSpiele.SelectedIndexChanged += CboSpiele_SelectedIndexChanged;

            lblSpielInfo = new Label
            {
                Text = "",
                Font = new Font("Segoe UI", 9, FontStyle.Italic),
                ForeColor = Color.Gray,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft
            };

            spielZeileLayout.Controls.Add(lblSpiel, 0, 0);
            spielZeileLayout.Controls.Add(cboSpiele, 1, 0);
            spielZeileLayout.Controls.Add(lblSpielInfo, 2, 0);

            var ergebnisEingabeFlow = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight,
                BackColor = Color.FromArgb(35, 35, 55),
                WrapContents = false,
                Padding = new Padding(0, 2, 0, 2)
            };

            var lblErgebnis = new Label { Text = "Ergebnis:", Font = new Font("Segoe UI", 10, FontStyle.Bold), ForeColor = Color.FromArgb(180, 180, 210), AutoSize = true, Margin = new Padding(0, 8, 10, 0) };
            var lblT1 = new Label { Text = "Team 1:", Font = new Font("Segoe UI", 10, FontStyle.Bold), ForeColor = Color.FromArgb(180, 180, 210), AutoSize = true, Margin = new Padding(0, 8, 6, 0) };
            nudErgebnisTeam1 = CreateNumericUpDown();
            var lblDp = new Label { Text = ":", Font = new Font("Segoe UI", 16, FontStyle.Bold), ForeColor = Color.White, AutoSize = true, Margin = new Padding(6, 4, 6, 0) };
            var lblT2 = new Label { Text = "Team 2:", Font = new Font("Segoe UI", 10, FontStyle.Bold), ForeColor = Color.FromArgb(180, 180, 210), AutoSize = true, Margin = new Padding(0, 8, 6, 0) };
            nudErgebnisTeam2 = CreateNumericUpDown();

            btnErgebnisEintragen = new Button
            {
                Text = "💾 Ergebnis speichern & Punkte berechnen",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                BackColor = Color.FromArgb(180, 100, 0), // Orange
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                Height = 38,
                Width = 400,
                Margin = new Padding(20, 2, 0, 0)
            };
            btnErgebnisEintragen.FlatAppearance.BorderSize = 0;
            // Ruft die Methode auf, wenn der Button geklickt wird
            btnErgebnisEintragen.Click += BtnErgebnisEintragen_Click;

            ergebnisEingabeFlow.Controls.AddRange(new Control[]
            {
                lblErgebnis, lblT1, nudErgebnisTeam1, lblDp, lblT2, nudErgebnisTeam2, btnErgebnisEintragen
            });

            ergebnisLayout.Controls.Add(lblErgebnisTitel, 0, 0);
            ergebnisLayout.Controls.Add(spielZeileLayout, 0, 1);
            ergebnisLayout.Controls.Add(ergebnisEingabeFlow, 0, 2);

            pnlErgebnis.Controls.Add(ergebnisLayout);

            // ===== 3. Split-Bereich (Links: Rangliste | Rechts: Tipps zum Spiel) =====
            // Wir machen ein Layout, das in zwei Spalten (Links 45%, Rechts 55%) geteilt ist.
            var splitLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 2,
                BackColor = Color.FromArgb(22, 22, 35),
                Margin = new Padding(0, 8, 0, 0)
            };
            splitLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 45f));  
            splitLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 55f));  
            splitLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 34f));       
            splitLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));       

            var lblRanglisteTitel = new Label
            {
                Text = "🏆 Gesamtrangliste:",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.FromArgb(255, 210, 0),
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.BottomLeft,
                Padding = new Padding(0, 0, 0, 2)
            };
            var lblTippsTitle = new Label
            {
                Text = "📋 Tipps zum ausgewählten Spiel:",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.FromArgb(180, 180, 220),
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.BottomLeft,
                Padding = new Padding(8, 0, 0, 2)
            };

            // Linke Tabelle (Rangliste)
            dgvRangliste = new DataGridView
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
                Font = new Font("Segoe UI", 11),
                GridColor = Color.FromArgb(60, 60, 80),
                Margin = new Padding(0, 0, 6, 0)
            };
            StyleDataGridView(dgvRangliste, Color.FromArgb(180, 100, 0)); // Orangener Kopf

            // Rechte Tabelle (Wer hat wie für dieses Spiel getippt?)
            dgvSpieleTipps = new DataGridView
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
                Font = new Font("Segoe UI", 11),
                GridColor = Color.FromArgb(60, 60, 80),
                Margin = new Padding(6, 0, 0, 0)
            };
            StyleDataGridView(dgvSpieleTipps, Color.FromArgb(16, 137, 62)); // Grüner Kopf

            splitLayout.Controls.Add(lblRanglisteTitel, 0, 0);
            splitLayout.Controls.Add(lblTippsTitle, 1, 0);
            splitLayout.Controls.Add(dgvRangliste, 0, 1);
            splitLayout.Controls.Add(dgvSpieleTipps, 1, 1);

            // ----- 4. Button-Leiste unten -----
            var btnPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight,
                BackColor = Color.FromArgb(22, 22, 35),
                WrapContents = false,
                Padding = new Padding(0, 6, 0, 0)
            };

            var btnRefresh = new Button
            {
                Text = "🔄 Rangliste aktualisieren",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                BackColor = Color.FromArgb(60, 60, 90),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                Height = 40,
                Width = 260,
                Margin = new Padding(0, 0, 10, 0)
            };
            btnRefresh.FlatAppearance.BorderSize = 0;
            btnRefresh.Click += (s, e) => { LadeRangliste(); LadeSpieleFuerErgebnis(); };

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

            btnPanel.Controls.AddRange(new Control[] { btnRefresh, btnZurueck });

            // Alles ins Root-Layout einfügen
            rootLayout.Controls.Add(lblTitel, 0, 0);
            rootLayout.Controls.Add(pnlErgebnis, 0, 1);
            rootLayout.Controls.Add(splitLayout, 0, 2);
            rootLayout.Controls.Add(btnPanel, 0, 3);

            this.Controls.Add(rootLayout);
        }

        /// <summary>
        /// Wenn man im Dropdown ein anderes Spiel anklickt, rufen wir eine Methode auf,
        /// um die Tabelle auf der rechten Seite zu aktualisieren.
        /// </summary>
        private void CboSpiele_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (cboSpiele.SelectedIndex < 0) return;
            int spielId = spielIds[cboSpiele.SelectedIndex];
            LadeTippsZuSpiel(spielId);
        }

        /// <summary>
        /// Das ist das Herzstück: Wenn der Lehrer das echte Ergebnis speichert!
        /// 1. Wir speichern das Ergebnis bei dem Spiel (UPDATE spiele).
        /// 2. Wir laden alle Tipps für dieses Spiel.
        /// 3. Wir berechnen für jeden Tipp die Punkte (3 Punkte für exakt, 1 für Tendenz).
        /// 4. Wir speichern die Punkte bei den Tipps (UPDATE tipps).
        /// </summary>
        private void BtnErgebnisEintragen_Click(object? sender, EventArgs e)
        {
            if (cboSpiele.SelectedIndex < 0)
            {
                MessageBox.Show("Bitte wählen Sie ein Spiel aus!", "Kein Spiel ausgewählt",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int spielId = spielIds[cboSpiele.SelectedIndex];
            int e1 = (int)nudErgebnisTeam1.Value; // Echtes Ergebnis Team 1
            int e2 = (int)nudErgebnisTeam2.Value; // Echtes Ergebnis Team 2
            string spielName = cboSpiele.SelectedItem?.ToString() ?? "Unbekannt";

            var confirm = MessageBox.Show(
                $"Ergebnis für:\n'{spielName}'\n\nals {e1}:{e2} speichern und Punkte berechnen?",
                "Ergebnis bestätigen", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (confirm != DialogResult.Yes) return;

            try
            {
                using var conn = DBConnection.GetConnection();
                conn.Open();

                // SCHRITT 1: Speichere das Ergebnis im Spiel!
                string updateSpiel = "UPDATE spiele SET ergebnis_team1 = @e1, ergebnis_team2 = @e2 WHERE id = @sid";
                
                QueryLogger.Log($"UPDATE spiele SET ergebnis_team1 = {e1}, ergebnis_team2 = {e2} WHERE id = {spielId}");

                using (var cmd = new MySqlCommand(updateSpiel, conn))
                {
                    cmd.Parameters.AddWithValue("@e1", e1);
                    cmd.Parameters.AddWithValue("@e2", e2);
                    cmd.Parameters.AddWithValue("@sid", spielId);
                    cmd.ExecuteNonQuery();
                }

                // SCHRITT 2: Lade alle Tipps, die Schüler für dieses Spiel abgegeben haben.
                string ladeTipps = "SELECT id, tipp_team1, tipp_team2 FROM tipps WHERE spiel_id = @sid";
                QueryLogger.Log($"SELECT id, tipp_team1, tipp_team2 FROM tipps WHERE spiel_id = {spielId}");

                using var tippCmd = new MySqlCommand(ladeTipps, conn);
                tippCmd.Parameters.AddWithValue("@sid", spielId);

                var tipps = new List<(int id, int t1, int t2)>();
                using (var reader = tippCmd.ExecuteReader())
                {
                    while (reader.Read())
                        tipps.Add((reader.GetInt32("id"), reader.GetInt32("tipp_team1"), reader.GetInt32("tipp_team2")));
                }

                // SCHRITT 3 & 4: Punkte berechnen und speichern.
                int anzahlAktualisiert = 0;
                foreach (var (id, t1, t2) in tipps)
                {
                    // Unsere kleine Methode 'BerechnePunkte' vergleicht den Tipp (t1, t2) mit dem Ergebnis (e1, e2).
                    int punkte = BerechnePunkte(t1, t2, e1, e2);
                    
                    // Speichere die verdienten Punkte für diesen Nutzer in der Datenbank
                    string updateTipp = "UPDATE tipps SET punkte = @punkte WHERE id = @id";
                    QueryLogger.Log($"UPDATE tipps SET punkte = {punkte} WHERE id = {id}"); // Log fürs Terminal

                    using var updateCmd = new MySqlCommand(updateTipp, conn);
                    updateCmd.Parameters.AddWithValue("@punkte", punkte);
                    updateCmd.Parameters.AddWithValue("@id", id);
                    updateCmd.ExecuteNonQuery();
                    
                    anzahlAktualisiert++;
                }

                MessageBox.Show(
                    $"✅ Ergebnis gespeichert!\n\nPunkte für {anzahlAktualisiert} Tipp(s) wurden berechnet und aktualisiert.",
                    "Ergebnis gespeichert", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Lade Tabellen neu, damit man die Veränderungen sofort sieht.
                LadeSpieleFuerErgebnis();
                LadeRangliste();
                nudErgebnisTeam1.Value = 0;
                nudErgebnisTeam2.Value = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fehler beim Speichern des Ergebnisses:\n{ex.Message}",
                    "Datenbankfehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Die Mathematik hinter der Punkteberechnung:
        /// - 3 Punkte: Du hast das exakte Ergebnis getippt (z.B. Tipp: 2:1, Ergebnis: 2:1)
        /// - 1 Punkt: Du hattest die richtige Tendenz. Also du wusstest, dass Team 1 gewinnt (z.B. Tipp 3:0, Ergebnis 1:0)
        /// - 0 Punkte: Falsch getippt.
        /// </summary>
        private static int BerechnePunkte(int t1, int t2, int e1, int e2)
        {
            if (t1 == e1 && t2 == e2) return 3; // Exakt richtig!
            
            // "Math.Sign" gibt 1 zurück wenn die erste Zahl größer ist, 
            // -1 wenn sie kleiner ist, und 0 wenn beide gleich sind (Unentschieden).
            // Wenn die Vorzeichen übereinstimmen, war die Tendenz richtig.
            if (Math.Sign(t1 - t2) == Math.Sign(e1 - e2)) return 1; 
            
            return 0; // Leider komplett falsch.
        }

        /// <summary>
        /// Lädt die Spiele ins Dropdown. Wir laden auch die, die schon vorbei sind, 
        /// falls der Lehrer das Ergebnis nachträglich korrigieren will!
        /// </summary>
        private void LadeSpieleFuerErgebnis()
        {
            spielIds.Clear();
            cboSpiele.Items.Clear();

            try
            {
                using var conn = DBConnection.GetConnection();
                conn.Open();
                string sql = @"SELECT id, team1, team2, datum, ergebnis_team1, ergebnis_team2 
                               FROM spiele ORDER BY datum DESC";
                QueryLogger.Log(sql);

                using var cmd = new MySqlCommand(sql, conn);
                using var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    spielIds.Add(reader.GetInt32("id"));
                    string datum = reader.GetDateTime("datum").ToString("dd.MM.yyyy HH:mm");
                    
                    // Prüfen, ob schon ein Ergebnis da ist.
                    string ergebnis = reader.IsDBNull(reader.GetOrdinal("ergebnis_team1"))
                        ? "offen" // Noch nicht gespielt
                        : $"{reader.GetInt32("ergebnis_team1")}:{reader.GetInt32("ergebnis_team2")} ✓"; // Schon eingetragen
                        
                    cboSpiele.Items.Add($"{reader.GetString("team1")} vs. {reader.GetString("team2")} — {datum} [{ergebnis}]");
                }

                if (cboSpiele.Items.Count == 0)
                    cboSpiele.Items.Add("Keine Spiele vorhanden");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fehler beim Laden der Spiele:\n{ex.Message}",
                    "Datenbankfehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Lädt für die RECHTE Tabelle alle Tipps, die zum ausgewählten Spiel gehören.
        /// </summary>
        private void LadeTippsZuSpiel(int spielId)
        {
            try
            {
                using var conn = DBConnection.GetConnection();
                conn.Open();
                
                // Wir fügen ein "CASE WHEN" hinzu. Das ist wie ein IF im SQL.
                // Es zeigt direkt in der Tabelle Emojis an, je nachdem ob man 3, 1 oder 0 Punkte hat.
                string sql = @"SELECT t.benutzername AS 'Name',
                               CONCAT(t.tipp_team1, ':', t.tipp_team2) AS 'Tipp',
                               t.punkte AS 'Punkte',
                               CASE 
                                 WHEN t.punkte = 3 THEN '🥇 Exakt'
                                 WHEN t.punkte = 1 THEN '✅ Tendenz'
                                 ELSE '❌ Falsch'
                               END AS 'Bewertung'
                               FROM tipps t
                               WHERE t.spiel_id = @sid
                               ORDER BY t.punkte DESC, t.benutzername ASC";
                               
                QueryLogger.Log($"Lade Tipps für Spiel ID {spielId} ...");

                using var cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@sid", spielId);
                using var adapter = new MySqlDataAdapter(cmd);
                var table = new System.Data.DataTable();
                adapter.Fill(table);
                dgvSpieleTipps.DataSource = table;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fehler beim Laden der Tipps:\n{ex.Message}",
                    "Datenbankfehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Hier wird per SQL die LINKE große Gesamtrangliste berechnet!
        /// Das ist ein sehr komplexer SQL-Befehl (wichtig für die Lehrer!).
        /// </summary>
        private void LadeRangliste()
        {
            try
            {
                using var conn = DBConnection.GetConnection();
                conn.Open();
                
                // Erläuterung für die Präsentation:
                // ROW_NUMBER() verteilt die Platzierung (1, 2, 3...)
                // SUM(punkte) summiert die Punkte aller Tipps eines Nutzers zusammen.
                // COUNT(*) zählt, wie oft der Nutzer insgesamt getippt hat.
                // GROUP BY benutzername fasst alle Tipps eines Namens zusammen.
                string sql = @"SELECT 
                               ROW_NUMBER() OVER (ORDER BY SUM(punkte) DESC, COUNT(*) DESC) AS 'Platz',
                               benutzername AS 'Name',
                               SUM(punkte) AS 'Gesamtpunkte',
                               COUNT(*) AS 'Tipps gesamt',
                               SUM(CASE WHEN punkte = 3 THEN 1 ELSE 0 END) AS 'Exakt (3P)',
                               SUM(CASE WHEN punkte = 1 THEN 1 ELSE 0 END) AS 'Tendenz (1P)'
                               FROM tipps
                               GROUP BY benutzername
                               ORDER BY Gesamtpunkte DESC";
                               
                QueryLogger.Log(sql);

                using var adapter = new MySqlDataAdapter(sql, conn);
                var table = new System.Data.DataTable();
                adapter.Fill(table);
                dgvRangliste.DataSource = table;

                // Event abonnieren, um die Zeilen von Platz 1, 2 und 3 golden/silber/bronze zu färben!
                dgvRangliste.RowPrePaint -= DgvRangliste_RowPrePaint;
                dgvRangliste.RowPrePaint += DgvRangliste_RowPrePaint;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fehler beim Laden der Rangliste:\n{ex.Message}",
                    "Datenbankfehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Färbt die ersten 3 Plätze der Rangliste ein! (Gold, Silber, Bronze)
        /// </summary>
        private void DgvRangliste_RowPrePaint(object? sender, DataGridViewRowPrePaintEventArgs e)
        {
            if (e.RowIndex < 0 || e.RowIndex >= dgvRangliste.Rows.Count) return;
            var row = dgvRangliste.Rows[e.RowIndex];
            
            // "switch" prüft den Index der Zeile (0 = Erster, 1 = Zweiter, 2 = Dritter)
            row.DefaultCellStyle.BackColor = e.RowIndex switch
            {
                0 => Color.FromArgb(80, 65, 0),   // Gold
                1 => Color.FromArgb(50, 50, 60),  // Silber
                2 => Color.FromArgb(60, 35, 10),  // Bronze
                _ => Color.FromArgb(28, 28, 45)   // Normales Dunkelblau für den Rest
            };
        }

        // =========================================================
        // Hilfsmethoden fürs Design
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
            Maximum = 30,
            Value = 0,
            Font = new Font("Segoe UI", 13, FontStyle.Bold),
            BackColor = Color.FromArgb(50, 50, 70),
            ForeColor = Color.White,
            Width = 70,
            Height = 36,
            Margin = new Padding(0, 2, 8, 0)
        };

        private static void StyleDataGridView(DataGridView dgv, Color headerAccent)
        {
            dgv.DefaultCellStyle.BackColor = Color.FromArgb(28, 28, 45);
            dgv.DefaultCellStyle.ForeColor = Color.White;
            dgv.DefaultCellStyle.SelectionBackColor = Color.FromArgb(60, 60, 100);
            dgv.DefaultCellStyle.SelectionForeColor = Color.White;
            dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(40, 40, 65);
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = headerAccent;
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            dgv.EnableHeadersVisualStyles = false;
            dgv.ColumnHeadersHeight = 42;
            dgv.RowTemplate.Height = 36;
        }
    }
}
