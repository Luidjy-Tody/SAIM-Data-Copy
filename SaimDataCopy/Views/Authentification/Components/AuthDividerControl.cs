using SaimDataCopy.Styles.Authentification.Identification;

namespace SaimDataCopy.Views.Authentification.Components
{
    public class AuthDividerControl : UserControl
    {
        public AuthDividerControl()
        {
            Height = 1;
            Width = 360;
            BackColor = IdentificationStyle.Separateur;
        }
    }
}