using System;
using System.Collections.Generic;

namespace WMTippTool.Database
{
    /// <summary>
    /// Diese Klasse ist unser "Gehirn" für das SQL-Terminal. 
    /// Sie ist 'static', was bedeutet, dass sie im gesamten Programm nur einmal existiert.
    /// Immer wenn wir etwas in der Datenbank machen (z.B. ein Spiel hinzufügen), 
    /// rufen wir hier die Methode 'Log' auf, um den Befehl zu speichern.
    /// </summary>
    public static class QueryLogger
    {
        // Eine Liste, die alle unsere gesammelten Log-Einträge speichert.
        // Das ist wie ein digitales Notizbuch für das Programm.
        private static readonly List<string> logs = new();

        // Ein Event, das immer dann ausgelöst wird, wenn ein neuer Log-Eintrag hinzukommt.
        // Das LogForm (unser Terminal-Fenster) wird sich bei diesem Event anmelden, 
        // um immer sofort Bescheid zu wissen, wenn es etwas Neues zum Anzeigen gibt.
        public static event Action<string>? OnLogAdded;

        /// <summary>
        /// Speichert eine neue Nachricht und sagt allen Bescheid, die zuhören (z.B. dem Terminal).
        /// </summary>
        /// <param name="message">Die Nachricht oder der SQL-Befehl, der gespeichert werden soll.</param>
        public static void Log(string message)
        {
            // Füge das aktuelle Datum und die genaue Uhrzeit vor die Nachricht
            // So sieht das dann z.B. so aus: "[10:45:12] DELETE FROM spiele..."
            string timestamp = DateTime.Now.ToString("HH:mm:ss");
            string fullMessage = $"[{timestamp}] {message}";

            // Füge die Nachricht zu unserem Notizbuch (der Liste) hinzu
            logs.Add(fullMessage);

            // Löse das Event aus, um dem Terminal (falls es offen ist) Bescheid zu sagen,
            // dass eine neue Nachricht da ist.
            OnLogAdded?.Invoke(fullMessage);
        }

        /// <summary>
        /// Gibt alle bisher gesammelten Logs zurück.
        /// Das brauchen wir, wenn wir das Terminal-Fenster öffnen und sehen wollen, 
        /// was bisher alles passiert ist.
        /// </summary>
        public static IReadOnlyList<string> GetAllLogs()
        {
            // Gibt unsere Liste als schreibgeschützte Ansicht (IReadOnlyList) zurück
            return logs.AsReadOnly();
        }
    }
}
