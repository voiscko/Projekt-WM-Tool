using System.Drawing;
using System.Windows.Forms;

namespace WMTippTool.Formulare
{
    // Diese Datei enthält nur das Aussehen (Design) des Tipp-Fensters.
    // Die eigentliche Logik (SQL, Buttons) steht in TippForm.cs.
    public partial class TippForm : Form
    {
        // === Bausteine (Controls) des Fensters ===
        private System.Windows.Forms.ComboBox cboSpiele;
        private System.Windows.Forms.TextBox txtBenutzername;
        private System.Windows.Forms.NumericUpDown nudTippTeam1;
        private System.Windows.Forms.NumericUpDown nudTippTeam2;
        private System.Windows.Forms.Button btnTippSpeichern;
        private System.Windows.Forms.DataGridView dgvTipps;
        private System.Windows.Forms.Label lblSpielHinweis;

        private void KomponentenInitialisieren()
        {
            this.Text = "✏️ Tipp abgeben";
            this.MinimumSize = new System.Drawing.Size(800, 620);
            this.Size = new System.Drawing.Size(1100, 820);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(22, 22, 35);
            this.AutoScaleMode = AutoScaleMode.Dpi;

            TableLayoutPanel hauptLayout = new TableLayoutPanel();
            hauptLayout.Dock = DockStyle.Fill;
            hauptLayout.ColumnCount = 1;
            hauptLayout.RowCount = 5;
            hauptLayout.BackColor = Color.FromArgb(22, 22, 35);
            hauptLayout.Padding = new Padding(14, 10, 14, 10);
            hauptLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 52f));
            hauptLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 215f));
            hauptLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 36f));
            hauptLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));
            hauptLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 55f));
            hauptLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));

            Label lblTitel = new Label();
            lblTitel.Text = "✏️ Tipp abgeben";
            lblTitel.Font = new Font("Segoe UI", 20, FontStyle.Bold);
            lblTitel.ForeColor = Color.FromArgb(255, 210, 0);
            lblTitel.Dock = DockStyle.Fill;
            lblTitel.TextAlign = ContentAlignment.MiddleLeft;

            Panel pnlEingabe = new Panel();
            pnlEingabe.Dock = DockStyle.Fill;
            pnlEingabe.BackColor = Color.FromArgb(35, 35, 55);
            pnlEingabe.BorderStyle = BorderStyle.FixedSingle;
            pnlEingabe.Padding = new Padding(14, 8, 14, 8);

            TableLayoutPanel eingabeLayout = new TableLayoutPanel();
            eingabeLayout.Dock = DockStyle.Fill;
            eingabeLayout.ColumnCount = 2;
            eingabeLayout.RowCount = 4;
            eingabeLayout.BackColor = Color.FromArgb(35, 35, 55);
            eingabeLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 190f));
            eingabeLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
            eingabeLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 25f));
            eingabeLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 26f));
            eingabeLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 25f));
            eingabeLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 50f));

            cboSpiele = new ComboBox();
            cboSpiele.Dock = DockStyle.Fill;
            cboSpiele.Font = new Font("Segoe UI", 11);
            cboSpiele.BackColor = Color.FromArgb(50, 50, 70);
            cboSpiele.ForeColor = Color.White;
            cboSpiele.DropDownStyle = ComboBoxStyle.DropDownList;
            cboSpiele.FlatStyle = FlatStyle.Flat;
            cboSpiele.Margin = new Padding(0, 4, 0, 2);
            cboSpiele.SelectedIndexChanged += CboSpiele_AuswahlGeaendert;
            eingabeLayout.Controls.Add(DesignHelper.LabelStylen("Spiel auswählen:"), 0, 0);
            eingabeLayout.Controls.Add(cboSpiele, 1, 0);

            lblSpielHinweis = new Label();
            lblSpielHinweis.Text = "Kein Spiel ausgewählt";
            lblSpielHinweis.Font = new Font("Segoe UI", 10, FontStyle.Italic);
            lblSpielHinweis.ForeColor = Color.Gray;
            lblSpielHinweis.Dock = DockStyle.Fill;
            lblSpielHinweis.TextAlign = ContentAlignment.MiddleLeft;
            lblSpielHinweis.Padding = new Padding(190, 0, 0, 0);
            eingabeLayout.Controls.Add(lblSpielHinweis, 0, 1);
            eingabeLayout.SetColumnSpan(lblSpielHinweis, 2);

            txtBenutzername = new TextBox();
            txtBenutzername.Dock = DockStyle.Fill;
            txtBenutzername.Font = new Font("Segoe UI", 11);
            txtBenutzername.BackColor = Color.FromArgb(50, 50, 70);
            txtBenutzername.ForeColor = Color.White;
            txtBenutzername.BorderStyle = BorderStyle.FixedSingle;
            txtBenutzername.Margin = new Padding(0, 4, 0, 4);
            eingabeLayout.Controls.Add(DesignHelper.LabelStylen("Ihr Name:"), 0, 2);
            eingabeLayout.Controls.Add(txtBenutzername, 1, 2);

            nudTippTeam1 = DesignHelper.ZahlenFeldErstellen();
            nudTippTeam2 = DesignHelper.ZahlenFeldErstellen();

            btnTippSpeichern = new Button();
            btnTippSpeichern.Text = "✅ Tipp speichern";
            btnTippSpeichern.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            btnTippSpeichern.BackColor = Color.FromArgb(16, 137, 62);
            btnTippSpeichern.ForeColor = Color.White;
            btnTippSpeichern.FlatStyle = FlatStyle.Flat;
            btnTippSpeichern.Cursor = Cursors.Hand;
            btnTippSpeichern.Height = 38;
            btnTippSpeichern.Width = 220;
            btnTippSpeichern.Margin = new Padding(20, 2, 0, 0);
            btnTippSpeichern.FlatAppearance.BorderSize = 0;
            btnTippSpeichern.Click += BtnTippSpeichern_Klick;

            Label lblTeam1 = new Label(); lblTeam1.Text = "Team 1:"; lblTeam1.Font = new Font("Segoe UI", 10, FontStyle.Bold); lblTeam1.ForeColor = Color.FromArgb(180, 180, 210); lblTeam1.AutoSize = true; lblTeam1.Margin = new Padding(0, 6, 6, 0);
            Label lblDoppelpunkt = new Label(); lblDoppelpunkt.Text = ":"; lblDoppelpunkt.Font = new Font("Segoe UI", 16, FontStyle.Bold); lblDoppelpunkt.ForeColor = Color.White; lblDoppelpunkt.AutoSize = true; lblDoppelpunkt.Margin = new Padding(6, 2, 6, 0);
            Label lblTeam2 = new Label(); lblTeam2.Text = "Team 2:"; lblTeam2.Font = new Font("Segoe UI", 10, FontStyle.Bold); lblTeam2.ForeColor = Color.FromArgb(180, 180, 210); lblTeam2.AutoSize = true; lblTeam2.Margin = new Padding(0, 6, 6, 0);

            FlowLayoutPanel tippFluss = new FlowLayoutPanel();
            tippFluss.Dock = DockStyle.Fill;
            tippFluss.FlowDirection = FlowDirection.LeftToRight;
            tippFluss.BackColor = Color.FromArgb(35, 35, 55);
            tippFluss.WrapContents = false;
            tippFluss.Padding = new Padding(0, 4, 0, 4);
            tippFluss.Controls.AddRange(new Control[] { lblTeam1, nudTippTeam1, lblDoppelpunkt, lblTeam2, nudTippTeam2, btnTippSpeichern });
            eingabeLayout.Controls.Add(DesignHelper.LabelStylen("Ihr Tipp:"), 0, 3);
            eingabeLayout.Controls.Add(tippFluss, 1, 3);
            pnlEingabe.Controls.Add(eingabeLayout);

            Label lblAlleTipps = new Label();
            lblAlleTipps.Text = "📋 Alle abgegebenen Tipps:";
            lblAlleTipps.Font = new Font("Segoe UI", 13, FontStyle.Bold);
            lblAlleTipps.ForeColor = Color.FromArgb(180, 180, 220);
            lblAlleTipps.Dock = DockStyle.Fill;
            lblAlleTipps.TextAlign = ContentAlignment.BottomLeft;

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
            DesignHelper.TabelleStylen(dgvTipps);

            Button btnAktualisieren = DesignHelper.AktionsButtonErstellen("🔄 Tipps aktualisieren", Color.FromArgb(60, 60, 90));
            btnAktualisieren.Click += BtnAktualisieren_Klick;
            Button btnZurueck = DesignHelper.AktionsButtonErstellen("← Zurück zum Menü", Color.FromArgb(50, 50, 75));
            btnZurueck.Click += BtnZurueck_Klick;

            FlowLayoutPanel btnPanel = new FlowLayoutPanel();
            btnPanel.Dock = DockStyle.Fill;
            btnPanel.FlowDirection = FlowDirection.LeftToRight;
            btnPanel.BackColor = Color.FromArgb(22, 22, 35);
            btnPanel.WrapContents = false;
            btnPanel.Padding = new Padding(0, 6, 0, 0);
            btnPanel.Controls.AddRange(new Control[] { btnAktualisieren, btnZurueck });

            hauptLayout.Controls.Add(lblTitel, 0, 0);
            hauptLayout.Controls.Add(pnlEingabe, 0, 1);
            hauptLayout.Controls.Add(lblAlleTipps, 0, 2);
            hauptLayout.Controls.Add(dgvTipps, 0, 3);
            hauptLayout.Controls.Add(btnPanel, 0, 4);
            this.Controls.Add(hauptLayout);
        }
    }
}
