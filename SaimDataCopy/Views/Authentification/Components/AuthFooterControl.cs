using SaimDataCopy.Styles.Authentification.Identification;

namespace SaimDataCopy.Views.Authentification.Components
{
    public class AuthFooterControl : UserControl
    {
        public AuthFooterControl()
        {
            Size = new Size(520, 35);
            BackColor = Color.Transparent;

            Label lblFooter = new Label
            {
                Text = "SAIM LTD — v1.0.0",
                Font = IdentificationStyle.TextePetit(),
                ForeColor = IdentificationStyle.VertClair,
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Fill
            };

            Controls.Add(lblFooter);
        }
    }
}