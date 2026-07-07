using SaimDataCopy.Styles.Authentification;

namespace SaimDataCopy.Views.Authentification.Components
{
    public class AuthMessageControl : UserControl
    {
        private readonly Label lblMessage;

        public AuthMessageControl()
        {
            Height = 25;
            Width = 360;
            BackColor = Color.Transparent;

            lblMessage = new Label
            {
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = AuthentificationFormStyle.LabelChamp(),
                ForeColor = AuthentificationFormStyle.RougeErreur,
                Visible = false
            };

            Controls.Add(lblMessage);
        }

        public void AfficherErreur(string message)
        {
            lblMessage.Text = message;
            lblMessage.ForeColor = AuthentificationFormStyle.RougeErreur;
            lblMessage.Visible = true;
        }

        public void AfficherSucces(string message)
        {
            lblMessage.Text = message;
            lblMessage.ForeColor = AuthentificationFormStyle.VertSucces;
            lblMessage.Visible = true;
        }

        public void Effacer()
        {
            lblMessage.Text = string.Empty;
            lblMessage.Visible = false;
        }
    }
}