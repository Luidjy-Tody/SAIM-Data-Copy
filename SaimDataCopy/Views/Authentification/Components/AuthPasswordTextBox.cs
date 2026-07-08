using FontAwesome.Sharp;
using SaimDataCopy.Styles.Authentification.Identification;

namespace SaimDataCopy.Views.Authentification.Components
{
    public class AuthPasswordTextBox : UserControl
    {
        private readonly TextBox txtMotDePasse;
        private readonly IconButton btnAfficherMasquer;

        public AuthPasswordTextBox()
        {
            Size = new Size(430, 42);
            BackColor = Color.White;

            txtMotDePasse = new TextBox
            {
                Location = new Point(0, 0),
                Size = new Size(400, 24),
                UseSystemPasswordChar = true,
                PlaceholderText = "Entrez votre mot de passe"
            };

            IdentificationStyle.AppliquerTextBox(txtMotDePasse);

            btnAfficherMasquer = new IconButton
            {
                Location = new Point(402, -6),
                Size = new Size(28, 34),
                IconChar = IconChar.Eye,
                IconColor = Color.FromArgb(100, 116, 139),
                IconSize = 14,
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.White,
                Cursor = Cursors.Hand
            };

            btnAfficherMasquer.FlatAppearance.BorderSize = 0;
            btnAfficherMasquer.FlatAppearance.MouseOverBackColor = Color.White;
            btnAfficherMasquer.FlatAppearance.MouseDownBackColor = Color.White;

            btnAfficherMasquer.Click += BtnAfficherMasquer_Click;

            Controls.Add(txtMotDePasse);
            Controls.Add(btnAfficherMasquer);
        }

        public string Texte
        {
            get => txtMotDePasse.Text;
            set => txtMotDePasse.Text = value;
        }

        public TextBox TextBox => txtMotDePasse;

        private void BtnAfficherMasquer_Click(object? sender, EventArgs e)
        {
            txtMotDePasse.UseSystemPasswordChar = !txtMotDePasse.UseSystemPasswordChar;

            btnAfficherMasquer.IconChar = txtMotDePasse.UseSystemPasswordChar
                ? IconChar.Eye
                : IconChar.EyeSlash;
        }
    }
}