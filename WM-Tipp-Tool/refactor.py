import os
import re

directory = "/Users/voiscko/Documents/Projekt WM Tool/WM-Tipp-Tool"

replacements = {
    r"\bDBConnection\b": "DatenbankVerbindung",
    r"\bDbConfig\b": "DatenbankKonfiguration",
    r"\bInitDatabase\b": "DatenbankInitialisieren",
    r"\bGetConnection\b": "VerbindungAbrufen",
    r"\bTestConnection\b": "VerbindungTesten",
    r"\bLoadConfig\b": "KonfigurationLaden",
    r"\bWriteFallbackConfig\b": "StandardKonfigurationSchreiben",
    r"\bToConnectionString\b": "ZuVerbindungsString",
    r"\bQueryLogger\b": "SQLProtokollierer",
    r"\bOnLogAdded\b": "BeiNeuemProtokollEintrag",
    r"\bLog\b": "Protokollieren",
    r"\bGetAllLogs\b": "AlleProtokolleAbrufen",
    r"\bLogForm\b": "ProtokollForm",
    r"\bDatabaseViewForm\b": "DatenbankAnsichtForm",
    r"\bInitializeComponent\b": "KomponentenInitialisieren",
    
    r"\bBtnSpiele_Click\b": "BtnSpiele_Klick",
    r"\bBtnTippen_Click\b": "BtnTippen_Klick",
    r"\bBtnRangliste_Click\b": "BtnRangliste_Klick",
    r"\bBtnDatabase_Click\b": "BtnDatenbank_Klick",
    r"\bBtnLogs_Click\b": "BtnProtokolle_Klick",
    r"\bWrapButton\b": "ButtonVerpacken",
    r"\bCreateMenuButton\b": "MenueButtonErstellen",
    r"\bCheckDbStatus\b": "DatenbankStatusPruefen",
    r"\blblDbStatus\b": "lblDatenbankStatus",
    r"\bbtnDatabase\b": "btnDatenbank",
    r"\bbtnLogs\b": "btnProtokolle",

    r"\bBtnAktualisieren_Click\b": "BtnAktualisieren_Klick",
    r"\bBtnZurueck_Click\b": "BtnZurueck_Klick",
    r"\bCboSpiele_SelectedIndexChanged\b": "CboSpiele_AuswahlGeaendert",
    r"\bBtnTippSpeichern_Click\b": "BtnTippSpeichern_Klick",
    r"\bStyleLabel\b": "LabelStylen",
    r"\bCreateNumericUpDown\b": "ZahlenFeldErstellen",
    r"\bStyleDataGridView\b": "TabelleStylen",
    r"\bspielIds\b": "spielIDs",

    r"\bDtpPanel_SizeChanged\b": "DtpPanel_GroesseGeaendert",
    r"\bBtnHinzufuegen_Click\b": "BtnHinzufuegen_Klick",
    r"\bBtnLoeschen_Click\b": "BtnLoeschen_Klick",
    r"\bStyleTextBox\b": "TextFeldStylen",
    r"\bCreateActionButton\b": "AktionsButtonErstellen",

    r"\bBtnErgebnisEintragen_Click\b": "BtnErgebnisEintragen_Klick",
    r"\bDgvRangliste_RowPrePaint\b": "DgvRangliste_ZeileVorZeichnen",

    r"\bcboTables\b": "cboTabellen",
    r"\bdgvData\b": "dgvDaten",
    r"\bCboTables_SelectedIndexChanged\b": "CboTabellen_AuswahlGeaendert",
    r"\bLoadTableData\b": "TabellenDatenLaden",

    r"\btxtLogs\b": "txtProtokolle",
    r"\bQueryLogger_OnLogAdded\b": "SQLProtokollierer_BeiNeuemProtokollEintrag",
    r"\bAppendLog\b": "ProtokollHinzufuegen",
    
    r"\brootLayout\b": "hauptLayout",
    r"\binputLayout\b": "eingabeLayout",
    r"\bpnlHeader\b": "pnlKopfbereich",
    r"\bpnlInput\b": "pnlEingabe",
    r"\blblSpielLabel\b": "lblSpielHinweis",
    
    r"\btopPanel\b": "oberesPanel",
    r"\bsplitLayout\b": "geteiltesLayout",
    r"\bdtpPanel\b": "dtpPanel",
    r"\bergebnisLayout\b": "ergebnisLayout",
    r"\bspielZeileLayout\b": "spielZeileLayout",
    r"\bergebnisEingabeFlow\b": "ergebnisEingabeFlow",
    r"\btippFlow\b": "tippFlussLayout"
}

for root, _, files in os.walk(directory):
    for file in files:
        if file.endswith(".cs") or file.endswith(".csproj") or file == "Program.cs":
            filepath = os.path.join(root, file)
            with open(filepath, "r", encoding="utf-8") as f:
                content = f.read()
            
            new_content = content
            for pattern, replacement in replacements.items():
                new_content = re.sub(pattern, replacement, new_content)
                
            if new_content != content:
                with open(filepath, "w", encoding="utf-8") as f:
                    f.write(new_content)
                print(f"Updated {filepath}")

print("Done.")
