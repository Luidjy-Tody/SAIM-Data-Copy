namespace SaimDataCopy.Helpers
{
    // Helper central pour gérer les chemins de stockage de l'application.
    // Les fichiers modifiables ne doivent pas être stockés dans Program Files,
    // car Windows bloque l'écriture dans ce dossier après installation.
    public static class CheminApplicationHelper
    {
        public static string ObtenirDossierApplication()
        {
            string dossierApplication = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                "SAIM Ltd",
                "SaimDataCopy"
            );

            if (!Directory.Exists(dossierApplication))
            {
                Directory.CreateDirectory(dossierApplication);
            }

            return dossierApplication;
        }

        public static string ObtenirDossierData()
        {
            string dossierData = Path.Combine(ObtenirDossierApplication(), "Data");

            if (!Directory.Exists(dossierData))
            {
                Directory.CreateDirectory(dossierData);
            }

            return dossierData;
        }
    }
}