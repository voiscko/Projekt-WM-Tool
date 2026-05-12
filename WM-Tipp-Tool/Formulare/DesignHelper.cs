using System.Drawing;
using System.Windows.Forms;

namespace WMTippTool.Formulare
{
    // Enthält alle gemeinsamen Design-Hilfsmethoden für die Fenster.
    // Diese Datei gehört zu Mark (Zusammensetzen von allem).
    internal static class DesignHelper
    {
        public static Label LabelStylen(string text)
        {
            Label lbl = new Label();
            lbl.Text = text;
            lbl.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            lbl.ForeColor = Color.FromArgb(180, 180, 210);
            lbl.Dock = DockStyle.Fill;
            lbl.TextAlign = ContentAlignment.MiddleLeft;
            return lbl;
        }

        public static void TabelleStylen(DataGridView dgv)
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

        public static NumericUpDown ZahlenFeldErstellen()
        {
            NumericUpDown nud = new NumericUpDown();
            nud.Minimum = 0;
            nud.Maximum = 30;
            nud.Value = 0;
            nud.Font = new Font("Segoe UI", 13, FontStyle.Bold);
            nud.BackColor = Color.FromArgb(50, 50, 70);
            nud.ForeColor = Color.White;
            nud.Width = 70;
            nud.Height = 36;
            nud.Margin = new Padding(0, 2, 8, 0);
            return nud;
        }

        public static TextBox TextFeldStylen()
        {
            TextBox txt = new TextBox();
            txt.Dock = DockStyle.Fill;
            txt.Font = new Font("Segoe UI", 11);
            txt.BackColor = Color.FromArgb(50, 50, 70);
            txt.ForeColor = Color.White;
            txt.BorderStyle = BorderStyle.FixedSingle;
            txt.Margin = new Padding(0, 4, 8, 4);
            return txt;
        }

        public static Button AktionsButtonErstellen(string text, Color backColor)
        {
            Button btn = new Button();
            btn.Text = text;
            btn.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            btn.BackColor = backColor;
            btn.ForeColor = Color.White;
            btn.FlatStyle = FlatStyle.Flat;
            btn.Cursor = Cursors.Hand;
            btn.Height = 40;
            btn.AutoSize = false;
            btn.Width = 260;
            btn.Margin = new Padding(0, 0, 10, 0);
            btn.FlatAppearance.BorderSize = 0;
            return btn;
        }
    }
}
