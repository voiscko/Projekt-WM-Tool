using WMTippTool.Database;
using WMTippTool.Forms;

/*
 * Projekt: WM-Tipp-Tool 2026
 * Entwickler: voiscko (Telegram: @voiscko)
 * Beschreibung: Haupt-Einstiegspunkt der Anwendung
 */

namespace WMTippTool
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            // DPI-Awareness MUSS vor allem anderen gesetzt werden
            Application.SetHighDpiMode(HighDpiMode.PerMonitorV2);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            try
            {
                DBConnection.InitDatabase();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Fehler beim Initialisieren der Datenbank:\n\n{ex.Message}\n\n" +
                    "Bitte stellen Sie sicher, dass MySQL läuft und die Zugangsdaten korrekt sind.",
                    "Datenbankfehler",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }

            Application.Run(new MainForm());
        }
    }
}
