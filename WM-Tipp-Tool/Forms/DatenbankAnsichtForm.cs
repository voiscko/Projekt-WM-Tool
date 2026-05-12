using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using WMTippTool.Database;

namespace WMTippTool.Forms
{
    /// <summary>
    /// In diesem Fenster können wir uns die Datenbank-Tabellen ("spiele" und "tipps")
    /// genau so ansehen, wie sie in MySQL gespeichert sind.
    /// Das ist super, um den Lehrern zu zeigen, dass die Daten wirklich in der Datenbank landen.
    /// </summary>
    public class DatenbankAnsichtForm : Form
    {
        // Ein Dropdown-Menü, um auszuwählen, welche Tabelle wir sehen wollen
        private ComboBox cboTabellen;
        // Die Tabelle, in der die Daten angezeigt werden
        private DataGridView dgvDaten;

        public DatenbankAnsichtForm()
        {
            KomponentenInitialisieren();

            // Wenn das Fenster öffnet, laden wir direkt die erste Tabelle ("spiele")
            cboTabellen.SelectedIndex = 0;
        }

        private void KomponentenInitialisieren()
        {
            // Fenster-Eigenschaften einstellen
            this.Text = "🗄️ Datenbanken ansehen";
            this.Size = new Size(900, 600);
            this.MinimumSize = new Size(600, 400);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(22, 22, 35); // Dunkler Hintergrund
            this.AutoScaleMode = AutoScaleMode.Dpi;

            // Ein Layout-Gitter erstellen, um die Elemente schön anzuordnen
            TableLayoutPanel hauptLayout = new TableLayoutPanel();
            hauptLayout.Dock = DockStyle.Fill;
            hauptLayout.ColumnCount = 1;
            hauptLayout.RowCount = 3;
            hauptLayout.BackColor = Color.FromArgb(22, 22, 35);
            hauptLayout.Padding = new Padding(14);

            // Wir definieren 3 Zeilen: Oben das Dropdown, in der Mitte die Tabelle, unten der Zurück-Button
            hauptLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 50f));  // Dropdown-Zeile
            hauptLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));  // Tabellen-Bereich
            hauptLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 50f));  // Button-Zeile

            // 1. Zeile: Das Dropdown-Menü
            FlowLayoutPanel oberesPanel = new FlowLayoutPanel();
            oberesPanel.Dock = DockStyle.Fill;
            oberesPanel.FlowDirection = FlowDirection.LeftToRight;
            oberesPanel.WrapContents = false;

            Label lblSelect = new Label();
            lblSelect.Text = "Tabelle auswählen:";
            lblSelect.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            lblSelect.ForeColor = Color.White;
            lblSelect.AutoSize = true;
            lblSelect.Margin = new Padding(0, 5, 10, 0);

            cboTabellen = new ComboBox();
            cboTabellen.Font = new Font("Segoe UI", 11);
            cboTabellen.BackColor = Color.FromArgb(50, 50, 70);
            cboTabellen.ForeColor = Color.White;
            cboTabellen.DropDownStyle = ComboBoxStyle.DropDownList;
            cboTabellen.FlatStyle = FlatStyle.Flat;
            cboTabellen.Width = 200;

            // Wir fügen die zwei Namen unserer Tabellen ins Dropdown ein
            cboTabellen.Items.Add("spiele");
            cboTabellen.Items.Add("tipps");
            // Wenn sich die Auswahl ändert, laden wir die Daten neu
            cboTabellen.SelectedIndexChanged += CboTabellen_AuswahlGeaendert;

            oberesPanel.Controls.Add(lblSelect);
            oberesPanel.Controls.Add(cboTabellen);

            // 2. Zeile: Die Tabelle (DataGridView)
            dgvDaten = new DataGridView();
            dgvDaten.Dock = DockStyle.Fill;
            dgvDaten.BackgroundColor = Color.FromArgb(28, 28, 45);
            dgvDaten.BorderStyle = BorderStyle.None;
            dgvDaten.RowHeadersVisible = false;
            dgvDaten.AllowUserToAddRows = false;    // Nutzer darf hier keine Daten ändern
            dgvDaten.AllowUserToDeleteRows = false; // Nutzer darf hier nichts löschen
            dgvDaten.ReadOnly = true;               // Nur-Lese-Modus
            dgvDaten.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvDaten.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvDaten.Font = new Font("Segoe UI", 11);

            // Ein bisschen Farbe für die Tabelle
            dgvDaten.DefaultCellStyle.BackColor = Color.FromArgb(28, 28, 45);
            dgvDaten.DefaultCellStyle.ForeColor = Color.White;
            dgvDaten.DefaultCellStyle.SelectionBackColor = Color.FromArgb(0, 120, 215);
            dgvDaten.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(40, 40, 65);
            dgvDaten.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(255, 210, 0); // Gelbe Überschriften
            dgvDaten.EnableHeadersVisualStyles = false;

            // 3. Zeile: Der Zurück-Button
            Button btnZurueck = new Button();
            btnZurueck.Text = "← Zurück";
            btnZurueck.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            btnZurueck.BackColor = Color.FromArgb(50, 50, 75);
            btnZurueck.ForeColor = Color.White;
            btnZurueck.FlatStyle = FlatStyle.Flat;
            btnZurueck.Cursor = Cursors.Hand;
            btnZurueck.Height = 40;
            btnZurueck.Width = 150;
            btnZurueck.FlatAppearance.BorderSize = 0;
            // Schließt dieses Fenster wenn geklickt
            btnZurueck.Click += BtnZurueck_Klick;

            // Alles in das Haupt-Layout einfügen
            hauptLayout.Controls.Add(oberesPanel, 0, 0);
            hauptLayout.Controls.Add(dgvDaten, 0, 1);
            hauptLayout.Controls.Add(btnZurueck, 0, 2);

            this.Controls.Add(hauptLayout);
        }

        // Wird aufgerufen wenn der Zurück-Button geklickt wird
        private void BtnZurueck_Klick(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Wird aufgerufen, wenn man im Dropdown-Menü eine andere Tabelle auswählt.
        /// </summary>
        private void CboTabellen_AuswahlGeaendert(object sender, EventArgs e)
        {
            if (cboTabellen.SelectedItem == null)
            {
                return;
            }

            // Der Name der ausgewählten Tabelle (z.B. "spiele" oder "tipps")
            string tableName = cboTabellen.SelectedItem.ToString();

            // Lade die Daten aus der MySQL Datenbank
            TabellenDatenLaden(tableName);
        }

        /// <summary>
        /// Verbindet sich mit der Datenbank und holt alle Daten aus der angegebenen Tabelle.
        /// </summary>
        private void TabellenDatenLaden(string tableName)
        {
            try
            {
                // Eine Verbindung zur Datenbank aufbauen
                using (MySqlConnection conn = DatenbankVerbindung.VerbindungAbrufen())
                {
                    conn.Open();

                    // Der SQL-Befehl lautet einfach "SELECT * FROM tabellenname"
                    // '*' bedeutet: Gib mir ALLE Spalten.
                    string sql = "SELECT * FROM " + tableName;

                    // Wir loggen das für unser SQL-Terminal!
                    SQLProtokollierer.Protokollieren(sql);

                    // Ein DataAdapter füllt unsere virtuelle Tabelle (DataTable) mit den Ergebnissen aus MySQL
                    using (MySqlDataAdapter adapter = new MySqlDataAdapter(sql, conn))
                    {
                        DataTable table = new DataTable();
                        adapter.Fill(table);

                        // Wir sagen dem DataGridView, dass es die Daten aus der DataTable anzeigen soll
                        dgvDaten.DataSource = table;
                    }
                }
            }
            catch (Exception ex)
            {
                // Falls es einen Fehler gibt (z.B. Datenbank nicht erreichbar), zeigen wir eine Fehlermeldung
                MessageBox.Show("Fehler beim Laden der Tabelle '" + tableName + "':\n" + ex.Message,
                    "Datenbankfehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
