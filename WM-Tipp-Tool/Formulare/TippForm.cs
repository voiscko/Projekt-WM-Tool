using System;
using System.Collections.Generic;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using WMTippTool.Datenbank;

namespace WMTippTool.Formulare
{
    // Das Fenster, in dem Spieler ihre Tipps abgeben können.
    // Das Aussehen (Design) ist in TippFormDesign.cs ausgelagert.
    // Diese Datei hier enthält nur die Logik: SQL-Abfragen und Button-Aktionen.
    public partial class TippForm : Form
    {
        // Speichert die echten Datenbank-IDs der Spiele im Hintergrund.
        // Das Dropdown zeigt Text, wir brauchen für SQL aber die ID.
        private List<int> spielIDs = new List<int>();

        // Konstruktor: Wird aufgerufen, wenn das Fenster geöffnet wird.
        public TippForm()
        {
            KomponentenInitialisieren(); // Baut das Fenster auf (in TippFormDesign.cs)
            LadeOffeneSpiele();          // Füllt das Dropdown mit Spielen
            LadeTipps();                 // Zeigt alle bisherigen Tipps in der Tabelle
        }

        // Wenn man ein Spiel im Dropdown auswählt, ändert sich der Hinweis-Text
        private void CboSpiele_AuswahlGeaendert(object sender, EventArgs e)
        {
            if (cboSpiele.SelectedIndex >= 0)
                lblSpielHinweis.Text = "✓ Spiel ausgewählt";
        }

        // Wird ausgeführt, wenn man auf "Tipp speichern" drückt
        private void BtnTippSpeichern_Klick(object sender, EventArgs e)
        {
            // Eingaben prüfen
            if (cboSpiele.SelectedIndex < 0)
            {
                MessageBox.Show("Bitte wählen Sie ein Spiel aus!", "Kein Spiel ausgewählt", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            string name = txtBenutzername.Text.Trim();
            if (string.IsNullOrEmpty(name))
            {
                MessageBox.Show("Bitte geben Sie Ihren Namen ein!", "Name fehlt", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int spielId = spielIDs[cboSpiele.SelectedIndex]; // Echte ID aus der Hintergrundliste
            int t1 = (int)nudTippTeam1.Value;
            int t2 = (int)nudTippTeam2.Value;

            try
            {
                using (MySqlConnection conn = DatenbankVerbindung.VerbindungAbrufen())
                {
                    conn.Open();

                    // 1. Duplikat-Check: Hat dieser Name schon für dieses Spiel getippt?
                    string checkSql = "SELECT COUNT(*) FROM tipps WHERE spiel_id = @sid AND benutzername = @name";
                    SQLProtokollierer.Protokollieren("SELECT COUNT(*) FROM tipps WHERE spiel_id = " + spielId + " AND benutzername = '" + name + "'");
                    long count = 0;
                    using (MySqlCommand checkCmd = new MySqlCommand(checkSql, conn))
                    {
                        checkCmd.Parameters.AddWithValue("@sid", spielId);
                        checkCmd.Parameters.AddWithValue("@name", name);
                        count = (long)checkCmd.ExecuteScalar();
                    }
                    if (count > 0)
                    {
                        MessageBox.Show("'" + name + "' hat für dieses Spiel bereits getippt!", "Tipp vorhanden", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    // 2. Tipp in die Datenbank schreiben (INSERT)
                    string sql = "INSERT INTO tipps (spiel_id, benutzername, tipp_team1, tipp_team2) VALUES (@sid, @name, @t1, @t2)";
                    SQLProtokollierer.Protokollieren("INSERT INTO tipps ... VALUES (" + spielId + ", '" + name + "', " + t1 + ", " + t2 + ")");
                    using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@sid", spielId);
                        cmd.Parameters.AddWithValue("@name", name);
                        cmd.Parameters.AddWithValue("@t1", t1);
                        cmd.Parameters.AddWithValue("@t2", t2);
                        cmd.ExecuteNonQuery(); // Schreibt den Tipp in MySQL!
                    }

                    MessageBox.Show("Tipp von '" + name + "' (" + t1 + ":" + t2 + ") gespeichert!", "Erfolg", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    txtBenutzername.Clear();
                    nudTippTeam1.Value = 0;
                    nudTippTeam2.Value = 0;
                    LadeTipps();
                }
            }
            catch (Exception ex) { MessageBox.Show("Fehler: " + ex.Message, "Datenbankfehler", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        // Holt alle Spiele ohne Ergebnis aus der DB → zeigt sie im Dropdown an
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
                    SQLProtokollierer.Protokollieren(sql);
                    using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            spielIDs.Add(reader.GetInt32("id"));
                            string datum = reader.GetDateTime("datum").ToString("dd.MM.yyyy HH:mm");
                            cboSpiele.Items.Add(reader.GetString("team1") + " vs. " + reader.GetString("team2") + " (" + datum + ")");
                        }
                    }
                    if (cboSpiele.Items.Count == 0)
                    {
                        cboSpiele.Items.Add("Keine offenen Spiele vorhanden");
                        cboSpiele.Enabled = false;
                        btnTippSpeichern.Enabled = false;
                        lblSpielHinweis.Text = "Keine Spiele verfügbar.";
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show("Fehler: " + ex.Message, "Datenbankfehler", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        // Holt alle Tipps aus der DB und zeigt sie in der Tabelle an (JOIN mit spiele)
        private void LadeTipps()
        {
            try
            {
                using (MySqlConnection conn = DatenbankVerbindung.VerbindungAbrufen())
                {
                    conn.Open();
                    string sql = @"SELECT t.id, CONCAT(s.team1, ' vs. ', s.team2) AS 'Spiel',
                               t.benutzername AS 'Name', CONCAT(t.tipp_team1, ':', t.tipp_team2) AS 'Tipp',
                               t.punkte AS 'Punkte' FROM tipps t
                               JOIN spiele s ON t.spiel_id = s.id
                               ORDER BY s.datum ASC, t.benutzername ASC";
                    SQLProtokollierer.Protokollieren(sql);
                    using (MySqlDataAdapter adapter = new MySqlDataAdapter(sql, conn))
                    {
                        System.Data.DataTable table = new System.Data.DataTable();
                        adapter.Fill(table);
                        dgvTipps.DataSource = table;
                    }
                    if (dgvTipps.Columns.Contains("id")) dgvTipps.Columns["id"].Visible = false;
                }
            }
            catch (Exception ex) { MessageBox.Show("Fehler: " + ex.Message, "Datenbankfehler", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private void BtnAktualisieren_Klick(object sender, EventArgs e) { LadeTipps(); }
        private void BtnZurueck_Klick(object sender, EventArgs e) { this.Close(); }
    }
}
