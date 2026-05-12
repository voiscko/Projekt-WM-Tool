using System.Drawing;
using System.Windows.Forms;

namespace WMTippTool.Formulare
{
    // Diese Datei enthält nur das Aussehen (Design) des Spiele-Fensters.
    // Die eigentliche Logik (SQL, Buttons) steht in SpielForm.cs.
    public partial class SpielForm : Form
    {
        // === Bausteine (Controls) des Fensters ===
        private DataGridView dgvSpiele;
        private TextBox txtTeam1;
        private TextBox txtTeam2;
        private DateTimePicker dtpDatum;
        private Button btnHinzufuegen;
        private Button btnLoeschen;
        private Button btnAktualisieren;

        private void KomponentenInitialisieren()
        {
            this.Text = "🏟️ Spiele verwalten";
            this.MinimumSize = new Size(800, 580);
            this.Size = new Size(1100, 780);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(22, 22, 35);
            this.ForeColor = Color.White;
            this.AutoScaleMode = AutoScaleMode.Dpi;

            TableLayoutPanel hauptLayout = new TableLayoutPanel();
            hauptLayout.Dock = DockStyle.Fill;
            hauptLayout.ColumnCount = 1;
            hauptLayout.RowCount = 4;
            hauptLayout.BackColor = Color.FromArgb(22, 22, 35);
            hauptLayout.Padding = new Padding(14, 10, 14, 10);
            hauptLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 52f));
            hauptLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 165f));
            hauptLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));
            hauptLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 55f));
            hauptLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));

            Label lblTitel = new Label();
            lblTitel.Text = "🏟️ Spiele verwalten";
            lblTitel.Font = new Font("Segoe UI", 20, FontStyle.Bold);
            lblTitel.ForeColor = Color.FromArgb(255, 210, 0);
            lblTitel.Dock = DockStyle.Fill;
            lblTitel.TextAlign = ContentAlignment.MiddleLeft;

            Panel pnlEingabe = new Panel();
            pnlEingabe.Dock = DockStyle.Fill;
            pnlEingabe.BackColor = Color.FromArgb(35, 35, 55);
            pnlEingabe.BorderStyle = BorderStyle.FixedSingle;
            pnlEingabe.Padding = new Padding(14, 10, 14, 10);

            TableLayoutPanel eingabeLayout = new TableLayoutPanel();
            eingabeLayout.Dock = DockStyle.Fill;
            eingabeLayout.ColumnCount = 4;
            eingabeLayout.RowCount = 3;
            eingabeLayout.BackColor = Color.FromArgb(35, 35, 55);
            eingabeLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 75f));
            eingabeLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50f));
            eingabeLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 75f));
            eingabeLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50f));
            eingabeLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 33.3f));
            eingabeLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 33.3f));
            eingabeLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 33.4f));

            txtTeam1 = DesignHelper.TextFeldStylen();
            txtTeam2 = DesignHelper.TextFeldStylen();
            eingabeLayout.Controls.Add(DesignHelper.LabelStylen("Team 1:"), 0, 0);
            eingabeLayout.Controls.Add(txtTeam1, 1, 0);
            eingabeLayout.Controls.Add(DesignHelper.LabelStylen("Team 2:"), 2, 0);
            eingabeLayout.Controls.Add(txtTeam2, 3, 0);

            dtpDatum = new DateTimePicker();
            dtpDatum.Dock = DockStyle.Fill;
            dtpDatum.Format = DateTimePickerFormat.Custom;
            dtpDatum.CustomFormat = "dd.MM.yyyy HH:mm";
            dtpDatum.ShowUpDown = false;
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

            Label lblDatumHint = new Label();
            lblDatumHint.Text = "Format: TT.MM.JJJJ HH:MM";
            lblDatumHint.Font = new Font("Segoe UI", 9, FontStyle.Italic);
            lblDatumHint.ForeColor = Color.FromArgb(120, 120, 150);
            lblDatumHint.Dock = DockStyle.Fill;
            lblDatumHint.TextAlign = ContentAlignment.MiddleLeft;
            lblDatumHint.Padding = new Padding(8, 0, 0, 0);
            eingabeLayout.Controls.Add(DesignHelper.LabelStylen("Datum:"), 0, 1);
            eingabeLayout.Controls.Add(dtpPanel, 1, 1);
            eingabeLayout.Controls.Add(lblDatumHint, 2, 1);
            eingabeLayout.SetColumnSpan(lblDatumHint, 2);

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
            btnHinzufuegen.Click += BtnHinzufuegen_Klick;
            eingabeLayout.Controls.Add(btnHinzufuegen, 0, 2);
            eingabeLayout.SetColumnSpan(btnHinzufuegen, 2);
            pnlEingabe.Controls.Add(eingabeLayout);

            dgvSpiele = new DataGridView();
            dgvSpiele.Dock = DockStyle.Fill;
            dgvSpiele.BackgroundColor = Color.FromArgb(28, 28, 45);
            dgvSpiele.BorderStyle = BorderStyle.None;
            dgvSpiele.RowHeadersVisible = false;
            dgvSpiele.AllowUserToAddRows = false;
            dgvSpiele.AllowUserToDeleteRows = false;
            dgvSpiele.ReadOnly = true;
            dgvSpiele.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvSpiele.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvSpiele.Font = new Font("Segoe UI", 10);
            dgvSpiele.GridColor = Color.FromArgb(60, 60, 80);
            dgvSpiele.Margin = new Padding(0, 8, 0, 8);
            DesignHelper.TabelleStylen(dgvSpiele);

            btnLoeschen = DesignHelper.AktionsButtonErstellen("🗑️ Ausgewähltes Spiel löschen", Color.FromArgb(180, 40, 40));
            btnLoeschen.Click += BtnLoeschen_Klick;
            btnAktualisieren = DesignHelper.AktionsButtonErstellen("🔄 Aktualisieren", Color.FromArgb(60, 60, 90));
            btnAktualisieren.Click += BtnAktualisieren_Klick;
            Button btnZurueck = DesignHelper.AktionsButtonErstellen("← Zurück zum Menü", Color.FromArgb(50, 50, 75));
            btnZurueck.Click += BtnZurueck_Klick;

            FlowLayoutPanel btnPanel = new FlowLayoutPanel();
            btnPanel.Dock = DockStyle.Fill;
            btnPanel.FlowDirection = FlowDirection.LeftToRight;
            btnPanel.BackColor = Color.FromArgb(22, 22, 35);
            btnPanel.WrapContents = false;
            btnPanel.Padding = new Padding(0, 6, 0, 0);
            btnPanel.Controls.AddRange(new Control[] { btnLoeschen, btnAktualisieren, btnZurueck });

            hauptLayout.Controls.Add(lblTitel, 0, 0);
            hauptLayout.Controls.Add(pnlEingabe, 0, 1);
            hauptLayout.Controls.Add(dgvSpiele, 0, 2);
            hauptLayout.Controls.Add(btnPanel, 0, 3);
            this.Controls.Add(hauptLayout);
        }

        private void DtpPanel_GroesseGeaendert(object sender, System.EventArgs e)
        {
            Panel pnl = (Panel)sender;
            dtpDatum.Width = pnl.Width;
        }
    }
}
