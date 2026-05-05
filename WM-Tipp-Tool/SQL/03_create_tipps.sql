-- WM-Tipp-Tool: Tabelle tipps erstellen
USE wm_tipp_db;

CREATE TABLE IF NOT EXISTS tipps (
    id INT AUTO_INCREMENT PRIMARY KEY,
    spiel_id INT NOT NULL,
    benutzername VARCHAR(100) NOT NULL,
    tipp_team1 INT NOT NULL,
    tipp_team2 INT NOT NULL,
    punkte INT NOT NULL DEFAULT 0,
    FOREIGN KEY (spiel_id) REFERENCES spiele(id) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
