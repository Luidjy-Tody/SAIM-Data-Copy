namespace SaimDataCopy.Helpers
{
    public static class AuthentificationConnexionHelper
    {
        private const string VariableEnvironnement = "SAIMDATACOPY_AUTH_CONNECTION";

        public static string ObtenirChaineConnexion()
        {
            string? chaineConnexionVariable = Environment.GetEnvironmentVariable(VariableEnvironnement);

            if (!string.IsNullOrWhiteSpace(chaineConnexionVariable))
            {
                return chaineConnexionVariable.Trim();
            }

            return "server=localhost;port=3308;database=saimdatacopy_auth;user=root;password=root3308";
        }
    }
}