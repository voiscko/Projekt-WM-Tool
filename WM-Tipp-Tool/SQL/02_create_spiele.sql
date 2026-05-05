-- WM-Tipp-Tool: Tabelle spiele erstellen
USE wm_tipp_db;

CREATE TABLE IF NOT EXISTS spiele (
    id INT AUTO_INCREMENT PRIMARY KEY,
    team1 VARCHAR(100) NOT NULL,
    team2 VARCHAR(100) NOT NULL,
    datum DATETIME NOT NULL,
    ergebnis_team1 INT NULL,
    ergebnis_team2 INT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
