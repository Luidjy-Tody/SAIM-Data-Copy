using SaimDataCopy.Styles.Authentification.Identification;

namespace SaimDataCopy.Views.Authentification.Components
{
    public class AuthHeaderControl : UserControl
    {
        public AuthHeaderControl()
        {
            Size = new Size(520, 90);
            BackColor = Color.Transparent;

            Label lblTitre = new Label
            {
                Text = "SaimDataCopy",
                Font = IdentificationStyle.TitreApplication(),
                ForeColor = Color.White,
                TextAlign = ContentAlignment.MiddleCenter,
                Location = new Point(0, 0),
                Size = new Size(520, 45)
            };

            Label lblSousTitre = new Label
            {
                Text = "Copie automatique des données entre serveurs",
                Font = IdentificationStyle.SousTitreApplication(),
                ForeColor = IdentificationStyle.VertClair,
                TextAlign = ContentAlignment.MiddleCenter,
                Location = new Point(0, 50),
                Size = new Size(520, 25)
            };

            Controls.Add(lblTitre);
            Controls.Add(lblSousTitre);
        }
    }
}