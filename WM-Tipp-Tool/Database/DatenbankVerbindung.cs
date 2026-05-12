using System;
using System.IO;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

/*
 * Projekt: WM-Tipp-Tool 2026
 * Entwickler: voiscko (Telegram: @voiscko)
 */

namespace WMTippTool.Database
{
    /// <summary>
    /// Diese Klasse ist quasi unser "Telefon" zur MySQL-Datenbank.
    /// Sie regelt den Aufbau der Verbindung und liest das Passwort aus der "db.config" Datei.
    /// </summary>
    public static class DatenbankVerbindung
    {
        // Hier speichern wir die gelesenen Konfigurationsdaten (wie Server, Passwort etc.)
        // Startwert ist null, weil die Daten noch nicht geladen wurden.
        private static DatenbankKonfiguration _config = null;

        /// <summary>
        /// Holt sich die Konfiguration. Wenn sie noch nicht geladen wurde,
        /// wird sie jetzt aus der Datei "db.config" geladen. (Lazy Loading)
        /// </summary>
        private static DatenbankKonfiguration Config
        {
            get
            {
                // Falls die Konfiguration noch nicht geladen wurde, laden wir sie jetzt.
                if (_config == null)
                {
                    _config = KonfigurationLaden();
                }
                return _config;
            }
        }

        /// <summary>
        /// Erstellt ein neues "Telefonkabel" (Verbindung) zur Datenbank.
        /// </summary>
        public static MySqlConnection VerbindungAbrufen()
        {
            return new MySqlConnection(Config.ZuVerbindungsString());
        }

