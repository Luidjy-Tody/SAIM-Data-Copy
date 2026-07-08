using SaimDataCopy.Styles.Authentification.Identification;

namespace SaimDataCopy.Views.Authentification.Components
{
    public class AuthMessageControl : UserControl
    {
        private readonly Label lblMessage;

        public AuthMessageControl()
        {
            Height = 25;
            Width = 400;
            BackColor = Color.White;
            Visible = false;

            lblMessage = new Label
            {
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = IdentificationStyle.LabelChamp(),
                ForeColor = IdentificationStyle.RougeErreur,
                BackColor = Color.White,
                Visible = true
            };

            Controls.Add(lblMessage);
        }

        public void AfficherErreur(string message)
        {
            lblMessage.Text = message;
            lblMessage.ForeColor = IdentificationStyle.RougeErreur;
            Visible = true;
        }

        public void AfficherSucces(string message)
        {
            lblMessage.Text = message;
            lblMessage.ForeColor = IdentificationStyle.VertSucces;
            Visible = true;
        }

        public void Effacer()
        {
            lblMessage.Text = string.Empty;
            Visible = false;
        }
    }
}