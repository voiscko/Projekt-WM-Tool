using System;
using System.IO;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace WMTippTool.Database
{
    /// <summary>
    /// Diese Klasse ist quasi unser "Telefon" zur MySQL-Datenbank.
    /// Sie regelt den Aufbau der Verbindung und liest das Passwort aus der "db.config" Datei.
    /// </summary>
    public static class DBConnection
    {
        // Hier speichern wir die gelesenen Konfigurationsdaten (wie Server, Passwort etc.)
        private static DbConfig? _config;

        /// <summary>
        /// Holt sich die Konfiguration. Wenn sie noch nicht geladen wurde, 
        /// wird sie jetzt aus der Datei "db.config" geladen. (Lazy Loading)
        /// </summary>
        private static DbConfig Config => _config ??= LoadConfig();

        /// <summary>
        /// Erstellt ein neues "Telefonkabel" (Verbindung) zur Datenbank.
        /// </summary>
        public static MySqlConnection GetConnection()
        {
            return new MySqlConnection(Config.ToConnectionString());
        }

        /// <summary>
        /// Diese Methode wird beim Start ausgeführt. Sie erstellt die Datenbank 
        /// und die Tabellen, falls diese noch nicht existieren.
        /// </summary>
        public static void InitDatabase()
        {
            var cfg = Config;

            // Zuerst verbinden wir uns OHNE eine bestimmte Datenbank anzugeben,
            // damit wir den Befehl "CREATE DATABASE" (Datenbank erstellen) ausführen können.
            string initConnStr =
                $"Server={cfg.Host};Port={cfg.Port};Uid={cfg.User};Pwd={cfg.Password};CharSet=utf8mb4;";

            using var initConn = new MySqlConnection(initConnStr);
            initConn.Open();

            string createDbSql = $"CREATE DATABASE IF NOT EXISTS `{cfg.Database}` CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;";
            // Log in unser SQL-Terminal!
            QueryLogger.Log(createDbSql);

            using var createDb = new MySqlCommand(createDbSql, initConn);
            createDb.ExecuteNonQuery(); // Befehl an MySQL schicken!
            initConn.Close();

            // Jetzt verbinden wir uns MIT der neuen (oder bestehenden) Datenbank, um Tabellen anzulegen.
            using var conn = GetConnection();
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
            QueryLogger.Log("CREATE TABLE IF NOT EXISTS spiele ...");
            using (var cmd = new MySqlCommand(createSpiele, conn))
                cmd.ExecuteNonQuery();

            QueryLogger.Log("CREATE TABLE IF NOT EXISTS tipps ...");
            using (var cmd = new MySqlCommand(createTipps, conn))
                cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// Ein kurzer Ping an die Datenbank, um zu schauen ob XAMPP läuft.
        /// </summary>
        public static bool TestConnection()
        {
            try
            {
                using var conn = GetConnection();
                conn.Open();
                return true; // Hat geklappt!
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

        private static DbConfig LoadConfig()
        {
            // Falls jemand das Programm zum ersten Mal startet und die Datei nicht hat...
            if (!File.Exists(ConfigPath))
            {
                // ... erstellen wir einfach eine Standard-Datei für ihn!
                var defaultCfg = new DbConfig();
                WriteFallbackConfig(defaultCfg);

                MessageBox.Show(
                    "⚙️ Keine db.config gefunden!\n\n" +
                    "Eine neue db.config mit Standardwerten wurde erstellt:\n" +
                    $"  📄 {ConfigPath}\n\n" +
                    "Bitte öffne die Datei und trage dein MySQL-Passwort ein.\n" +
                    "Danach starte das Programm neu.",
                    "Konfiguration fehlt",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return defaultCfg;
            }

            // Wenn die Datei da ist, lesen wir sie Zeile für Zeile durch.
            var cfg = new DbConfig();
            foreach (string line in File.ReadAllLines(ConfigPath))
            {
                // Kommentare (Zeilen die mit '#' anfangen) und leere Zeilen ignorieren
                if (string.IsNullOrWhiteSpace(line) || line.TrimStart().StartsWith('#'))
                    continue;

                // Wir suchen nach dem '=' (z.B. bei "User=root")
                int sep = line.IndexOf('=');
                if (sep < 0) continue;

                // Links vom '=' ist der Schlüssel (key), rechts ist der Wert (value)
                string key = line[..sep].Trim();
                string value = line[(sep + 1)..].Trim();

                // Je nachdem, wie der Schlüssel heißt, speichern wir den Wert.
                switch (key)
                {
                    case "Host":     cfg.Host     = value; break;
                    case "Port":     cfg.Port      = int.TryParse(value, out int p) ? p : 3306; break;
                    case "Database": cfg.Database  = value; break;
                    case "User":     cfg.User      = value; break;
                    case "Password": cfg.Password  = value; break;
                }
            }

            return cfg;
        }

        /// <summary>
        /// Schreibt eine Standard db.config Datei.
        /// </summary>
        private static void WriteFallbackConfig(DbConfig cfg)
        {
            try
            {
                File.WriteAllText(ConfigPath,
                    "# WM-Tipp-Tool — Datenbank-Konfiguration\n" +
                    "# Diese Datei NICHT in Git einchecken! (steht in .gitignore)\n\n" +
                    $"Host={cfg.Host}\n" +
                    $"Port={cfg.Port}\n" +
                    $"Database={cfg.Database}\n" +
                    $"User={cfg.User}\n" +
                    $"Password={cfg.Password}\n");
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
        private class DbConfig
        {
            public string Host     { get; set; } = "localhost";
            public int    Port     { get; set; } = 3306;
            public string Database { get; set; } = "wm_tipp_db";
            public string User     { get; set; } = "root";
            public string Password { get; set; } = ""; // Häufig ist das Passwort in XAMPP leer ("")

            /// <summary>
            /// Baut den fertigen Text zusammen, den MySQL braucht, um sich anzumelden (ConnectionString).
            /// </summary>
            public string ToConnectionString() =>
                $"Server={Host};Port={Port};Database={Database};Uid={User};Pwd={Password};CharSet=utf8mb4;";
        }
    }
}