        /// <summary>
        /// Diese Methode wird beim Start ausgeführt. Sie erstellt die Datenbank
        /// und die Tabellen, falls diese noch nicht existieren.
        /// </summary>
        public static void DatenbankInitialisieren()
        {
            DatenbankKonfiguration cfg = Config;

            // Zuerst verbinden wir uns OHNE eine bestimmte Datenbank anzugeben,
            // damit wir den Befehl "CREATE DATABASE" (Datenbank erstellen) ausführen können.
            string initConnStr =
                "Server=" + cfg.Host + ";Port=" + cfg.Port + ";Uid=" + cfg.User + ";Pwd=" + cfg.Password + ";CharSet=utf8mb4;";

            using (MySqlConnection initConn = new MySqlConnection(initConnStr))
            {
                initConn.Open();

                string createDbSql = "CREATE DATABASE IF NOT EXISTS `" + cfg.Database + "` CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;";
                // Protokollieren in unser SQL-Terminal!
                SQLProtokollierer.Protokollieren(createDbSql);

                using (MySqlCommand createDb = new MySqlCommand(createDbSql, initConn))
                {
                    createDb.ExecuteNonQuery(); // Befehl an MySQL schicken!
                }

                initConn.Close();
            }

            // Jetzt verbinden wir uns MIT der neuen (oder bestehenden) Datenbank, um Tabellen anzulegen.
            using (MySqlConnection conn = VerbindungAbrufen())
            {
                conn.Open();

                // Der SQL-Befehl für die Tabelle "spiele"
                string createSpiele = @"
                CREATE TABLE IF NOT EXISTS spiele (
                    id INT AUTO_INCREMENT PRIMARY KEY,
                    team1 VARCHAR(100) NOT NULL,
                    team2 VARCHAR(100) NOT NULL,
                    datum DATETIME NOT NULL,
                    ergebnis_team1 INT NULL,
                    ergebnis_team2 INT NULL
                ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;";

                // Der SQL-Befehl für die Tabelle "tipps"
                string createTipps = @"
                CREATE TABLE IF NOT EXISTS tipps (
                    id INT AUTO_INCREMENT PRIMARY KEY,
                    spiel_id INT NOT NULL,
                    benutzername VARCHAR(100) NOT NULL,
                    tipp_team1 INT NOT NULL,
                    tipp_team2 INT NOT NULL,
                    punkte INT NOT NULL DEFAULT 0,
                    FOREIGN KEY (spiel_id) REFERENCES spiele(id) ON DELETE CASCADE
                ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;";

                // Ab in unser SQL-Terminal damit!
                SQLProtokollierer.Protokollieren("CREATE TABLE IF NOT EXISTS spiele ...");
                using (MySqlCommand cmd = new MySqlCommand(createSpiele, conn))
                {
                    cmd.ExecuteNonQuery();
                }

                SQLProtokollierer.Protokollieren("CREATE TABLE IF NOT EXISTS tipps ...");
                using (MySqlCommand cmd = new MySqlCommand(createTipps, conn))
                {
                    cmd.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Ein kurzer Ping an die Datenbank, um zu schauen ob XAMPP läuft.
        /// </summary>
        public static bool VerbindungTesten()
        {
            try
            {
                using (MySqlConnection conn = VerbindungAbrufen())
                {
                    conn.Open();
                    return true; // Hat geklappt!
                }
            }
            catch
            {
                return false; // MySQL ist offline (oder falsches Passwort)
            }
        }

        // =====================================================================
        // Config laden (Hier lesen wir die Textdatei aus)
        // =====================================================================

        // Wo liegt die db.config Datei? Genau da, wo auch die .exe Datei unseres Programms liegt.
        private static readonly string ConfigPath =
            Path.Combine(AppContext.BaseDirectory, "db.config");

        private static DatenbankKonfiguration KonfigurationLaden()
        {
            // Falls jemand das Programm zum ersten Mal startet und die Datei nicht hat...
            if (!File.Exists(ConfigPath))
            {
                // ... erstellen wir einfach eine Standard-Datei für ihn!
                DatenbankKonfiguration defaultCfg = new DatenbankKonfiguration();
                StandardKonfigurationSchreiben(defaultCfg);

                MessageBox.Show(
                    "⚙️ Keine db.config gefunden!\n\n" +
                    "Eine neue db.config mit Standardwerten wurde erstellt:\n" +
                    "  📄 " + ConfigPath + "\n\n" +
                    "Bitte öffne die Datei und trage dein MySQL-Passwort ein.\n" +
                    "Danach starte das Programm neu.",
                    "Konfiguration fehlt",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return defaultCfg;
            }

            // Wenn die Datei da ist, lesen wir sie Zeile für Zeile durch.
            DatenbankKonfiguration cfg = new DatenbankKonfiguration();
            foreach (string line in File.ReadAllLines(ConfigPath))
            {
                // Kommentare (Zeilen die mit '#' anfangen) und leere Zeilen ignorieren
                if (string.IsNullOrWhiteSpace(line) || line.TrimStart().StartsWith('#'))
                {
                    continue;
                }

                // Wir suchen nach dem '=' (z.B. bei "User=root")
                int sep = line.IndexOf('=');
                if (sep < 0)
                {
                    continue;
                }

                // Links vom '=' ist der Schlüssel (key), rechts ist der Wert (value)
                string key   = line.Substring(0, sep).Trim();
                string value = line.Substring(sep + 1).Trim();

                // Je nachdem, wie der Schlüssel heißt, speichern wir den Wert.
                switch (key)
                {
                    case "Host":
                        cfg.Host = value;
                        break;
                    case "Port":
                        // int.TryParse: Variable muss vorher deklariert werden
                        int p = 0;
                        if (int.TryParse(value, out p))
                        {
                            cfg.Port = p;
                        }
                        else
                        {
                            cfg.Port = 3306;
                        }
                        break;
                    case "Database":
                        cfg.Database = value;
                        break;
                    case "User":
                        cfg.User = value;
                        break;
                    case "Password":
                        cfg.Password = value;
                        break;
                }
            }

            return cfg;
        }

        /// <summary>
        /// Schreibt eine Standard db.config Datei.
        /// </summary>
        private static void StandardKonfigurationSchreiben(DatenbankKonfiguration cfg)
        {
            try
            {
                File.WriteAllText(ConfigPath,
                    "# WM-Tipp-Tool — Datenbank-Konfiguration\n" +
                    "# Diese Datei NICHT in Git einchecken! (steht in .gitignore)\n\n" +
                    "Host="     + cfg.Host     + "\n" +
                    "Port="     + cfg.Port     + "\n" +
                    "Database=" + cfg.Database + "\n" +
                    "User="     + cfg.User     + "\n" +
                    "Password=" + cfg.Password + "\n");
            }
            catch
            {
                // Fehler beim Schreiben? Einfach ignorieren, Programm nimmt dann die Standardwerte aus dem RAM.
            }
        }

        // =====================================================================
        // Hilfsklasse für die Konfigurationsdaten
        // =====================================================================

        /// <summary>
        /// Ein "Behälter" (Klasse) für unsere Anmeldedaten.
        /// Die Werte hier sind die Standardwerte (funktioniert meistens für lokales XAMPP).
        /// </summary>
        private class DatenbankKonfiguration
        {
            public string Host     { get; set; } = "localhost";
            public int    Port     { get; set; } = 3306;
            public string Database { get; set; } = "wm_tipp_db";
            public string User     { get; set; } = "root";
            public string Password { get; set; } = ""; // Häufig ist das Passwort in XAMPP leer ("")

            /// <summary>
            /// Baut den fertigen Text zusammen, den MySQL braucht, um sich anzumelden (ConnectionString).
            /// </summary>
            public string ZuVerbindungsString()
            {
                return "Server=" + Host + ";Port=" + Port + ";Database=" + Database + ";Uid=" + User + ";Pwd=" + Password + ";CharSet=utf8mb4;";
            }
        }
    }
}
