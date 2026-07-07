using FontAwesome.Sharp;
using SaimDataCopy.Styles.Authentification;

namespace SaimDataCopy.Views.Authentification.Components
{
    public class AuthPasswordTextBox : UserControl
    {
        private readonly TextBox txtMotDePasse;
        private readonly IconButton btnAfficherMasquer;

        public AuthPasswordTextBox()
        {
            Height = 38;
            Width = 360;
            BackColor = Color.Transparent;

            txtMotDePasse = new TextBox
            {
                Location = new Point(0, 0),
                Width = 320,
                Height = 38,
                UseSystemPasswordChar = true
            };

            AuthTextBoxStyle.Appliquer(txtMotDePasse);

            btnAfficherMasquer = new IconButton
            {
                Width = 38,
                Height = 38,
                Location = new Point(322, 0),

                IconChar = IconChar.Eye,
                IconColor = Color.Gray,
                IconSize = 18,

                FlatStyle = FlatStyle.Flat,
                BackColor = Color.White,
                Cursor = Cursors.Hand
            };

            btnAfficherMasquer.FlatAppearance.BorderSize = 0;

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