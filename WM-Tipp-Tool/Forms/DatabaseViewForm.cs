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
    public class DatabaseViewForm : Form
    {
        // Ein Dropdown-Menü, um auszuwählen, welche Tabelle wir sehen wollen
        private ComboBox cboTables = null!;
        // Die Tabelle, in der die Daten angezeigt werden
        private DataGridView dgvData = null!;

        public DatabaseViewForm()
        {
            InitializeComponent();
            
            // Wenn das Fenster öffnet, laden wir direkt die erste Tabelle ("spiele")
            cboTables.SelectedIndex = 0; 
        }

        private void InitializeComponent()
        {
            // Fenster-Eigenschaften einstellen
            this.Text = "🗄️ Datenbanken ansehen";
            this.Size = new Size(900, 600);
            this.MinimumSize = new Size(600, 400);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(22, 22, 35); // Dunkler Hintergrund
            this.AutoScaleMode = AutoScaleMode.Dpi;

            // Ein Layout-Gitter erstellen, um die Elemente schön anzuordnen
            var rootLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 3,
                BackColor = Color.FromArgb(22, 22, 35),
                Padding = new Padding(14)
            };
            // Wir definieren 3 Zeilen: Oben das Dropdown, in der Mitte die Tabelle, unten der Zurück-Button
            rootLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 50f));  // Dropdown-Zeile
            rootLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));  // Tabellen-Bereich
            rootLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 50f));  // Button-Zeile

            // 1. Zeile: Das Dropdown-Menü
            var topPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false
            };

            var lblSelect = new Label
            {
                Text = "Tabelle auswählen:",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = true,
                Margin = new Padding(0, 5, 10, 0)
            };

            cboTables = new ComboBox
            {
                Font = new Font("Segoe UI", 11),
                BackColor = Color.FromArgb(50, 50, 70),
                ForeColor = Color.White,
                DropDownStyle = ComboBoxStyle.DropDownList,
                FlatStyle = FlatStyle.Flat,
                Width = 200
            };
            // Wir fügen die zwei Namen unserer Tabellen ins Dropdown ein
            cboTables.Items.Add("spiele");
            cboTables.Items.Add("tipps");
            // Wenn sich die Auswahl ändert, laden wir die Daten neu
            cboTables.SelectedIndexChanged += CboTables_SelectedIndexChanged;

            topPanel.Controls.Add(lblSelect);
            topPanel.Controls.Add(cboTables);

            // 2. Zeile: Die Tabelle (DataGridView)
            dgvData = new DataGridView
            {
                Dock = DockStyle.Fill,
                BackgroundColor = Color.FromArgb(28, 28, 45),
                BorderStyle = BorderStyle.None,
                RowHeadersVisible = false,
                AllowUserToAddRows = false, // Nutzer darf hier keine Daten ändern
                AllowUserToDeleteRows = false, // Nutzer darf hier nichts löschen
                ReadOnly = true, // Nur-Lese-Modus
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                Font = new Font("Segoe UI", 11)
            };

            // Ein bisschen Farbe für die Tabelle
            dgvData.DefaultCellStyle.BackColor = Color.FromArgb(28, 28, 45);
            dgvData.DefaultCellStyle.ForeColor = Color.White;
            dgvData.DefaultCellStyle.SelectionBackColor = Color.FromArgb(0, 120, 215);
            dgvData.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(40, 40, 65);
            dgvData.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(255, 210, 0); // Gelbe Überschriften
            dgvData.EnableHeadersVisualStyles = false;

            // 3. Zeile: Der Zurück-Button
            var btnZurueck = new Button
            {
                Text = "← Zurück",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                BackColor = Color.FromArgb(50, 50, 75),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                Height = 40,
                Width = 150
            };
            btnZurueck.FlatAppearance.BorderSize = 0;
            btnZurueck.Click += (s, e) => this.Close(); // Schließt dieses Fenster

            // Alles in das Haupt-Layout einfügen
            rootLayout.Controls.Add(topPanel, 0, 0);
            rootLayout.Controls.Add(dgvData, 0, 1);
            rootLayout.Controls.Add(btnZurueck, 0, 2);

            this.Controls.Add(rootLayout);
        }

        /// <summary>
        /// Wird aufgerufen, wenn man im Dropdown-Menü eine andere Tabelle auswählt.
        /// </summary>
        private void CboTables_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (cboTables.SelectedItem == null) return;
            
            // Der Name der ausgewählten Tabelle (z.B. "spiele" oder "tipps")
            string tableName = cboTables.SelectedItem.ToString()!;
            
            // Lade die Daten aus der MySQL Datenbank
            LoadTableData(tableName);
        }

        /// <summary>
        /// Verbindet sich mit der Datenbank und holt alle Daten aus der angegebenen Tabelle.
        /// </summary>
        private void LoadTableData(string tableName)
        {
            try
            {
                // Eine Verbindung zur Datenbank aufbauen
                using var conn = DBConnection.GetConnection();
                conn.Open();

                // Der SQL-Befehl lautet einfach "SELECT * FROM tabellenname"
                // '*' bedeutet: Gib mir ALLE Spalten.
                string sql = $"SELECT * FROM {tableName}";
                
                // Wir loggen das für unser SQL-Terminal!
                QueryLogger.Log(sql);

                // Ein DataAdapter füllt unsere virtuelle Tabelle (DataTable) mit den Ergebnissen aus MySQL
                using var adapter = new MySqlDataAdapter(sql, conn);
                var table = new DataTable();
                adapter.Fill(table);

                // Wir sagen dem DataGridView, dass es die Daten aus der DataTable anzeigen soll
                dgvData.DataSource = table;
            }
            catch (Exception ex)
            {
                // Falls es einen Fehler gibt (z.B. Datenbank nicht erreichbar), zeigen wir eine Fehlermeldung
                MessageBox.Show($"Fehler beim Laden der Tabelle '{tableName}':\n{ex.Message}",
                    "Datenbankfehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
