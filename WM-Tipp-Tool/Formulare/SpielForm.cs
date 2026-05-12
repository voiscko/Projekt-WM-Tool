using System;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using WMTippTool.Datenbank;

namespace WMTippTool.Formulare
{
    // Das Fenster, in dem Spiele angelegt und gelöscht werden können.
    // Das Aussehen (Design) ist in SpielFormDesign.cs ausgelagert.
    // Diese Datei hier enthält nur die Logik: SQL-Abfragen und Button-Aktionen.
    public partial class SpielForm : Form
    {
        // Konstruktor: Wird aufgerufen, wenn das Fenster geöffnet wird.
        public SpielForm()
        {
            KomponentenInitialisieren(); // Baut das Fenster auf (in SpielFormDesign.cs)
            LadeSpiele();                // Zeigt alle Spiele sofort in der Tabelle an
        }

        // Wird ausgeführt, wenn man auf "Spiel hinzufügen" drückt
        private void BtnHinzufuegen_Klick(object sender, EventArgs e)
        {
            string team1 = txtTeam1.Text.Trim();
            string team2 = txtTeam2.Text.Trim();

            // Sicherheitscheck: Wurden beide Teamnamen eingegeben?
            if (string.IsNullOrEmpty(team1) || string.IsNullOrEmpty(team2))
            {
                MessageBox.Show("Bitte beide Teamnamen eingeben!", "Pflichtfelder fehlen", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            // Ein Team kann nicht gegen sich selbst spielen
            if (team1 == team2)
            {
                MessageBox.Show("Team 1 und Team 2 dürfen nicht identisch sein!", "Ungültige Eingabe", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (MySqlConnection conn = DatenbankVerbindung.VerbindungAbrufen())
                {
                    conn.Open();

                    // Spiel in die Datenbank schreiben (INSERT)
                    // Die @-Zeichen sind Platzhalter, die uns vor SQL-Injection schützen!
                    string sql = "INSERT INTO spiele (team1, team2, datum) VALUES (@t1, @t2, @datum)";
                    string logSql = "INSERT INTO spiele ... VALUES ('" + team1 + "', '" + team2 + "', '" + dtpDatum.Value.ToString("yyyy-MM-dd HH:mm:ss") + "')";
                    SQLProtokollierer.Protokollieren(logSql);

                    using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@t1", team1);
                        cmd.Parameters.AddWithValue("@t2", team2);
                        cmd.Parameters.AddWithValue("@datum", dtpDatum.Value);
                        cmd.ExecuteNonQuery(); // "Los, mach es!"
                    }

                    MessageBox.Show("Spiel '" + team1 + " vs. " + team2 + "' wurde hinzugefügt!", "Erfolg", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    txtTeam1.Clear();
                    txtTeam2.Clear();
                    LadeSpiele();
                }
            }
            catch (Exception ex) { MessageBox.Show("Fehler: " + ex.Message, "Datenbankfehler", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        // Wird ausgeführt, wenn man ein Spiel auswählt und auf "Löschen" drückt
        private void BtnLoeschen_Klick(object sender, EventArgs e)
        {
            if (dgvSpiele.SelectedRows.Count == 0)
            {
                MessageBox.Show("Bitte wählen Sie ein Spiel aus der Liste aus.", "Keine Auswahl", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Wir holen uns die ID und den Namen des ausgewählten Spiels
            int spielId = Convert.ToInt32(dgvSpiele.SelectedRows[0].Cells["id"].Value);
            string spielName = dgvSpiele.SelectedRows[0].Cells["Team 1"].Value.ToString() + " vs. " + dgvSpiele.SelectedRows[0].Cells["Team 2"].Value.ToString();

            // Sicherheitsfrage: Wirklich löschen?
            DialogResult result = MessageBox.Show("Soll '" + spielName + "' wirklich gelöscht werden?\nAlle Tipps dazu werden ebenfalls gelöscht!", "Löschen bestätigen", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result != DialogResult.Yes) return;

            try
            {
                using (MySqlConnection conn = DatenbankVerbindung.VerbindungAbrufen())
                {
                    conn.Open();
                    SQLProtokollierer.Protokollieren("DELETE FROM spiele WHERE id = " + spielId);
                    using (MySqlCommand cmd = new MySqlCommand("DELETE FROM spiele WHERE id = @id", conn))
                    {
                        cmd.Parameters.AddWithValue("@id", spielId);
                        cmd.ExecuteNonQuery(); // Löscht das Spiel aus MySQL
                    }
                    MessageBox.Show("Spiel wurde erfolgreich gelöscht.", "Gelöscht", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LadeSpiele();
                }
            }
            catch (Exception ex) { MessageBox.Show("Fehler: " + ex.Message, "Datenbankfehler", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        // Holt alle Spiele aus der Datenbank und zeigt sie in der Tabelle an
        private void LadeSpiele()
        {
            try
            {
                using (MySqlConnection conn = DatenbankVerbindung.VerbindungAbrufen())
                {
                    conn.Open();
                    string sql = @"SELECT id, team1 AS 'Team 1', team2 AS 'Team 2',
                               DATE_FORMAT(datum, '%d.%m.%Y %H:%i') AS 'Datum',
                               IFNULL(CONCAT(ergebnis_team1, ':', ergebnis_team2), '—') AS 'Ergebnis'
                               FROM spiele ORDER BY datum ASC";
                    SQLProtokollierer.Protokollieren(sql);
                    using (MySqlDataAdapter adapter = new MySqlDataAdapter(sql, conn))
                    {
                        System.Data.DataTable table = new System.Data.DataTable();
                        adapter.Fill(table);
                        dgvSpiele.DataSource = table;
                    }
                    if (dgvSpiele.Columns.Contains("id")) dgvSpiele.Columns["id"].Visible = false;
                }
            }
            catch (Exception ex) { MessageBox.Show("Fehler: " + ex.Message, "Datenbankfehler", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private void BtnAktualisieren_Klick(object sender, EventArgs e) { LadeSpiele(); }
        private void BtnZurueck_Klick(object sender, EventArgs e) { this.Close(); }
    }
}
